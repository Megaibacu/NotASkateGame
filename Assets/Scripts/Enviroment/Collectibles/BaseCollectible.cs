using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCollectible : MonoBehaviour
{
    public CollectibleSystem collectibles;

    private void Start()
    {
        collectibles = FindObjectOfType<CollectibleSystem>();
    }

    public virtual void ApplyCollectible()
    {
        collectibles.NewCollectible();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyCollectible();
            Destroy(this.gameObject);
        }
    }
}
