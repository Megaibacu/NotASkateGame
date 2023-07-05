using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Cinemachine;

public class Tricking : MonoBehaviour
{
    public SkateController sC;
    public ScoreManager scoreM;
    private Animator anim;
    private Rigidbody rB;

    [Header("===============Skate Tricks===============")]
    public SkateTricks[] tricks; //All the tricks that can be perfornmed by the player. In the future it would be cool to make this customizable
    public SkateTricks[] grabTricks;
    public SkateTricks airTurning;
    public SkateTricks[] grindTypes;
    public float comboTimer; // The time that the player has to do another trick to continue the combo
    public SkateTricks currentTrick;
    [HideInInspector] public bool flipTricking; //Checks if the player is mid trick to know if they have to fall
    [HideInInspector] public bool grabTricking; //Checks if the player is mid trick to know if they have to fall
    [HideInInspector] public bool fall; //If the player touches the ground and is doing a tricks then they fall

    [Header("===============Boost===============")]
    public float tailgrabTimer; //Times the time spent while doing a tailgrab in order to decide how long the player has to boost
    public float boostTimer;
    public bool grabBoost;
    public float finalBoostSpeed, finalBoostTimer;
    public float smallBoostTime, mediumBoostTime, bigBoostTime;
    public float smallBoostSpeed, mediumBoostSpeed, bigBoostSpeed;

    [Header("===============Boost Effects===============")]
    public float minSpeedForEffect;

    public GameObject boostSpeedLines;
    public CinemachineVirtualCamera virtualCam;
    private CinemachineBasicMultiChannelPerlin _virtualCBMP;
    public PostProcessVolume postVolume;
    private ChromaticAberration _cA;
    private DepthOfField _doF;
    private LensDistortion _lD;
    private MotionBlur _mB;
    public float chromaticAberrationIntensity;
    private float ogCAIntensity;
    public float doFFocusDistance;
    private float ogDoFDistance;

    [Header("===============Air Bomb===============")]
    public float maxGravity;
    public float gravityAccelerationmultiplier;

    [Header("===============Air Movement===============")]
    public float min360LandAngle;
    public float max360LandAngle;
    public float min180LandAngle;
    public float max180LandAngle;
    public float minSketchyLandAngle, maxSketchyLandAngle;
    public float minPerfectLandAngle, maxPerfectLandAngle;
    public float airTurningValue;
    float landAngle;
    bool turnDetection;

    [Header("===============Grinding===============")]
    public bool darkSlide;

    [Header("===============Slope Movement===============")]
    SlopeDirection slopeDir;

    void Start()
    {
        sC = GetComponent<SkateController>();
        scoreM = GetComponent<ScoreManager>();
        rB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        //slopeDir = GetComponent<SlopeDirection>();

        //VolumeEffects
        postVolume.profile.TryGetSettings(out _cA);
        postVolume.profile.TryGetSettings(out _doF);
        postVolume.profile.TryGetSettings(out _lD);
        postVolume.profile.TryGetSettings(out _mB);
        ogCAIntensity = _cA.intensity.value;
        ogDoFDistance = _doF.focusDistance.value;

        _virtualCBMP = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        DeactivateSpeedEffects();
    }

    void Update()
    {
        FallManager();
        GrabBoost();

        if (sC.grounded)
		{
            sC.momentum = rB.velocity;
            EndTrick();
        }
        else
        {
            AirTurning();
        }

        //SkateSpeedEffects();
    }

