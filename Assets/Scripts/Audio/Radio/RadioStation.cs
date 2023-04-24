using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioStation : MonoBehaviour
{
    [Header("==========Attributes==========")]
    public string stationName;
    public Sprite stationSprite;

    [Header("==========Audios==========")]
    public AudioSource currentAudio;
    public AudioClip audioPlaying;
    public int currentClipNumb;
    public AudioClip[] songs;

    public AudioClip[] currentClips;

    [Header("==========Playing==========")]
    public bool currentRadio;
    public bool stopRadio;
    public float waitTime;

    private void Start()
    {
        currentAudio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (currentRadio != true && stopRadio != true)
        {
            waitTime -= 1 * Time.deltaTime;
            if (waitTime <= 0)
            {
                currentAudio.Stop();
                ShuffleRadio();
                stopRadio = true;
            }
        }

        if (!stopRadio)
        {
            if (!currentAudio.isPlaying)
            {
                currentClipNumb++;
                if (currentClipNumb >= currentClips.Length)
                {
                    currentClipNumb = 0;
                }

                audioPlaying = currentClips[currentClipNumb];
            }
        }
    }

    public void ShuffleRadio()
    {
        currentClips = songs;
        List<int> numbersTaken = new List<int>();

        for (int i = 0; i < songs.Length; i++)
        {
            bool next = false;
            while (next != false)
            {
                int newNumber = Random.Range(0, songs.Length);
                if (numbersTaken.Contains(newNumber) != true)
                {
                    currentClips[i] = songs[newNumber];
                    next = true;
                }
            }
        }
    }
}
