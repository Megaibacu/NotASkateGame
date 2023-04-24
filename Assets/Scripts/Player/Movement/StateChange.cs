using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum States { parkour, skating}

public class StateChange : MonoBehaviour
{
    [Header("==========Boost==========")]
    public float parkourSpeedPercentage;
    public float skateSpeedPercentage;
    public float skateSwitchBoost;

    public States state;
    public PlayerMovement playerMov;
    public SkateController skateController;
    public GameObject skateGO;
    public GameObject audiomanager;
    public float boostTimer;
    public float boostMultiplier;
    Animator anim;
    public PlayerInput _playerInput;

    public float cooldown;

    //para pausar
    public bool paused;

    //para resetear el sonido
    public GameObject newaudiomanager;
    AudioSource source;

    private void Start()
    {
        playerMov = GetComponent<PlayerMovement>();
        skateController = GetComponent<SkateController>();
        anim = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        InputManager();

        if(state == States.parkour)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Parkour"), 1.0f);
            anim.SetLayerWeight(anim.GetLayerIndex("SkateLayer"), 0.0f);
        }
        else if(state == States.skating)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("SkateLayer"), 1.0f);
            anim.SetLayerWeight(anim.GetLayerIndex("Parkour"), 0.0f);
        }
    }

    public void InputManager()
    {
        if(state == States.parkour)
        {
            if (_playerInput.actions["SwitchState"].WasPressedThisFrame() && paused == false)
            {
                state = States.skating;
                skateGO.SetActive(true);
                playerMov.gameObject.GetComponent<PlayerMovement>().enabled = false;
                playerMov.gameObject.GetComponent<SkateController>().enabled = true;

                parkourSpeedPercentage = Mathf.Abs(playerMov.moveSpeed / playerMov.sprintSpeed);
                skateController.currentSpeed = skateController.maxSpeed * parkourSpeedPercentage;
                //StartCoroutine(SkateBoost());
            }
        }
        else
        {
            if (_playerInput.actions["SwitchState"].WasPressedThisFrame() && paused == false)
            {
                state = States.parkour;
                skateController.isplaying = false;
                skateGO.SetActive(false);
                playerMov.gameObject.GetComponent<PlayerMovement>().enabled = true;
                playerMov.gameObject.GetComponent<SkateController>().enabled = false;

                //Conservation of momentum
                skateSpeedPercentage = skateController.currentSpeed / skateController.maxSpeed;
                if (skateSpeedPercentage < .5f)
                {
                    playerMov.moveSpeed = 5;
                }
                else
                {
                    playerMov.moveSpeed = 10;
                }
                
                //to stop the sound of the skate
               //newaudiomanager.GetComponent<NewAudioManager>().StopSound();
               // source = newaudiomanager.GetComponent<AudioSource>();
                //source.volume = 1;
            }

        }
    }

    public IEnumerator SkateBoost()
    {
        skateController.maxSpeed = skateSwitchBoost;
        yield return new WaitForSeconds(boostTimer);
        skateController.maxSpeed = skateController.ogMaxSpeed;
        StopCoroutine(SkateBoost());
    }
}