    #region =====FLIP TRICKS=====
    public void FlipTrick()
    {
        if (!sC.grounded && !flipTricking && !grabTricking)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.s_flick, transform.position);
            string currentAnim = string.Empty;
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                currentAnim = tricks[0].animationTriggered;
                currentTrick = tricks[0];
                Debug.Log("Pop Shuvit");
            }
            else if (Input.GetAxisRaw("Vertical") > 0)
            {
                currentAnim = tricks[1].animationTriggered;
                currentTrick = tricks[1];
                Debug.Log("Impossible");
            }
            else if (Input.GetAxisRaw("Horizontal") < 0)
            {
                currentAnim = tricks[2].animationTriggered;
                currentTrick = tricks[2];
                Debug.Log("Kickflip");
            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                currentAnim = tricks[3].animationTriggered;
                currentTrick = tricks[3];
                Debug.Log("Heelflip");
            }
            else
            {
                currentAnim = "KickFlip";
            }

            anim.SetTrigger(currentAnim);
            flipTricking = true;
        }
    }
    #endregion

    #region =====GRAB TRICKS=====
    public void StartGrabTricks()
    {
        if (!sC.grounded && !flipTricking)
        {
            tailgrabTimer += Time.deltaTime;
            grabTricking = true;
            scoreM.curretnScore += (int)(grabTricks[0].scoreAwarded * Time.deltaTime);
            TailGrabBoostTimer();
            anim.SetBool("GrabTricking", true);
            grabBoost = false;
        }
    }

    public void EndGrabTrick()
    {
        boostTimer = 0;
        grabBoost = true;
        Vector3 boostVel = sC.orientation.transform.forward * finalBoostSpeed;
        sC.rb.velocity = new Vector3(boostVel.x, sC.rb.velocity.y, boostVel.z);
        finalBoostSpeed = 0;
        tailgrabTimer = 0;
        grabTricking = false;
        anim.SetBool("GrabTricking", false);
    }
    #endregion

    #region =====AIR BOMB=====
    float ogLocalGravity;
    public void AirBomb()
    {
        if (!sC.grounded)
        {
            sC.localGravity = Mathf.Lerp(sC.localGravity, maxGravity, gravityAccelerationmultiplier * Time.deltaTime);
            ActivateSpeedEffects();
        }
    }

    public void EndAirBomb()
    {
        sC.localGravity = Mathf.Lerp(sC.localGravity, ogLocalGravity, Time.deltaTime);
        DeactivateSpeedEffects();
    }
    #endregion

    #region =====360s & 180s=====
    public void AirTurning()
    {
        //Calculating how many turns
        float angle = Vector3.Angle(sC.orientation.transform.forward, sC.groundOrientation.forward);

        if (angle >= max180LandAngle && angle <= 180)
        {
            if (turnDetection)
            {
                airTurningValue += 180;
                turnDetection = false;
            }
        }
        else { turnDetection = true; }
    }

    public void AirLanding()
    {
        //NEED TO CHANGE THIS IN THE FUTURE
        //IF THE PLAYER IS NOT ON A SLOPE STEEP ENOUGH DO THIS
        //ELSE THEN ADD MOMENTUM TO THE PLAYER FOLLOWING THE FORWARD OF THAT SLOPE 

        landAngle = Vector3.Angle(sC.orientation.transform.forward, sC.groundOrientation.forward); //The angle that the player lands in relation to the direction before they jumped
        
        int airScore = (int)(airTurning.scoreAwarded * airTurningValue); //Calcualtes the score that should be awarded to the player. Changes depending on how many times the player has turned in the air
        if ((landAngle >= min360LandAngle && landAngle <= max360LandAngle) || (landAngle >= max180LandAngle && landAngle <= min180LandAngle))
        {
            sC.orientation.transform.forward = new Vector3(sC.groundOrientation.forward.x, 0, sC.groundOrientation.forward.z);
            currentTrick = airTurning;
            AddPoints(airScore);
            //point addition = TurnTrick.scoreAdded * timesTurned
        }
        else
        {
            anim.SetTrigger("Fall");
            fall = true;
            sC.currentSpeed = 0;
        }

        airScore = 0;
        //Times turned = trick
    }
    #endregion

    public void WhatJump()
    {
        //Decides what type of jump the player has to do Ollie/Nollie

        //if grabtrickbutton clicked then do footplant
    }
    
    #region =====Grinding=====
    public void GrindType()
    {
        if (flipTricking)
        {
            darkSlide = true;
        }
        else
        {
            darkSlide = false;
        }

        float grindAngle = Vector3.Angle(sC.orientation.transform.forward, transform.forward) * Mathf.Rad2Deg; //Change to the forward of the rail
        if (grindAngle >= 0 && grindAngle < 30)
        {
            if (sC.verticalInput < 0)
            {

            }
            else if (sC.verticalInput > 0)
            {

            }
            else
            {

            }
            //grindType = 50 50
        }
        else if (grindAngle >= 30 && grindAngle < 150)
        {
            //grindType = 
        }
        else if (grindAngle >= 150 && grindAngle < 210)
        {
            //grindType = 
        }
        else if (grindAngle >= 210 && grindAngle < 270)
        {

        }
        else
        {
            //grindType = 50 50
        }
        //Compare the orientation forwrd to the direction of the grind to know what type of grind
    }
    #endregion

    #region =====FALLING AND GETTING UP=====
    public void FallManager()
    {
        if (sC.grounded && flipTricking || sC.grounded && grabTricking)
        {
            fall = true;
            anim.SetTrigger("Fall");
        }

        if (fall)
        {
            sC.maxSpeed = 0;
            scoreM.combo = 0;
        }
    }

    public void EndTrick()
    {
        if(!sC.grounded)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.t_Finish, transform.position);
            AddPoints(currentTrick.scoreAwarded);
        }

        flipTricking = false;
        grabTricking = false;
    }

    public void GetUp()
    {
        fall = false;
    }
    #endregion

    #region =====POINTS=====
    public void AddPoints(int scoreAdded)
    {
        StopCoroutine(StartComboCounter());
        StartCoroutine(StartComboCounter());
        scoreM.combo++;
        if (scoreM.combo > 0) { scoreM.curretnScore += (scoreAdded * scoreM.combo); }
        else { scoreM.curretnScore += scoreAdded; }
    }

    public IEnumerator StartComboCounter()
    {
        yield return new WaitForSeconds(comboTimer);
        scoreM.combo = 0;
    }
    #endregion

    #region =====GRAB BOOSTS=====
    public void TailGrabBoostTimer()
    {
        if (tailgrabTimer < smallBoostTime)
        {
            finalBoostSpeed = 0;
            finalBoostTimer = 0;
        }
        else if (tailgrabTimer >= smallBoostTime && tailgrabTimer < mediumBoostTime)
        {
            finalBoostSpeed = smallBoostSpeed;
            finalBoostTimer = smallBoostTime;
        }
        else if (tailgrabTimer >= mediumBoostTime && tailgrabTimer < bigBoostTime)
        {
            finalBoostSpeed = mediumBoostSpeed;
            finalBoostTimer = mediumBoostTime;
        }
        else if (tailgrabTimer >= bigBoostTime)
        {
            finalBoostSpeed = bigBoostSpeed;
            finalBoostTimer = bigBoostTime;
        }
    }

    public void GrabBoost()
    {
        if (grabBoost)
        {
            ActivateSpeedEffects();
            boostTimer += Time.deltaTime;

            if (boostTimer <= finalBoostTimer) 
            {
                sC.rb.velocity = new Vector3(finalBoostSpeed, 0, finalBoostSpeed);
            }
            else
            {
                DeactivateSpeedEffects();
            }
        }
    }
    #endregion

    #region =====EFFECTS & VISUALS=====
    public void ActivateSpeedEffects()
    {
        boostSpeedLines.SetActive(true);
        _mB.active = true;
        _cA.intensity.value = chromaticAberrationIntensity;
        _doF.focalLength.value = doFFocusDistance;

        _virtualCBMP.m_AmplitudeGain = Mathf.Lerp(_virtualCBMP.m_AmplitudeGain, 1, Time.deltaTime);
        _virtualCBMP.m_FrequencyGain = Mathf.Lerp(_virtualCBMP.m_FrequencyGain, 1, Time.deltaTime);
    }

    public void DeactivateSpeedEffects()
    {
        boostSpeedLines.SetActive(false);
        _mB.active = false;
        _cA.intensity.value = ogCAIntensity;
        _doF.focalLength.value = ogDoFDistance;

        _virtualCBMP.m_AmplitudeGain = 0;
        _virtualCBMP.m_FrequencyGain = 0;
        //_virtualCBMP.m_AmplitudeGain = Mathf.Lerp(_virtualCBMP.m_AmplitudeGain, 0, Time.deltaTime);
        //_virtualCBMP.m_FrequencyGain = Mathf.Lerp(_virtualCBMP.m_FrequencyGain, 0, Time.deltaTime);
    }

    public void SkateSpeedEffects()
    {
        if (sC.currentSpeed >= minSpeedForEffect)
        {
            ActivateSpeedEffects();
        }
        else
        {
            DeactivateSpeedEffects();
        }
    }
    #endregion
}
