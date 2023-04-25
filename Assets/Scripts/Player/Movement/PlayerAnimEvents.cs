using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{

    Movement mS;
    Rigidbody rb;
    private void Start()
    {
        mS = GetComponent<Movement>();
        rb = GetComponent<Rigidbody>();
    }
    public void GetUp()
    {
        mS.fall = false;
    }

    public void RestartRB()
    {
        rb.isKinematic = false;
    }
}
