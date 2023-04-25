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
    public float localGravity; //Used for the down force when in air

    private float reverseTimer;
    private bool turning;

    [Header("===============Accelerations===============")]

    public float forwardAcceleration; //Acceleration when going forward
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

    [Header("---------Drifting----------")]
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
    
    splineTesting sT;
    public PlayerInput _playerInput;

    [Header("===========Sound=========")]
    public GameObject newaudiomanager;
    NewAudioManager newmanager;

    AudioSource source;
    
    public bool isplaying;

    private void Awake()
    {
        ogMaxSpeed = maxSpeed; //Gets the maxspeed at the beginning to know what value we put in inspector. Usefull for boosts to go back to the desired maxspeed
    }

    private void Start()
    {
        sT = GetComponent<splineTesting>();
        newmanager = newaudiomanager.GetComponent<NewAudioManager>();
        source = newaudiomanager.GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        canMove = true; //Makes sure that the player can move when they put the skate on
    }

    public override void Update()
    {
        base.Update();
        if (touchingGround)
        {
            Movement();
            Jump();
            Steering();
            Boost();
        }
        else if (!touchingGround)
        {
            AirMovement();
        }
        AirDetection();

        SoundCheck();
    }
    
    public void Movement()
    {
        playerDirection = transform.forward * _playerInput.actions["Forward"].ReadValue<float>() + transform.right * _playerInput.actions["Sideways"].ReadValue<float>();

        if (_playerInput.actions["Forward"].ReadValue<float>() > 0 && canMove)
        {
            //The player accelerates forward when the forward input is performed
            //Use of lerp to make the velocity change prograsively not at an instant because it would not be realistic enough
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * forwardAcceleration);
            steerMultiplier = Mathf.Lerp(steerMultiplier, minSteering, forwardAcceleration * Time.deltaTime);
            reverseTimer = 0;
            
            //so that the skate sounds higher as you accelerate
            if (source.volume < 1) //If thre player isn't facing a slope with a high angle they can move forward
            {
                source.volume += Time.deltaTime;
            }
                        
                //The player accelerates forward when the forward input is performed
                //Use of lerp to make the velocity change prograsively not at an instant because it would not be realistic enough
                currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, Time.deltaTime * forwardAcceleration);
                steerMultiplier = Mathf.Lerp(steerMultiplier, minSteering, forwardAcceleration * Time.deltaTime);
                reverseTimer = 0;
                //so that the skate sounds higher as you accelerate
                if (source.volume < 1)
                {
                    source.volume += Time.deltaTime;
                }
        }
        
        else if (_playerInput.actions["Forward"].ReadValue<float>() < 0)
        {
            //Instead of going backwards changing to goofy
            //Player needs to have a slower reverse speed and acceleration to simulate the real world in a minimum way

            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * reverseAcceleration);
            steerMultiplier = Mathf.Lerp(steerMultiplier, maxSteering, reverseAcceleration * Time.deltaTime);
        }
        else
        {
            //Breaking needs to be discussed. Whaqt should be the timing of deceleration?
            currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * breaking);
            steerMultiplier = Mathf.Lerp(steerMultiplier, maxSteering, breaking * Time.deltaTime);
            reverseTimer = 0;
            //make the skate sound lower as you brake
            if (source.volume > 0)
            {
                source.volume -= Time.deltaTime;
            }
        }

        //Change of velocity in the local forward
        Vector3 velocity = orientation.transform.forward * currentSpeed; //This part of the script is only meant to change the forward movement of the player so it should only change the forward vector (local)
        rb.velocity = velocity; //Changed the movement mechanics from force based to velocity based
     
    }

    //====================Steering====================

    public void Steering()
    {
        steerDirection = _playerInput.actions["Sideways"].ReadValue<float>();
        Vector3 steerVect; //Used to get the final rotation of the object
        float steerAmount;
        
        //----------Final Steering Direction---------
        steerAmount = steerDirection * steerMultiplier;
        steerVect = new Vector3(orientation.transform.eulerAngles.x, orientation.transform.eulerAngles.y + steerAmount, orientation.transform.eulerAngles.z);
        orientation.transform.eulerAngles = Vector3.Lerp(orientation.transform.eulerAngles, steerVect, steerTiming* Time.deltaTime);

        //===============Turning 180===============

        if (_playerInput.actions["Turn"].WasPressedThisFrame() && currentSpeed <= 0.5f)  
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        }
    }

 

    public void Boost()
    {
        boostTime -= Time.deltaTime;

        if(boostTime > 0) //If the player has a boost
        {
            //Play animation for boosts
            maxSpeed = boostSpeed;
            currentSpeed = Mathf.Lerp(currentSpeed, maxSpeed, 1 * Time.deltaTime);
        }
        else
        {
            maxSpeed = ogMaxSpeed;
        }
    }

    public override void Jump()
    {
        if (_playerInput.actions["Jump"].WasPressedThisFrame() && grinding)
        {
            sT.EndGrind();
        }
        if (_playerInput.actions["Jump"].IsPressed() && !grinding)
        {
            if (skateJumpPreassure < maxSkateJumpFoce) { skateJumpPreassure += Time.deltaTime * skateJumpMultiplier; }
            else { skateJumpPreassure = maxSkateJumpFoce; }
        }
        if (_playerInput.actions["Jump"].WasReleasedThisFrame() && !grinding)
        {
            skateJumpPreassure += skateJumpPreassure + minSkateJumpFoce;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            newmanager.PlaySound("Skate Jump");

            rb.AddForce(Vector3.up * skateJumpPreassure, ForceMode.Impulse);
            skateJumpPreassure = 0;
        }
    }
    
    //====================Air Movement====================

    public void AirMovement()
    {
        //Function serves for players to always have their skate facing the floor
        //This makes the skating easier and more arcade as they don't have to worry about landing mechanics like in Skate 3
        //It also makes game design and programming easier as we don't have to worry about creating those landing mechanics with a complex system
        
        //IMPORTANT!!
        //WE SHOULD TALK ABOUT THINGS LIKE HIGH RAMPS AND HOW TO FACE THAT PROBLEM

        //Function to keep the momentum of the player
              

        if (!touchingGround) //Will only work if the player is in the air
        {
            RaycastHit hit;
            if (Physics.Raycast(orientation.transform.position, -Vector3.up, out hit, 100f))
            {
                orientation.transform.rotation = Quaternion.Lerp(orientation.transform.rotation, Quaternion.FromToRotation(orientation.transform.up * 2, hit.normal) * orientation.transform.rotation, 3f * Time.deltaTime);

            }

            //NEED TO CHANGE THE REAL ROTATION FOR A SIMPLE MESH ROTATION IN ORDER TO GET THE FORWARD OF THE GAMEOBJECT AND COMPARE IT TO THE FORWARD OF THE ROTATING MESH TO CHECK IF AT THE MOMENT OF LANDING YOU SHOULD FALL
            steerDirection = _playerInput.actions["Sideways"].ReadValue<float>();

            if (steerDirection != 0)
            {
                Vector3 airRot = new Vector3(orientation.transform.eulerAngles.x, orientation.transform.eulerAngles.y + airSteerMultiplier * steerDirection, orientation.transform.eulerAngles.z);
                orientation.transform.eulerAngles = Vector3.Lerp(orientation.transform.eulerAngles, airRot, airSteerTiming * Time.deltaTime);
            }

            currentSpeed = Mathf.Lerp(currentSpeed, 0, .25f * Time.deltaTime);
            Vector3 airSpeed = orientation.transform.forward * currentSpeed;
            airSpeed.y = rb.velocity.y;
            rb.velocity = airSpeed;
        }
    }

    //====================Air Detection====================

    public void AirDetection()
    {

        //==========Change of contraints and gravity depending if on air==========
        //If the player is in air they can do backflips now and gravity is added
        //The shouldn't be gravity while the player is not in the air because they will lose speed in slopes

        if (!touchingGround)
        {
            rb.AddForce(Vector3.up * localGravity, ForceMode.Force);
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    //==============================Setters==============================
    public void SetCurrentSpeed(float amount)
    {
        currentSpeed = amount;
    }

    public void SoundCheck()
    {
        //mira si tiene que sonar el sonido del skate rodando y a que volumen
        //acordarse que para tocar el volumen mientras que se juega hay que manipular el source que se crea, no el de la escena
        if (currentSpeed > 0.4f && touchingGround == true)
        {
            if (isplaying == false)
            {
                newmanager.PlaySound("Skating");
                isplaying = true;
            }
            //este es el sonido que suena cuando te mueves con el skate, se loopea hasta que te paras
        }
        else
        {
            newmanager.StopSound();
            isplaying = false;
        }
    }
}
