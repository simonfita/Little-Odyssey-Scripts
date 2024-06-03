using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : TaskBase
{
    public LocationPatrolRoute route;
    int i = 0;

    protected override void Update()
    {
        if (npc.movement.reachedLocation)
        {
            npc.movement.GoTo(route.GetPosition(i));
            i++;
        }
    }
}
