using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    [Header("----------Layout----------")]
    public Transform start;
    public EndGoal end;
    public Transform[] startPositions;

    [Header("----------Timer----------")]
    public float timeInRace;

    [Header("----------Participants----------")]
    public GameObject[] participants;
    public bool isRace;

    PlayerMovement playerMov;
    SkateController skateController;

    private void Update()
    {

        if (!end.raceEnded) { timeInRace += Time.deltaTime; }

        if (end.raceEnded)
        {
            if(end.entities[0].tag == "Enemy") { Debug.Log("Race Lost"); }
            if(end.entities[0].tag == "Player") { Debug.Log("Race Won"); }


        }
    }

    public void RaceStart()
    {
        playerMov = FindObjectOfType<PlayerMovement>();
        playerMov.GetComponent<StateChange>().state = States.parkour;
        playerMov.transform.position = start.transform.position;
        playerMov.enabled = false;
        skateController = FindObjectOfType<SkateController>();
        skateController.GetComponent<StateChange>().state = States.skating;
        skateController.transform.position = start.transform.position;
        skateController.enabled = false;

        if (isRace)
        {
            playerMov.GetComponent<StateChange>().state = States.skating;
            for (int i = 0; i < participants.Length; i++)
            {
                if (participants[i].gameObject.tag == "RaceEnemy")
                {
                    participants[i].GetComponent<EnemyRaceMovement>().enabled = false;
                }
            }
        }

        StartCoroutine(RaceStartTimer());
    }

    public IEnumerator RaceStartTimer()
    {
        yield return new WaitForSeconds(3);

        if (!isRace)
        {
            playerMov.enabled = true;
        }

        if (isRace)
        {
            skateController.enabled = true;

            for (int i = 0; i < participants.Length; i++)
            {
                if (participants[i].gameObject.tag == "RaceEnemy")
                {
                    participants[i].GetComponent<EnemyRaceMovement>().enabled = true;
                }
            }
        }
        
    }
}
