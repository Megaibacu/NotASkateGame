using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallFootsteps : MonoBehaviour
{
    public GameObject audioobject;
    NewAudioManager manager;
    void Start()
    {
        manager = audioobject.GetComponent<NewAudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FootstepL()
    {
        manager.PlaySound("FootstepL");
    }
    public void FootstepR()
    {
        manager.PlaySound("FootstepR");
    }
}
