using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRunning : MonoBehaviour
{
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float wallJumpUpForce;
    public float wallJumpSideForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    [Header("Exiting")]
    public bool useGravity;
    public float gravityCounterForce;

    [Header("References")]
    public Transform orientation;
    public Transform body;
    private PlayerMovement pm;
    private Rigidbody rb;
    public PlayerInput _playerInput;
    private GameObject lastWall;
    private GameObject wallhit;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (!AboveGround())
            lastWall = null;
        CheckForWall();
        StateMachine();
        //Debug.Log(wallLeft);
        //Debug.Log(wallLeft);
        //esto lo he quitado porque ocurria cada frame
    }

    private void FixedUpdate()
    {
        if(pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position + transform.up, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position + transform.up, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
        if (wallLeft || wallRight)
            wallhit = wallRight ? rightWallHit.transform.gameObject : leftWallHit.transform.gameObject;
    }

    private bool AboveGround()
    {

        if(pm.state == PlayerMovement.MovementState.wallrunning)
        {
            return true;
        }
        else
        {
            return pm.state == PlayerMovement.MovementState.air;
        } 
        
    }

    private void StateMachine()
    {
        //  State 1 - Wallruning
        if ((wallLeft || wallRight) && AboveGround() && !exitingWall && (lastWall == null || lastWall != wallhit))
        {
            if (!pm.wallrunning)
                StartWallRun();

            //  wallrun timer
            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;
            if (wallRunTimer <= 0 && pm.wallrunning)
            {
                lastWall = wallRight ? rightWallHit.transform.gameObject : leftWallHit.transform.gameObject;
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }


            if (_playerInput.actions["Jump"].WasPressedThisFrame())
                WallJump();
        }

        //  state 2 - Exit
        else if(exitingWall)
        {          
            if (pm.wallrunning)    
                StopWallRun();          
            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        //  state 3 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        wallRunTimer = maxWallRunTime;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        //  forward force
        if (wallRight && !exitingWall)
        {
            body.forward = -wallForward;
            orientation.forward = -wallForward;
            rb.velocity = -wallForward * wallRunForce * Time.deltaTime + Vector3.up * rb.velocity.y;
        }

        else if(!exitingWall)
        {
            body.forward = wallForward;
            orientation.forward = wallForward;
            rb.velocity = wallForward * wallRunForce * Time.deltaTime + Vector3.up * rb.velocity.y;
        }

        // push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput > 0))
            rb.AddForce(-wallNormal * 50, ForceMode.Force);

            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRun()
    {
        rb.constraints = 0;
        rb.constraints = RigidbodyConstraints.FreezeRotationX;
        pm.wallrunning = false;
    }

    private void WallJump()
    {
        lastWall = wallRight ? rightWallHit.transform.gameObject : leftWallHit.transform.gameObject;

       
        // enter exiting wall state
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        forceToApply.z = rb.velocity.z;

        // reset y velocity and add Force
        pm.Jump(forceToApply);
    }
}
