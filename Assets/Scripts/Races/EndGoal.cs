using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGoal : MonoBehaviour
{
    public List<GameObject> entities = new List<GameObject>();
    public bool raceEnded;
    PlayerInteract player;

    private void Start()
    {
        player = FindObjectOfType<PlayerInteract>();
        raceEnded = false;
    }

    public void EndRace()
    {
        raceEnded = true;
        entities.Add(player.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        entities.Add(other.gameObject);

        if (other.tag == "Player")
        {
            other.GetComponent<StateChange>().enabled = true;
        }

        raceEnded = true;
    }
}
