using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Mas de 1 audiomanager en escena");
        } 

        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldpos)
    {

        RuntimeManager.PlayOneShot(sound, worldpos);
    
    }
    public void PlayOneShot(EventInstance sound, Vector3 worldpos)
    {

        sound.start();

    }
}
