using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockSkateFlip : MonoBehaviour
{

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.F))
        {
            transform.Rotate(0, 1.3f, 0);
        }
    }
}
