using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socialize : TaskBase
{
    private float timeToMove = 0;

    protected override void Update ()
    {
        timeToMove -= Time.deltaTime;

        if (timeToMove > 0)
            return;
        
        timeToMove = Random.Range(2, 10);

        npc.movement.GoTo(npc.locations.meetingArea.GetPosition(),true);
    }
}
