using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : TaskBase
{


    protected override void OnEnable()
    {
        base.OnEnable();
        npc.movement.GoTo(npc.locations.home.GetPosition());
    }

    protected override void Update()
    {
        if (npc.asleep || npc.isInvolvedInMainQuest || npc.involvedInContracts.Count!=0)
            return;
        if(npc.movement.reachedLocation)
        {
            npc.token.SetActive(false);
            npc.asleep = true;
        }

        
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        npc.token.SetActive(true);
        npc.asleep = false;

    }
}
