using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public CollectibleSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system = FindObjectOfType<CollectibleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            system.money++;
            Debug.Log(system);
            Destroy(this.gameObject);
        }
    }
}
