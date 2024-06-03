using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationSettlement : LocationBase
{
    public TriggerCollider enterTrigger;

    public List<Stockpile> settlementStockpiles;

    private void Awake()
    {
        enterTrigger.OnTriggerEntered += OnEnter;
        enterTrigger.OnTriggerExited += OnExit;
    }

    private void OnEnter(Collider col)
    {
        Refs.player.EnterSettlement(this);            
    }

    private void OnExit(Collider col)
    {
        Refs.player.ExitSettlement(this);
    }

}
