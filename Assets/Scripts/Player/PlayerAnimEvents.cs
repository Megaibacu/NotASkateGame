using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    Tricking tR;
    private void Start()
    {
        tR = GetComponent<Tricking>();
    }
    public void GetUp()
    {
        tR.fall = false;
    }

    public void RestartRB()
    {
        tR.sC.rb.isKinematic = false;
    }
}
