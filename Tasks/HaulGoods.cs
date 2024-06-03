using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaulGoods : TaskBase
{

    public Stockpile firstStockpile;
    public Stockpile secondStockpile;

    Coroutine haul;

    protected override void Update()
    {
        if(haul==null && !firstStockpile.isEmpty && secondStockpile !=null)
            haul = StartCoroutine(HaulGood());
    }

    protected override void OnDisable()
    {
        if (haul != null)
        { 
            StopCoroutine(haul);
            haul = null;
        } 
    }
    
    public override bool CanEndTask()
    {
        return npc.token.heldGoods == null;
    }

    IEnumerator HaulGood()
    {
        npc.movement.GoTo(firstStockpile.npcPoint.position);

        while (!npc.movement.reachedLocation)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1);
        firstStockpile.TakeFrom(npc.token);

        npc.movement.GoTo(secondStockpile.npcPoint.position);
        while (!npc.movement.reachedLocation)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1);
        npc.token.LeaveGoods(secondStockpile);

        haul = null;
        yield break;
    }
}
