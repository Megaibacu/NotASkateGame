using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum States { parkour, skating}

public class StateChange : MonoBehaviour
{
    public Movement _mov;
    private SkateController _sc;
    [HideInInspector]public PlayerMovement _pMov;

    [Header("==========Boost==========")]
    public float parkourSpeedPercentage;
    public float skateSpeedPercentage;
    public float skateSwitchBoost;

    [Header("==========Particles==========")]
    public GameObject[] parkourPartciles;
    public GameObject[] skateParticles;

    public States state;
    public GameObject skateGO;
    public GameObject audiomanager;
    public float boostTimer;
    public float boostMultiplier;
    Animator anim;

    public float cooldown;

    //para pausar
    public bool paused;

    //para resetear el sonido
    public GameObject newaudiomanager;
    AudioSource source;

    private void Start()
    {
        _pMov = GetComponent<PlayerMovement>();
        _sc = GetComponent<SkateController>();
        anim = GetComponent<Animator>();
        _mov = _pMov;
    }

    private void Update()
    {
        AnimationLayers();
        _mov.StateHandler();
        //_mov.SlopeDetection();
        _mov.GroundRotation();
        _mov.Grounded();
        _mov.CoyoteCheck();
        _mov.SpeedControl();
        _mov.AirMovement();
        _mov.AirDetection();
        _mov.SoundCheck();
        _mov.Move();
    }

    public void AnimationLayers()
    {
        if (state == States.parkour)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("Parkour"), 1.0f);
            anim.SetLayerWeight(anim.GetLayerIndex("SkateLayer"), 0.0f);
        }
        else if (state == States.skating)
        {
            anim.SetLayerWeight(anim.GetLayerIndex("SkateLayer"), 1.0f);
            anim.SetLayerWeight(anim.GetLayerIndex("Parkour"), 0.0f);
        }
    }

    public void SwitchState()
    {
        if(state == States.parkour)
        {
                FMODEvents.instance.musicEvent.setParameterByName("State", 1);
                state = States.skating;
                AudioManager.instance.PlayOneShot(FMODEvents.instance.stateSwitch, transform.position);
                skateGO.SetActive(true);                
                parkourSpeedPercentage = Mathf.Abs(_mov.currentSpeed / _pMov.sprintSpeed);
                _mov = _sc;
                _mov.currentSpeed = _mov.maxSpeed * parkourSpeedPercentage;
        }

        else
        {
                AudioManager.instance.StopSound(FMODEvents.instance.skateRolling);
                 FMODEvents.instance.musicEvent.setParameterByName("State", 0);
                state = States.parkour;
                _sc.isplaying = false;
                skateGO.SetActive(false);               

                //Conservation of momentum
                skateSpeedPercentage = _mov.currentSpeed / _mov.maxSpeed;
                _mov = _pMov;
                if (skateSpeedPercentage < .5f)
                {
                    _mov.currentSpeed = 5;
                }
                else
                {
                    _mov.currentSpeed = 10;
                }
            
        }
    }

    public void ParticleActivation(GameObject[] activeGOs, GameObject[] deactivateGOs)
    {
        for (int i = 0; i < activeGOs.Length; i++)
        {
            activeGOs[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < deactivateGOs.Length; i++)
        {
            deactivateGOs[i].gameObject.SetActive(false);
        }
    }

    /*public IEnumerator SkateBoost()
    {
        skateController.maxSpeed = skateSwitchBoost;
        yield return new WaitForSeconds(boostTimer);
        skateController.maxSpeed = skateController.ogMaxSpeed;
        StopCoroutine(SkateBoost());
    }*/
}
