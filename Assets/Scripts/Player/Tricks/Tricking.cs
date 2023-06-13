using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tricking : MonoBehaviour
{
    public SkateController sC;
    ScoreManager scoreM;
    private Animator anim;
    private Rigidbody rB;

    [Header("===============Skate Tricks===============")]
    public SkateTricks[] tricks; //All the tricks that can be perfornmed by the player. In the future it would be cool to make this customizable
    public SkateTricks[] grabTricks;
    public float comboTimer; // The time that the player has to do another trick to continue the combo
    SkateTricks currentTrick;
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


    // Start is called before the first frame update
    void Start()
    {
        sC = GetComponent<SkateController>();
        scoreM = GetComponent<ScoreManager>();
        rB = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SkateTricks();
        GrabBoost();

        if (sC.grounded)
		{
            sC.momentum = rB.velocity;
            EndTrick();
        }
    }

    public void SkateTricks()
    {
        if (!sC.grounded && !flipTricking && !grabTricking)
        {
            
            if (Input.GetButtonDown("FlipTricks"))
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

        if (!sC.grounded && !flipTricking)
        {
            if (Input.GetButton("GrabTricks"))
            {
                tailgrabTimer += Time.deltaTime;
                grabTricking = true;
                scoreM.curretnScore += (int)(grabTricks[0].scoreAwarded * Time.deltaTime);
                TailGrabBoostTimer();
                anim.SetBool("GrabTricking", true);
                grabBoost = false;
            }
            if (Input.GetButtonUp("GrabTricks"))
            {
                boostTimer = 0;
                grabBoost = true;
                Vector3 boostVel = sC.orientation.transform.forward * finalBoostSpeed;
                sC.rb.velocity = new Vector3(boostVel.x, sC.rb.velocity.y, boostVel.z);
                //sC.rb.AddForce(sC.orientation.transform.forward * finalBoostSpeed, ForceMode.Impulse);
                finalBoostSpeed = 0;
                tailgrabTimer = 0;
                grabTricking = false;
                anim.SetBool("GrabTricking", false);
            }
            
        }
        

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
        else 
        { 
            sC.maxSpeed = sC.ogMaxSpeed; 
        }
    }
    public void EndTrick()
    {
        if(!sC.grounded)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.t_Finish, transform.position);
            AddPoints();
        }

        flipTricking = false;
        grabTricking = false;
    }

    public void AddPoints()
    {
        StopCoroutine(StartComboCounter());
        StartCoroutine(StartComboCounter());
        scoreM.combo++;
        if (scoreM.combo > 0) { scoreM.curretnScore += (currentTrick.scoreAwarded * scoreM.combo); }
        else { scoreM.curretnScore += currentTrick.scoreAwarded; }
    }

    public IEnumerator StartComboCounter()
    {
        yield return new WaitForSeconds(comboTimer);
        scoreM.combo = 0;
    }

    public void GetUp()
    {
        fall = false;
    }

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
            boostTimer += Time.deltaTime;

            if (boostTimer <= finalBoostTimer) 
            {
                sC.rb.velocity = new Vector3(finalBoostSpeed, 0, finalBoostSpeed);
            }
        }
    }
}
