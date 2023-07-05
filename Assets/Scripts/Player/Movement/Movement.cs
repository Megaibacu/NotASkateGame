using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public abstract class Movement : MonoBehaviour
{
    [HideInInspector] public GameObject orientation; //The way the player model is facing
    [HideInInspector] public GameObject player;

    //[Header("===============References===============")]
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
     public LayerMask floor;


    [Header("===============Movement===============")]
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
    public float airMaxSpeed;

    [Header("===============Slope===============")]
    public float slopeAcceleration;
    public float maxDegreeAngle; //the max angle that where the player can move forward with the skate
    public float minDegreeAngle; //the minimum angle for the velocity to change in a slope
    public bool onUpSlope; //When on slope if the slope is facing upwards
    public bool onDownSlope; //When on slope if the slope is facing downwards
    public bool canMove; //if the slope is too bug then the player will not be able to move forwrd
    public Transform slopeChecker; //Transform that is a unit forward in order to check if there is a slope in time for the player to break
    public float slopeAngle; //Maybe this should be measured with the orientation
    public RaycastHit slopeHit;
    public RaycastHit forwardHit;

    //pausing
    public bool paused;
    public enum MovementState
    {


    }
    public virtual void Move()
    {

    }
    public virtual void Jump(Vector3 grapplePoint)
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
        grounded = Physics.Raycast(player.transform.position, -orientation.transform.up, out hit, .1f, floor);
        anim.SetBool("Grounded", grounded);

        if(grounded)
        {
            switch (hit.transform.tag)
            {
                case "F_Concrete":
                    FMODEvents.instance.footStep.setParameterByName("Surface", 0);
                    break;
                case "F_Wood":
                    FMODEvents.instance.footStep.setParameterByName("Surface", 1);
                    break;
                default:
                    break;
            }
        }
       
    }

    public virtual void SpeedControl()
    {       
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // limit velocity if needed
        if (flatVel.magnitude > currentSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        
    }

    public void AirDetection()
    {

        //==========Change of contraints and gravity depending if on air==========
        //If the player is in air they can do backflips now and gravity is added
        //The shouldn't be gravity while the player is not in the air because they will lose speed in slopes

        if (!grounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y + localGravity * 6 * Time.deltaTime, rb.velocity.z);
            //rb.AddForce(Vector3.up * localGravity, ForceMode.Force);
        }

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    public virtual void AirMovement()
    {

    }

    //====================Slope Behavior====================

    public bool OnSlope()
    {
        if (Physics.Raycast(orientation.transform.position, -orientation.transform.up, out slopeHit, 0.5f))
        {
            slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return slopeAngle != 0 && slopeAngle > minDegreeAngle;
        }
        else { return false; }
    }
}
