using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : MonoBehaviour
{
    [Header("==========Audios==========")]
    public RadioStation[] stations;
    public AudioMixer mixer;

    public int currentRadio;

    public float waitBeforeOff;

    private void Start()
    {
        PlayRadio();
    }

    private void Update()
    {

    }

    public void AdjustVolume(float value)
    {
        mixer.SetFloat("MusicVolume", value);
    }

    public void PlayRadio()
    {
        for (int i = 0; i < stations.Length; i++)
        {
            if (i != currentRadio)
            {
                stations[i].currentAudio.volume = 0;
                stations[i].waitTime = waitBeforeOff;
                stations[i].currentRadio = false;
            }
        }

        stations[currentRadio].currentAudio.volume = 1;
        stations[currentRadio].stopRadio = false;
        stations[currentRadio].currentRadio = true;
    }

    public void NextStation()
    {
        currentRadio++;

        if (currentRadio >= stations.Length)
        {
            currentRadio = 0;
        }
    }

    public void PreviousStation()
    {
        currentRadio--;

        if (currentRadio < 0)
        {
            currentRadio = stations.Length - 1;
        }
    }
}
