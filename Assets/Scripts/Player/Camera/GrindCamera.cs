using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GrindCamera : MonoBehaviour
{
    public Transform grindcamfollow;
    public Transform player;
    CinemachineVirtualCamera camara;

    public bool active;

    
    void Start()
    {
        camara = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = grindcamfollow.position;


        if (active)
        {
            camara.Priority = 12;
        }
        else
        {
            camara.Priority = 0;
        }
    }
}
