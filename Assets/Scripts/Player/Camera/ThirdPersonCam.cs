using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    StateChange state;
    public PlayerInput _playerInput;

    public float rotationSpeed;

    private void Start()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        state = player.GetComponent<StateChange>();
    }

    private void Update()
    {
        if(state.state == States.parkour && !player.GetComponent<PlayerMovement>().wallrunning)
        {
            //  rotate orientation
            Vector3 viewDir = player.position - new Vector3(transform.position.x, player.position.y, transform.position.z);
            orientation.forward = viewDir.normalized;

            //  rotate player object
            float horizontalInput = _playerInput.actions["Sideways"].ReadValue<float>(); 
            float verticalInput = _playerInput.actions["Forward"].ReadValue<float>();
            Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;

            if (inputDir != Vector3.zero)
                playerObj.forward = Vector3.Slerp(playerObj.forward, inputDir.normalized, Time.deltaTime * rotationSpeed);
        }
                    
    }
}
