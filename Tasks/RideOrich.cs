using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RideOrich : TaskBase
{
    public Orich orich;
    
    public LocationPatrolRoute route;
    int i = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        npc.movement.enabled = false;
        orich.slot.EnterSlot(npc.token);
    }

    protected override void Update()
    {
        orich.agent.isStopped = npc.isInDialogue;

        if (orich.ReachedDestination())
        {
            orich.GoTo(route.GetPosition(i));
            i++;
        }
    }
    
    protected override void OnDisable()
    {
        if (QuitUtil.isQuitting)
            return;

        base.OnDisable();
        orich.slot.ExitSlot();
        npc.movement.enabled = true;

    }
}
