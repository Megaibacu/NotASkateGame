using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GrindCamera : MonoBehaviour
{
    public Transform grinscamfollow;
    CinemachineVirtualCamera camara;

    public bool active;

    
    void Start()
    {
        camara = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = grinscamfollow.position;

        if (active)
        {
            camara.Priority = 10;
        }
        else
        {
            camara.Priority = 0;
        }
    }
}
