using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Pause : MonoBehaviour
{
    [Header("thing1 = player, thing2 = audiomanager, thing3 = Pause UI, thing4 = thirdpersoncamera")]
    public GameObject[] thingsthatneedpausing;
    public GameObject blackbox;
    //para la blackbox en el menu de pausa
    public Transform[] positions;

    public PlayerInput _playerInput;

    int currentselection;
    float jumptimer;
    float soundstimer;
    float currentvolume;

    bool hasgrabbedsounds;
   
    void Start()
    {
        soundstimer = 0.3f;
        _playerInput = FindObjectOfType<PlayerInput>();

        positions[0] = thingsthatneedpausing[2].transform.GetChild(2);
        positions[1] = thingsthatneedpausing[2].transform.GetChild(3);
    }

    // Update is called once per frame
    void Update()
    {

        if (_playerInput.actions["Pause"].WasPressedThisFrame())
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
                changestates("Pause");
                _playerInput.SwitchCurrentActionMap("MenuControls");
            }
            else
            {                
                changestates("UnPause");
                Time.timeScale = 1;
                _playerInput.SwitchCurrentActionMap("Gameplay_p");
            }
        }

        if (Time.timeScale == 0)
        {
            menunavigation();
        }
        if (jumptimer > 0)
        {
            jumptimer -= Time.deltaTime;
        }
        else
        {
            if (Time.timeScale == 1)
            {
                thingsthatneedpausing[0].GetComponent<PlayerMovement>().paused = false;
            }
        }
    }
    void changestates(string state)
    {
        if (state == "Pause")
        {
            thingsthatneedpausing[0].GetComponent<StateChange>().paused = true;
            thingsthatneedpausing[0].GetComponent<PlayerMovement>().paused = true;
            thingsthatneedpausing[2].SetActive(true);
            thingsthatneedpausing[3].GetComponent<CinemachineBrain>().enabled = false;
            //esto es lo que hay que tocar cuando se pausa
        }
        else
        {
            thingsthatneedpausing[0].GetComponent<StateChange>().paused = false;
            thingsthatneedpausing[2].SetActive(false);
            jumptimer = 0.1f;
            thingsthatneedpausing[3].GetComponent<CinemachineBrain>().enabled = true;
            //lo mismo pero para cuando se despausa
        }
    }
    void menunavigation()
    {
        //this is for moving the blackbox and for seeing what the player is selecting
        if (currentselection == 0)
        {
            blackbox.transform.position = positions[0].position;
        }
        else
        {
            blackbox.transform.position = positions[1].position;
        }

        if (_playerInput.actions["Up"].WasPressedThisFrame())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.h_button, transform.position);
            if (currentselection == 0)
            {
                currentselection++;
            }
            else
            {
                currentselection = 0;
            }
        }
        if (_playerInput.actions["Down"].WasPressedThisFrame())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.h_button, transform.position);
            if (currentselection == 1)
            {
                currentselection--;
            }
            else
            {
                currentselection = 1;
            }
        }

        //this is for resuming or quitting the game
        if(_playerInput.actions["Select"].WasPressedThisFrame())
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.p_button, transform.position);
            if (currentselection == 0)
            {
                changestates("UnPause");
                Time.timeScale = 1;
                _playerInput.SwitchCurrentActionMap("Gameplay_p");
            }
            else
            {
                Application.Quit();
            }
        }
    }
}
