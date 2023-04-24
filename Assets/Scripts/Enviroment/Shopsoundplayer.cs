using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopsoundplayer : MonoBehaviour
{
    public AudioClip[] sounds;
    AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void playsound(string type)
    {
        if (type == "Arrow")
        {
            source.clip = sounds[0];
        }
        if (type == "Select")
        {
            source.clip = sounds[1];
        }
        if (type == "Enter")
        {
            source.clip = sounds[2];
        }
        source.Play();
    }
}
