using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform playerObj;
    private Rigidbody rb;
    private PlayerMovement pm;
    public PlayerInput _playerInput;

    [Header("Sliding")]
    public float maxSlideTime;
    public float slideForce;
    private float slideTimer;
    public bool sliding;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
        slideTimer = maxSlideTime;
        _playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        sliding = pm.sliding;
        if(pm.sliding)
        {
            SlidingMovement();
        }
    }

    public void StartSlide()
    {
        pm.sliding = true;

        //  animación
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //  sliding normal
        if(!pm.onUpSlope && !pm.onDownSlope || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        //  sliding down a slope
        else
        {
            rb.AddForce(pm.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);

        }

        if (slideTimer <= 0)
            StopSlide();

    }

    public void StopSlide()
    {
        pm.sliding = false;
        slideTimer = maxSlideTime;
    }
}
