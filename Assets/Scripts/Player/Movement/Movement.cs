using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Movement : MonoBehaviour
{
    public GameObject orientation; //The way the player model is facing

    [Header("===============References===============")]
    public Animator anim;
    public Rigidbody rb;

    [Header("===============States===============")]
    public bool grinding; //Checks if the player is grinding
    public bool isSliding, touchingGround; //Is sliding means that the player si drifting. Touching ground is a basic air checker

    [Header("===============Movement===============")]
    public Vector3 currentMomentum;
    public float maxSpeed;
    public float ogMaxSpeed;
    public float currentSpeed;
    [HideInInspector] public bool fall; //If the player touches the ground and is doing a tricks then they fall

    [Header("===============Slope===============")]
    public float slopeAcceleration;
    public float maxDegreeAngle; //the max angle that where the player can move forward with the skate
    public bool onUpSlope; //When on slope if the slope is facing upwards
    public bool onDownSlope; //When on slope if the slope is facing downwards
    public bool canMove; //if the slope is too bug then the player will not be able to move forwrd
    public Transform slopeChecker; //Transform that is a unit forward in order to check if there is a slope in time for the player to break

    public enum MovementState
    {


    }
    public virtual void Update()
    {
        SlopeDetection();
        StateHandler();
        GroundRotation();
    }
    public virtual void FixedUpdate()
    {
        
    }
    public virtual void Move()
    {

    }
    public virtual void Jump()
    {

    }
    public virtual void StateHandler()
    {

    }
    public void GroundRotation()
    {
        //Function serves to rotate the car in the X axis when in a slope
        RaycastHit hit;
        touchingGround = Physics.Raycast(orientation.transform.position, -orientation.transform.up, out hit, .1f);
        if (touchingGround) //Checks if the player is hitting the ground by shooting a raycast downwards
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
            touchingGround = false;
            anim.SetBool("Air", true);
        }
    }
    public bool Grounded() 
    {
        RaycastHit hit;
        return Physics.Raycast(orientation.transform.position, -orientation.transform.up, out hit, .1f);
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

    }

    //=======================================================
    //====================Slope Behavior====================
    //=======================================================
    public void SlopeDetection()
    {
        RaycastHit slopeHit;
        RaycastHit forwardHit;

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
    public void SlopeMovement(float angle)
    {
        float newMaxSpeed = Mathf.Abs(ogMaxSpeed / (angle * 10));
        if (onUpSlope && !onDownSlope)
        {
            maxSpeed = Mathf.Lerp(maxSpeed, newMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
        else if (onDownSlope && !onUpSlope)
        {
            maxSpeed = Mathf.Lerp(maxSpeed, newMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
        else if (!onDownSlope && !onUpSlope)
        {
            maxSpeed = Mathf.Lerp(maxSpeed, ogMaxSpeed, Time.deltaTime * slopeAcceleration);
        }
    }
}
