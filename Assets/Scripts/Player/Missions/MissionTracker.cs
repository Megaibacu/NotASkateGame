using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTracker : MonoBehaviour
{
    [Header("----------Mission----------")]
    public Missions[] missions;
}

[System.Serializable]
public class Missions
{
    public string missionName;
    public EndGoal goal;
}
