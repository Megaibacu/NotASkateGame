using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buying : MonoBehaviour
{
    Image lockicon;

    public Customizationshop shop;

    int currentimage;

    float wait;

    bool doonce;
    
    void Start()
    {
        wait = 0.5f;
       
    }

    // Update is called once per frame
    void Update()
    {
        if (wait > 0)
        {
            wait -= Time.deltaTime;
        }
        else
        {
            if (doonce == false)
            {
                lockicon = shop.lockicon;
                doonce = true;
            }

            if (shop.currentspray.unlocked == false)
            {
                lockicon.enabled = true;
            }
            else
            {
                lockicon.enabled = false;
            }  
        }

        if (shop.isshoping == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //buythething
            }
        }

    }
}
