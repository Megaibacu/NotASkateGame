using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finderscript : MonoBehaviour
{
    public int numberofobjects;
    void Start()
    {
        //este script esta para los objetos que se tengan que desactivar despues de que otros tantos los encuentrn con gameobject.Find
    }

    // Update is called once per frame
    void Update()
    {
        if (numberofobjects<=0)
        {
            this.gameObject.SetActive(false);
            this.enabled = false;
        }
    }
}
