using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    [field: Header("FootSteps SFX")]
    [field: SerializeField] public EventReference footSteps { get; private set; }
    public FMOD.Studio.EventInstance footStep;

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
    }
}
