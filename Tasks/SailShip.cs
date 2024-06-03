using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailShip : TaskBase
{
    public TokenSlot slot;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        npc.movement.enabled = false;
        slot.EnterSlot(npc.token);
    }

    
    protected override void OnDisable()
    {
        if (QuitUtil.isQuitting)
            return;

        base.OnDisable();
        slot.ExitSlot();
        npc.movement.enabled = true;
    }
}
