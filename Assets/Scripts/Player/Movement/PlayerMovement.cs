using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Movement
{
    public float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;

    [Header("==========Movement==========")]    
    Vector3 moveDirection;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float airMinSpeed;
    protected Vector3 velocity;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("==========Jumping==========")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;   

    [Header("==========Crouching==========")]
    public float crouchSpeed;   

    [Header("==========References==========")]
    public splineTesting sT;
    public MovementState state;

    new public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        air
    }

    public bool sprinting;
    public bool sliding;
    public bool crouching;
    public bool wallrunning;
    public bool freeze;
    public bool activeGrapple;

    private void Start()
    {
        
        sT = GetComponent<splineTesting>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        player = this.gameObject;
        readyToJump = true;
    }

    public override void StateHandler()
    {
        //  Reseting capsule size
        if(!crouching && !sliding)
        {
            GetComponent<CapsuleCollider>().height = 1.76f;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.86f, 0);
        }
            
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.freeze;
            rb.velocity = Vector3.zero;
            moveSpeed = 0f;
            rb.drag = 4;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
            rb.drag = 4;
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;            
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);

            // increase speed by one every second
            if (onDownSlope)
            {
                moveSpeed = slideSpeed;
            }

            else
                moveSpeed = slideSpeed;
            rb.drag = 4;
        }

        // Mode - Crouching
        else if (crouching && !sliding)
        {
            state = MovementState.crouching;
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.5f, 0);
            moveSpeed = crouchSpeed;
            rb.drag = 4;
        }

        // Mode - Sprinting
        else if (grounded && sprinting)
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
            rb.drag = 4;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
            rb.drag = 4;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                moveSpeed = airMinSpeed;

            rb.drag = 0;
        }
        anim.SetBool("Sliding", sliding);
        anim.SetBool("Crouch", crouching);
    }

    public override void Move()
    {
        if (activeGrapple || wallrunning) return;

        // calculate movement direction
        moveDirection = orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput;

        // on slope
        if (onUpSlope || onDownSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * forwardAcceleration);
        

        // in air
        else if (!grounded)
            currentSpeed = Mathf.Lerp(currentSpeed, moveSpeed, Time.deltaTime * forwardAcceleration);

        if (grounded)
        {
           velocity = (orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput).normalized * currentSpeed;
            rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); //Changed the movement mechanics from force based to velocity based
        }

        if(!grounded)
        {
            rb.velocity += ((horizontalInput * orientation.transform.right * Time.deltaTime + verticalInput * orientation.transform.forward * Time.deltaTime) * 20);
            if(rb.velocity.magnitude > airMaxSpeed)
            {
                rb.velocity = rb.velocity.normalized * airMaxSpeed;
            }
        }
 
        anim.SetFloat("XSpeed", rb.velocity.magnitude);
    }    

    public override void Jump(Vector3 jumpVector)
    {
        readyToJump = false;
        anim.SetTrigger("Jump");        
        // Jump Velocity
        rb.velocity = new Vector3(jumpVector.x, jumpVector.y + 1 * jumpForce, jumpVector.z);

        Invoke(nameof(ResetJump), jumpCooldown);
    }
    public void ResetJump()
    {
        readyToJump = true;
    }

    //Aplicarlo al movimiento de Slope
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {       
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public override void SpeedControl()
    {

    }
}

