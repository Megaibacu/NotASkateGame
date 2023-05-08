using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _pInput;
    private StateChange _sChange;
    private Sliding _slide;
    private splineTesting _grind;
    private Grapplin _grap;
    // Start is called before the first frame update
    void Start()
    {
        _grap = GetComponent<Grapplin>();
        _pInput = GetComponent<PlayerInput>();
        _sChange = GetComponent<StateChange>();
        _slide = GetComponent<Sliding>();
        _grind = GetComponent<splineTesting>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_pInput.actions["SwitchState"].WasPressedThisFrame() && _sChange._mov.paused == false)
        {
            _sChange.SwitchState();
        }

        _sChange._mov.horizontalInput = _pInput.actions["Sideways"].ReadValue<float>();
        _sChange._mov.verticalInput = _pInput.actions["Forward"].ReadValue<float>();

        if (_pInput.actions["Jump"].IsPressed() && ((_sChange._mov.readyToJump && _sChange._mov.grounded) || _sChange._mov.coyote && _sChange._mov.readyToJump || _grind.sc.grinding) && _sChange._mov.paused == false)
        {
            _sChange._mov.Jump(GetComponent<Rigidbody>().velocity);
        }
        if (_pInput.actions["Jump"].WasReleasedThisFrame() && ((_sChange._mov.readyToJump && _sChange._mov.grounded) || _sChange._mov.coyote && _sChange._mov.readyToJump) && _sChange._mov.paused == false)
        {
            _sChange._mov.JumpRelease();
        }

        if (_pInput.actions["Sprint"].WasPressedThisFrame())
        {
            _sChange._pMov.sprinting = !_sChange._pMov.sprinting;
        }

        if (_pInput.actions["Crouch"].IsPressed() && !_sChange._pMov.sliding && _sChange._mov.grounded)
        {
            _sChange._pMov.crouching = true;
        }

        if (_pInput.actions["Crouch"].WasPressedThisFrame() && (_sChange._mov.horizontalInput != 0 || _sChange._mov.verticalInput != 0) && _sChange._mov.grounded)
        {
            _slide.StartSlide();
        }

        if (_pInput.actions["Crouch"].WasReleasedThisFrame() && _slide.sliding)
        {
            _slide.StopSlide();
        }

        if (!_pInput.actions["Crouch"].IsPressed() && !Physics.Raycast(_sChange._mov.orientation.transform.position + Vector3.down * 0.3f, Vector3.up, 1 * 0.5f + 0.1f))
        {

            _sChange._pMov.crouching = false;

            _sChange._mov.anim.SetBool("Crouch", false);
        }

        if (_pInput.actions["Grapple"].WasPressedThisFrame() && _grap.grappleObject != null && _grap.isVisible(_grap.cam, _grap.grappleObject)) _grap.StartGrapple();


        /*//===============Skate Turning 180=============== 
        if (_pInput.actions["Turn"].WasPressedThisFrame() && _sChange._mov.currentSpeed <= 0.5f && _sChange.state == States.skating)
        {
            _sChange._mov.Turn
        }*/
    }
}
