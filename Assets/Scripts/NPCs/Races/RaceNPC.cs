using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceNPC : GenericNPC
{
    public RaceManager race;

    public void StartTheRace()
    {
        print("Hello");
        race.RaceStart();
    }
}
