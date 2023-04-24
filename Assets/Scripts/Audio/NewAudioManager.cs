using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAudioManager : MonoBehaviour
{
    AudioSource source;
    AudioSource source2;
    public NewSound[] sounds;
    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source2 = gameObject.AddComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySound(string type)
    {
        //esto chekea todos los sonido del array y suena el que se indique
        for (int amount = 0; amount < sounds.Length; amount++)
        {
            if (sounds[amount].name == type)
            {
                if (sounds[amount].canoverlap == false)
                {
                    source.clip = sounds[amount].clip;
                    source.volume = sounds[amount].volume;
                    source.Play();
                }
                else
                {
                    source2.clip = sounds[amount].clip;
                    source2.volume = sounds[amount].volume;
                    source2.Play();
                }
                
                if (sounds[amount].loop == true)
                {
                    source.loop = true;
                }
                else
                {
                    source.loop = false;
                }
                amount = sounds.Length;
            }
        }
    }
    public void StopSound()
    {
        source.Stop();
    }
}
