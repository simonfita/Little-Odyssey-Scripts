using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttendStand : TaskBase
{
    protected override void Update()
    {
        if (Vector3.Distance(npc.transform.position, npc.locations.ownStand.GetPosition()) > 0.5f)
            npc.movement.GoTo(npc.locations.ownStand.GetPosition(), true);
        else
        { 
            Vector3 dir = transform.position - npc.locations.ownStand.transform.position;
            transform.forward = dir;
        }
    }
}


