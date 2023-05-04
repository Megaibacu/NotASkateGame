using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    [HideInInspector] public GameObject orientation; //The way the player model is facing
    [HideInInspector] public GameObject player;

    //[Header("===============References===============")]
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;


    //[Header("===============Movement===============")]
    public float horizontalInput;
    public float verticalInput;
    protected RaycastHit hit;
    [HideInInspector] public Vector3 currentMomentum;
     public float maxSpeed;
    [HideInInspector] public float ogMaxSpeed;
    [HideInInspector] public float currentSpeed;
    [HideInInspector] public bool fall; //If the player touches the ground and is doing a tricks then they fall
    public float localGravity; //Used for the down force when in air
    [HideInInspector] public float forwardAcceleration; //Acceleration when going forward
    [HideInInspector] public bool readyToJump;
    public float coyoteTime;
    public bool coyote;
    public bool coyoteDone;
    public bool grounded;

    [Header("===============Slope===============")]
    public float slopeAcceleration;
    public float maxDegreeAngle; //the max angle that where the player can move forward with the skate
    public bool onUpSlope; //When on slope if the slope is facing upwards
    public bool onDownSlope; //When on slope if the slope is facing downwards
    public bool canMove; //if the slope is too bug then the player will not be able to move forwrd
    public Transform slopeChecker; //Transform that is a unit forward in order to check if there is a slope in time for the player to break
    public RaycastHit slopeHit;
    public RaycastHit forwardHit;

    [Header("===========Sound=========")]
    public GameObject newaudiomanager;
    public NewAudioManager newmanager;

    //pausing
    public bool paused;


    private void Update()
    {
        SlopeDetection();
        GroundRotation();
    }

    public enum MovementState
    {


    }
    public virtual void Move()
    {

    }
    public virtual void Jump()
    {

    }
    public void CoyoteCheck()
    {
        if (!grounded && !coyoteDone)
        {
            coyoteDone = true;
            coyote = true;
            StartCoroutine(DeactivateCoyote());
        }
        if (grounded) coyoteDone = false;
    }
    public IEnumerator DeactivateCoyote()
    {
        yield return new WaitForSeconds(coyoteTime);
        coyote = false;
    }
    public virtual void JumpRelease()
    {

    }
    public virtual void StateHandler()
    {

    }
    public virtual void SoundCheck() { }

    public void GroundRotation()
    {
        if (grounded) //Checks if the player is hitting the ground by shooting a raycast downwards
        {
            if (canMove) //Only rotates to face the ground if the slope in front is less than the maximum
            {
                orientation.transform.rotation = Quaternion.Lerp(orientation.transform.rotation, Quaternion.FromToRotation(orientation.transform.up * 2, hit.normal) * orientation.transform.rotation, 7.5f * Time.deltaTime); //Rotates the player to not face the ground
            }           
            anim.SetBool("Air", false);
        }
        else
        {
            currentMomentum = rb.velocity;
            anim.SetBool("Air", true);
        }
    }
    public void Grounded() 
    {       
        Debug.DrawRay(player.transform.position, -orientation.transform.up, Color.green, .05f);
        grounded = Physics.Raycast(player.transform.position, -orientation.transform.up, out hit, .05f);
        anim.SetBool("Grounded", grounded);

    }

    public void SpeedControl()
    {

        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // limit velocity if needed
        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        anim.SetFloat("XSpeed", flatVel.magnitude);
    }

    public void AirDetection()
    {

        //==========Change of contraints and gravity depending if on air==========
        //If the player is in air they can do backflips now and gravity is added
        //The shouldn't be gravity while the player is not in the air because they will lose speed in slopes

        if (!grounded)
        {
            rb.AddForce(Vector3.up * localGravity, ForceMode.Force);
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public virtual void AirMovement()
    {

    }

    //====================Slope Behavior====================
    public void SlopeDetection()
    {
        //Cheks the slope when IN a slope
        if (Physics.Raycast(orientation.transform.position, -orientation.transform.up, out slopeHit, 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            if (angle > 0)
            {
                onUpSlope = true;
                onDownSlope = false;
            }
            else if (angle < 0)
            {
                onUpSlope = false;
                onDownSlope = true;
            }
            else
            {
                onUpSlope = false;
                onDownSlope = false;
            }


        }

        //Checks the slope when FACING a slope
        if (Physics.Raycast(slopeChecker.position, -slopeChecker.up, out forwardHit, .5f))  //Creates a raycast going from the skate to the floor and gets the object hit
        {
            float angle = Vector3.Angle(Vector3.up, forwardHit.normal); //Gets the angle between the skate and the floor

            if (angle <= maxDegreeAngle) //If the angle in which the player is facing and the slope is less than the maximum angle you can skate on then you can go forward
            {
                canMove = true;
                SlopeMovement(angle);
            }
            else if (angle > maxDegreeAngle) //You cannot move forward
            {
                canMove = false;
                currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * 100);
            }
        }
    }

    public void SlopeMovement(float slopeAngle)
    {
        float newMaxSpeed = maxSpeed;
        if (onUpSlope && !onDownSlope)
        {
            if (slopeAngle > 0)
            {
                newMaxSpeed = Mathf.Abs(ogMaxSpeed / (slopeAngle / 10));
            }

            maxSpeed = newMaxSpeed; //Change once we know this is functional
            //maxSpeed = Mathf.Lerp(maxSpeed, newMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
        else if (onDownSlope && !onUpSlope)
        {
            newMaxSpeed = Mathf.Abs(ogMaxSpeed * (slopeAngle / 10));
            maxSpeed = newMaxSpeed;
            //maxSpeed = Mathf.Lerp(maxSpeed, newMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
        else if (!onDownSlope && !onUpSlope)
        {
            maxSpeed = ogMaxSpeed;
            //maxSpeed = Mathf.Lerp(maxSpeed, ogMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
    }
}
