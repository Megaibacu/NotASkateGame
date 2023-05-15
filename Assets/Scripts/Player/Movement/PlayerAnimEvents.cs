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

    }
}
