using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{

    Movement mS;
    private void Start()
    {
        mS = GetComponent<Movement>();
    }
    public void GetUp()
    {
        mS.fall = false;
    }

    public void RestartRB()
    {
        mS.rb.isKinematic = false;
    }
}
