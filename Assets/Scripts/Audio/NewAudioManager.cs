using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAudioManager : MonoBehaviour
{
    public List<AudioSource> sources;
    public NewSound[] sounds;
    void Start()
    {
        sources.AddRange(gameObject.GetComponents<AudioSource>());
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

                    sources[0].clip = sounds[amount].clip;
                    sources[0].volume = sounds[amount].volume;
                    sources[0].Play();
                }
                else
                {
                    sources[1].clip = sounds[amount].clip;
                    sources[1].volume = sounds[amount].volume;
                    sources[1].Play();
                }
                
                if (sounds[amount].loop == true)
                {
                    sources[0].loop = true;
                }
                else
                {
                    sources[0].loop = false;
                }
                amount = sounds.Length;
            }
        }
    }
    public void StopSound()
    {
        sources[0].Stop();
    }
}
