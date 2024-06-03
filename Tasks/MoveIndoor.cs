using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIndoor : TaskBase
{
    public Transform outdoor, indoor;
    public Goods goods;

    Coroutine move;

    protected override void Update()
    {
        if (move == null)
            move = StartCoroutine(Move());
    }

    protected override void OnDisable()
    {
        if (move != null)
        {
            StopCoroutine(move);
            move = null;
        }
    }

    public override bool CanEndTask()
    {
        return npc.token.heldGoods == null;
    }

    IEnumerator Move()
    {
        npc.movement.GoTo(outdoor.position);

        while (!npc.movement.reachedLocation)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(7);
        
        if (goods)
            npc.token.HoldGoods(Instantiate(goods));

        npc.movement.GoTo(indoor.position);
        while (!npc.movement.reachedLocation)
        {
            yield return new WaitForEndOfFrame();
        }
        npc.token.SetActive(false);
        npc.token.DestroyGoods();
        yield return new WaitForSeconds(20);
        npc.token.SetActive(true);


        move = null;
        yield break;
    }
}
