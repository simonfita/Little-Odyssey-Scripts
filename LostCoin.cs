using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostCoin : MonoBehaviour, I_Interactable
{
    public int moneyReward;

    public System.Action<LostCoin> OnCollected;

    private void Collect()
    {
        Refs.inventory.playerMoney.amount += moneyReward;
        Refs.ui.hud.SpawnGameMessege("I found half a coin!");

        OnCollected?.Invoke(this);
    }

    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.NotMountedAndMounted; }

    public bool CanBeInteracted() { return true; }

    public void Interact() { Collect(); }

    public string GetInteractionText() { return "Pick Up"; }

    public Transform GetInteractionTransform() { return transform; }
}