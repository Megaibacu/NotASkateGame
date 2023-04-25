using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Movement
{
    public float moveSpeed;
    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    public PlayerInput _playerInput;

    [Header("==========Movement==========")]
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallrunSpeed;
    public float airMinSpeed;

    public float speedIncreaseMultiplier;
    public float slopeIncreaseMultiplier;

    public float groundDrag;

    [Header("==========Jumping==========")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;
    public float coyoteTime;
    public bool coyote;
    public bool coyoteDone;
    public bool sprinting;

    [Header("==========Crouching==========")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("==========Ground Check==========")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

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
        _playerInput = GetComponent<PlayerInput>();
        player = this.gameObject;

        readyToJump = true;

        startYScale = transform.localScale.y;
        newmanager = newaudiomanager.GetComponent<NewAudioManager>();
    }

    public void Update()
    {
        StateHandler();
        // ground check
        grounded = Grounded() ;
        anim.SetBool("Grounded", grounded);
        if (!grounded && !coyoteDone)
        {
            coyoteDone = true;
            coyote = true;
        }
            if (grounded) coyoteDone = false;
       
        MyInput();
        SpeedControl();

        //  animator
        anim.SetFloat("XSpeed", rb.velocity.magnitude);
        anim.SetBool("Sliding", sliding);

        // handle drag
        if ((state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching) && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    public void FixedUpdate()
    {
       MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = _playerInput.actions["Sideways"].ReadValue<float>();
        verticalInput = _playerInput.actions["Forward"].ReadValue<float>(); 

        // when to jump
        
        if (_playerInput.actions["Jump"].IsPressed() && ((readyToJump && grounded)|| coyote && readyToJump) && paused == false)
        {                                    
            
                readyToJump = false;

                Jump();
                newmanager.PlaySound("Jump");
                anim.SetTrigger("Jump");

                Invoke(nameof(ResetJump), jumpCooldown);
            
        }

        if (_playerInput.actions["Sprint"].WasPressedThisFrame())
        {
            sprinting = !sprinting;
        }

        // start crouch
        if (_playerInput.actions["Crouch"].WasPressedThisFrame() && !sliding && grounded)
        {
            anim.SetBool("Crouch", true);

            crouching = true;
        }

        // stop crouch
        if (!_playerInput.actions["Crouch"].IsPressed() && !Physics.Raycast(orientation.transform.position + Vector3.down * 0.3f, Vector3.up, playerHeight * 0.5f + 0.1f, whatIsGround))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);

            crouching = false;

            anim.SetBool("Crouch", false);
        }
       
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
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            moveSpeed = wallrunSpeed;
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.35f, 0);

            // increase speed by one every second
            if (onDownSlope)
            {
                moveSpeed = slideSpeed;
            }

            else
                moveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.35f, 0);
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && sprinting)
        {
            Debug.Log("Sprint");
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                moveSpeed = airMinSpeed;
        }
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

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

        Vector3 velocity = (orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput).normalized * currentSpeed; //This part of the script is only meant to change the forward movement of the player so it should only change the forward vector (local)
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z); //Changed the movement mechanics from force based to velocity based
    }    

    public override void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 1f * jumpForce, rb.velocity.z);

    }
    public void ResetJump()
    {
        readyToJump = true;
    }

    public IEnumerator DeactivateCoyote()
    {
        yield return new WaitForSeconds(coyoteTime);
            coyote = false;
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
    }

    //Aplicarlo al movimiento de Slope
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {       
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    private Vector3 velocityToSet;
    private void SetVelocity()
    {
        
        rb.velocity = velocityToSet;

    }

}

