using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform playerObj;
    public Rigidbody rb;
    StateChange state;
    public PlayerInput _playerInput;
    public CinemachineFreeLook cam;
    public CinemachineVirtualCamera FPSCam;
    public float camCd;
    public float camCdT;

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
            cam.Priority = 11;
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

         if(state.state == States.skating)
        {
            if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
            {
                if(camCdT > 0)
                {
                    camCdT -= Time.deltaTime;
                }
                else
                cam.Priority = 9;

            }
            else
            {
                camCdT = camCd;
                cam.Priority = 11;
            }
        }
    }

    public void CameraLookForward()
    {

    }



    public void ChangeCameras(int FPSCamPriority, int TPSCamPriority)
    {
        FPSCam.Priority = FPSCamPriority;
        cam.Priority = TPSCamPriority;
    }
}
