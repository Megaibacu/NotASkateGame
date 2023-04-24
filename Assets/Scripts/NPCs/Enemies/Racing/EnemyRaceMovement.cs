using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRaceMovement : MonoBehaviour
{
    [Header("----------AI----------")]
    public Transform goal;
    NavMeshAgent nav;

    private void Start()
    {
        //nav = GetComponent<NavMeshAgent>();
        //nav.SetDestination(goal.position);
        //GetComponent<SplineWalker>().enabled = true;
        
    }



}
