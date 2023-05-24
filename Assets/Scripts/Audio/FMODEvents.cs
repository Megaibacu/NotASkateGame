using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("FootSteps SFX")]
    [field: SerializeField] public EventReference footSteps { get; private set; }
    public FMOD.Studio.EventInstance footStep;

    [field: Header("Jump SFX")]
    [field: SerializeField] public EventReference jumpSound { get; private set; }

    [field: Header("Slide SFX")]
    [field: SerializeField] public EventReference slide { get; private set; }

    [field: Header("Grapple SFX")]
    [field: SerializeField] public EventReference grapple { get; private set; }

    [field: Header("SkateRoll SFX")]
    [field: SerializeField] public EventReference skateRoll { get; private set; }
    public FMOD.Studio.EventInstance skateRolling;

    [field: Header("SkateJump SFX")]
    [field: SerializeField] public EventReference skateJump { get; private set; }

    [field: Header("Grind SFX")]
    [field: SerializeField] public EventReference grind { get; private set; }



   public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("More than one FMODEvents in scene");

            
        }
        instance = this;
    }

    private void Start()
    {
        footStep = RuntimeManager.CreateInstance(footSteps);
        skateRolling = RuntimeManager.CreateInstance(skateRoll);
    }
}
