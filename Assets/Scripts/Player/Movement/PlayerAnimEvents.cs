using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerAnimEvents : MonoBehaviour
{

    Movement mS;
    Rigidbody rb;
    private void Start()
    {
        mS = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
    }

    public void EndTrick()
    {
        
    }

    public void GetUp()
    {
        mS.fall = false;
    }

    public void RestartRB()
    {
        rb.isKinematic = false;
    }

    public void FootStep()
    {
        AudioManager.instance.PlayOneShot(FMODEvents.instance.footStep);
    }

    public void StopSteps()
    {
        AudioManager.instance.StopSound(FMODEvents.instance.footStep);
    }

    public void Land()
    {
        if(GetComponent<StateChange>().state == States.parkour)
        AudioManager.instance.PlayOneShot(FMODEvents.instance.p_Land, transform.position);
        else
        AudioManager.instance.PlayOneShot(FMODEvents.instance.s_Land, transform.position);

    }
}
