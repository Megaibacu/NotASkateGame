using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;

public class Buying : MonoBehaviour
{
    Image lockicon;

    public Customizationshop shop;
    CollectibleSystem system;

    int currentimage;

    float wait;

    bool doonce;
    
    void Start()
    {
        wait = 0.5f;
       system = FindObjectOfType<CollectibleSystem>();
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
            if (Input.GetKeyDown(KeyCode.Space) && shop.currentspray.unlocked == false)
            {
                if (system.money >= shop.currentspray.price)
                {
                    system.money -= shop.currentspray.price;
                    shop.currentspray.unlocked = true;
                }
            }
        }

    }
}
