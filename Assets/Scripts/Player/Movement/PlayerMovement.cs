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

    [Header("==========Slope Handling==========")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("==========References==========")]
    public splineTesting sT;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;


    public MovementState state;

    //sound
    public GameObject newaudiomanager;
    NewAudioManager newmanager;
    //pausing
    public bool paused;

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

        readyToJump = true;

        startYScale = transform.localScale.y;
        newmanager = newaudiomanager.GetComponent<NewAudioManager>();
    }

    public override void Update()
    {

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        anim.SetBool("Grounded", grounded);
        if (!grounded && !coyoteDone)
        {
            coyoteDone = true;
            coyote = true;
        }
            if (grounded) coyoteDone = false;

        if(_playerInput.actions["Sprint"].WasPressedThisFrame())
        {
            sprinting = !sprinting;
        }
        MyInput();
        SpeedControl();
        StateHandler();
        //  animator
        anim.SetFloat("XSpeed", rb.velocity.magnitude);
        anim.SetBool("Sliding", sliding);

        // handle drag
        if ((state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching) && !activeGrapple)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    public override void FixedUpdate()
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

    bool keepMomentum;
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
            desiredMoveSpeed = 0f;
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallrunSpeed;
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.35f, 0);

            // increase speed by one every second
            if (OnSlope() && rb.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }

            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Crouching
        else if (crouching)
        {
            state = MovementState.crouching;
            GetComponent<CapsuleCollider>().height = 1;
            GetComponent<CapsuleCollider>().center = new Vector3(0, 0.35f, 0);
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && sprinting)
        {
            Debug.Log("Sprint");
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (moveSpeed < airMinSpeed)
                desiredMoveSpeed = airMinSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else
            {
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;

        // deactivate keepMomentum
        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        // smoothly lerp movementSpeed to desired value
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (OnSlope())
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
                time += Time.deltaTime * speedIncreaseMultiplier;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        if (activeGrapple) return;

        // calculate movement direction
        moveDirection = orientation.transform.forward * verticalInput + orientation.transform.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if (!wallrunning) rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (activeGrapple) return;

        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public override void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    public void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
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

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

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

