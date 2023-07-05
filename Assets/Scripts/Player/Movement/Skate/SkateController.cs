using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkateController : Movement
{
    [Header("===============States===============")]
    public bool grinding; //Checks if the player is grinding

    new public enum MovementState
    {
        Skating,
        Drifting,
        Grinding,
        Tricking,
    }

    [Header("===============Movement===============")]
    public float maxReverseSpeed;
    public float timeToReverse; //The time multiplier to make the skate reevert smooth
    public float boostSpeed; //Speed for boosts like when changing from parkour to skate or when drifting
    private float realSpeed; //Checks the real velocity of the rb
    public float reverseSpeedDecrease; //Number that is used to divide the speed so that when you go backwards you go slower
    public float timeToTurn; //Marks the time that the player has to hold the deceleration button tu turn 180 in the floor
    public float slopeMovementSpeed;
    public float maxBallSpeed; //When the skater grabs their board in the floor to gain more velocity
    private bool goofy; //Change the player's stance

    private float reverseTimer;
    private bool turning;
    private bool directionalBreaking;

    SlopeDirection slopeDir;

    [Header("===============Accelerations===============")]
    public float reverseAcceleration; //Acceleration for full stopping. Makes breaking forcefully smooth
    public float breaking; //Deceleration when not holding a key
    private float boostTime; //How long does the player have a boost from drift or trick combos
    private Vector3 playerDirection;
    public Vector3 momentum;

    [Header("===============Steering===============")]
    public float steerMultiplier; //Multiplier that changes the amount of steering
    public float minSteering; //The minimum amount of steering the player can do
    public float maxSteering; //The maximum amount of steering the player can do
    private float steerDirection; // 1, 0 or -1 depending on the player's input
    public float steerTiming; //Time multiplier to make steering smooth
    public float airSteerMultiplier; //Change of multiplier in the air for 180s and 360s
    public float airSteerTiming; //Same thing but with the smoothing
    private float curretnRotate;
    private float rotate;

    [Header("===============Drifting===============")]
    private bool driftLeft, driftRight; //Checks if the player is drifting left or right
    public float outwardsDriftForce; //The force that pushes the player when drifting
    public float minSpeedToDrift; //The minimum speed that the player has to go when drifitng
    public float driftTime; //A timer that measures how much time the player has been drifting to check the boost
    public float smallBoostTime, mediumBoostTime, largeBoostTime; //Boost amount in time that multiplies the max velocity
    public float smallBoostAmount, mediumBoostAmount, largeBoostAmount; //Velocity multiplier when performing a big enough drift
    public GameObject smallDrift, mediumDrift, largeDrift; //Particles systems

    [Header("===============Skate Jump===============")]

    public float skateJumpPreassure; //The force used on the obj when jumping
    public float minSkateJumpFoce; //The minimum force that will be perfomred even if the player just presses the jump for one frame
    public float maxSkateJumpFoce; //Max force. Cannot jump higher
    public float skateJumpMultiplier; //Final jump force
    public float jumpMaxVelocity;
    public float maxDuckedVelocity;
    
    splineTesting sT;
    Tricking trks;

    [Header("===============Skate Turn===============")]
    public float timeTurning;
    public float turningMultiplier;
    public bool canTurn = true;
    public Transform playerBody;
    public Vector3 turnDir;
    Vector3 prevOrientationRotation;

    [Header("===============Air Movement===============")]
    public float trickRotationSpeed;
    public float minTrickRotationSpeed;
    public float maxTrickRotationSpeed;
    public float trickRotationAcceleration;
    public Transform groundOrientation;
    float groundTimer;
    float ogLocalGravity;

    //AudioSource source;


    public bool isplaying;

    private void Awake()
    {
        ogMaxSpeed = maxSpeed; //Gets the maxspeed at the beginning to know what value we put in inspector. Usefull for boosts to go back to the desired maxspeed
    }

    private void Start()
    {
        readyToJump = true;
        canTurn = true;
        sT = GetComponent<splineTesting>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        trks = GetComponent<Tricking>();
        slopeDir = GetComponent<SlopeDirection>();
        canMove = true; //Makes sure that the player can move when they put the skate on
        player = this.gameObject;
        ogLocalGravity = localGravity;
        groundOrientation.forward = orientation.transform.forward;
    }
    
    #region =========SKATE Movement=========
    public override void Move()
    {
        #region -----Sound-----
        FMODEvents.instance.skateRolling.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED && rb.velocity != Vector3.zero && grounded)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.skateRolling);
        }
        else if(rb.velocity.magnitude < 1 || !grounded)
        {
            AudioManager.instance.StopSound(FMODEvents.instance.skateRolling);
        }
        #endregion

        #region -----Movement & Steering-----

        //Players input
        playerDirection = orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput;

        //==========PLAYER CAN MOVE==========
        if (canMove)
        {
            #region -----Forward Movement-----
            if (verticalInput > 0)
            {
                //The player accelerates forward when the forward input is performed
                //Use of lerp to make the velocity change prograsively not at an instant because it would not be realistic enough
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed * verticalInput, Time.deltaTime * forwardAcceleration);
                steerMultiplier = Mathf.Lerp(steerMultiplier, minSteering, forwardAcceleration * Time.deltaTime);
                reverseTimer = 0;
                directionalBreaking = true;
            }
            #endregion

            #region -----Brakeing-----
            else if (verticalInput < 0)
            {
                //Instead of going backwards changing to goofy
                //Player needs to have a slower reverse speed and acceleration to simulate the real world in a minimum way
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * reverseAcceleration);
                steerMultiplier = Mathf.Lerp(steerMultiplier, maxSteering, reverseAcceleration * Time.deltaTime);
                reverseTimer += Time.deltaTime;

                if (reverseTimer >= timeToTurn && directionalBreaking)
                {
                    Turn();
                    directionalBreaking = false;
                }
            }
            else
            {
                //Breaking needs to be discussed. Whaqt should be the timing of deceleration?
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * breaking);
                steerMultiplier = Mathf.Lerp(steerMultiplier, maxSteering, breaking * Time.deltaTime);
                reverseTimer = 0;
                directionalBreaking = true;

            }
            #endregion

            //Change of velocity in the local forward
            Vector3 velocity = orientation.transform.forward * currentSpeed; //This part of the script is only meant to change the forward movement of the player so it should only change the forward vector (local)            
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); //Changed the movement mechanics from force based to velocity based
            

            //===================Steering============
            #region -----Steering-----
            steerDirection = horizontalInput;
            Vector3 steerVect; //Used to get the final rotation of the object
            float steerAmount;

            //----------Final Steering Direction---------
            steerAmount = steerDirection * steerMultiplier;
            steerVect = new Vector3(orientation.transform.eulerAngles.x, orientation.transform.eulerAngles.y + steerAmount, orientation.transform.eulerAngles.z);

            if (grounded)
            {
                orientation.transform.eulerAngles = Vector3.Lerp(orientation.transform.eulerAngles, steerVect, steerTiming * Time.deltaTime);
            }
            #endregion

        }
        else //==========AIR VELOCITY==========
        {
            if (currentSpeed > 1f)
            {
                currentSpeed = Mathf.Lerp(currentSpeed, (currentSpeed + maxSpeed / currentSpeed), forwardAcceleration * Time.deltaTime);
                Vector3 velocity = groundOrientation.forward * currentSpeed; //This part of the script is only meant to change the forward movement of the player so it should only change the forward vector (local)
                rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); //Changed the movement mechanics from force based to velocity based
            }
        }
        #endregion

        //I want to change this from a timer in the invoke to a variable condition. It wasn't working for some reason and I cannot waste more time with this, but in the future this will be changed to make it more stable
        #region -----Turning-----
        Debug.Log(turning);
        Vector3 actualRotation = orientation.transform.eulerAngles;
        if (turning)
        {
            if (!slopeDir.flatSurface) { orientation.transform.eulerAngles = turnDir; }
            else { orientation.transform.eulerAngles = Vector3.Lerp(orientation.transform.eulerAngles, turnDir, turningMultiplier * Time.deltaTime); }
        }
        else if (Mathf.Abs(actualRotation.y) == prevOrientationRotation.y + 180)
        {
            ResetTurn();
        }
        #endregion
    }
    #endregion

    #region ==========TURNING==========
    public void Turn()
    {
        turning = true;
        canTurn = false;
        
        turnDir = new Vector3(-orientation.transform.eulerAngles.x, orientation.transform.eulerAngles.y + 180, -orientation.transform.eulerAngles.z);

        prevOrientationRotation = orientation.transform.eulerAngles;
        if (turnDir.y > 360) { turnDir.y = turnDir.y - 360; }
        if (turnDir.y < 0) { turnDir.y = turnDir.y + 360; }
        Invoke(nameof(ResetTurn), timeTurning);
    }
    

    public void ResetTurn()
    {
        turning = false;
        canTurn = true;
    }
    #endregion

    #region =========SKATE JUMP=========
    public override void Jump(Vector3 gP)
    {
        if (grinding)
        {
            sT.EndGrind();
            trks.EndTrick();
            trks.GetUp();
        }
        if (!grinding)
        {
            maxSpeed = Mathf.Lerp(maxSpeed, maxDuckedVelocity, Time.deltaTime);
            if (skateJumpPreassure < maxSkateJumpFoce) 
            { 
                skateJumpPreassure += Time.deltaTime * skateJumpMultiplier;
                //if (OnSlope() && currentSpeed > maxSpeed / 2) { maxSpeed = Mathf.Lerp(maxSpeed, maxDuckedVelocity, Time.deltaTime); }
            }
            else { skateJumpPreassure = maxSkateJumpFoce; }
        }

    }

    public override void JumpRelease()
    {
        if (!grinding && skateJumpPreassure > minSkateJumpFoce)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.jumpSound, transform.position);
            AudioManager.instance.PlayOneShot(FMODEvents.instance.skateJump, transform.position);
            rb.velocity = new Vector3(rb.velocity.x, skateJumpPreassure, rb.velocity.z);
            skateJumpPreassure = 0;

            //if (slopeDir.upHill && currentSpeed < 3)
            //{
            //    Turn();
            //}
        }
        else
            skateJumpPreassure = 0;
    }
    #endregion

    #region ==========AIR MOVEMENT==========
    public override void AirMovement()
    {
        //Function serves for players to always have their skate facing the floor
        //This makes the skating easier and more arcade as they don't have to worry about landing mechanics like in Skate 3
        //It also makes game design and programming easier as we don't have to worry about creating those landing mechanics with a complex system

        //IMPORTANT!!
        //WE SHOULD TALK ABOUT THINGS LIKE HIGH RAMPS AND HOW TO FACE THAT PROBLEM

        //Function to keep the momentum of the player

        if (!grounded) //Will only work if the player is in the air
        {
            groundTimer = 0; //Resets the timer that detects the first frame of the player touching the ground
            anim.SetBool("Air", true); //Plays the animation for the playerbeing in the air
            RaycastHit hit; //A raycast detector that detects the ground
            if (Physics.Raycast(orientation.transform.position, -Vector3.up, out hit, 100f)) //Detects the floor
            {
                //Player will always face the floor when on air
                orientation.transform.rotation = Quaternion.Lerp(orientation.transform.rotation, Quaternion.FromToRotation(orientation.transform.up * 2, hit.normal) * orientation.transform.rotation, 3f * Time.deltaTime);
            }

            //NEED TO CHANGE THE REAL ROTATION FOR A SIMPLE MESH ROTATION IN ORDER TO GET THE FORWARD OF THE GAMEOBJECT AND COMPARE IT TO THE FORWARD OF THE ROTATING MESH TO CHECK IF AT THE MOMENT OF LANDING YOU SHOULD FALL
            steerDirection = horizontalInput;

            if (steerDirection != 0)
            {
                trickRotationSpeed = Mathf.Lerp(trickRotationSpeed, maxTrickRotationSpeed, trickRotationAcceleration * Time.deltaTime); //Speed of turn increases as the player turns more and more
                Vector3 airRot = new Vector3(orientation.transform.eulerAngles.x, orientation.transform.eulerAngles.y + trickRotationSpeed * steerDirection, orientation.transform.eulerAngles.z);
                orientation.transform.eulerAngles = Vector3.Lerp(orientation.transform.eulerAngles, airRot, airSteerTiming * Time.deltaTime);
            }
            canMove = false;
        }
        else
        {
            TouchGround();
            anim.SetBool("Air", false);
            canMove = true;
            localGravity = ogLocalGravity;

            if (!HalfPipes())
            {
                groundOrientation.forward = orientation.transform.forward;
            }
            else
            {
                groundOrientation.forward = Vector3.up;
            }

            trickRotationSpeed = minTrickRotationSpeed;
        }
    }  

    public void TouchGround()
    {
        if (groundTimer < Time.deltaTime)
        {
            trks.AirLanding();
        }

        groundTimer += Time.deltaTime;
    }
    #endregion

    public bool HalfPipes() //Used in the movement function
    {
        //if the player is in an object with the tag half pipe --> gravity = 0
        RaycastHit pipeHit;
        if (Physics.Raycast(orientation.transform.position, -orientation.transform.up, out pipeHit, 1.5f, floor))
        {
            if (hit.collider.CompareTag("PipeFloor"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else { return false; }
    }

    //==============================Setters & Getters==============================
    public void SetCurrentSpeed(float amount)
    {
        currentSpeed = amount;
    }

    public override void SoundCheck()
    {
        //mira si tiene que sonar el sonido del skate rodando y a que volumen
        //acordarse que para tocar el volumen mientras que se juega hay que manipular el source que se crea, no el de la escena
        if (currentSpeed > 0.4f && grounded == true)
        {
            if (isplaying == false)
            {
                isplaying = true;
            }
            //este es el sonido que suena cuando te mueves con el skate, se loopea hasta que te paras
        }
        else
        {
            isplaying = false;
        }

        
    }

    public void SwitchStance()
    {
        goofy = !goofy;
    }
}
