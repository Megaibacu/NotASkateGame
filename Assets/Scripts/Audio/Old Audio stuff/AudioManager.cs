using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Start()
    {
        FindObjectOfType<AudioClip>();
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.playOnAwake = false;
            s.source.loop = s.loop;
        }
    }

    public void PlaySound(string sound)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name.Equals(sound));

        if (s == null)
        {
            Debug.Log($"Sound {sound} not found");
        }

        Debug.Log(s.name);
        s.source.Play();
    }

    public void StopSound(string sound)
    {
        Sound s = Array.Find(sounds, Sound => Sound.name.Equals(sound));

        s.source.Stop();
    }
}
