using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinVase : MonoBehaviour, I_Interactable
{
    public int moneyReward;

    private void Collect()
    {
        Refs.inventory.playerMoney.amount += moneyReward;
        Destroy(gameObject);

        //Refs.ui.hud.SpawnGameMessege("You found some coins inside!", GameMessegeTypes.Collectible);
    }

    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.NotMountedAndMounted; }

    public bool CanBeInteracted() { return true; }

    public void Interact() { Collect(); }

    public string GetInteractionText() { return "Pick Up"; }

    public Transform GetInteractionTransform() { return transform; }
}
