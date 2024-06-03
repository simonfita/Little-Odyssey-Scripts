using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Interaction : MonoBehaviour, I_Interactable
{
    public NPC self;

    public bool CanBeInteracted()
    {
        return enabled && !self.asleep && Refs.player.talkedToNPC == false;
    }

    public string GetInteractionText()
    {
        return "Talk";
    }

    public Transform GetInteractionTransform()
    {
        return transform;
    }

    public InteractionMountingRequirement GetMountingRequirement()
    {
        return self.token.inSlot ? InteractionMountingRequirement.OnlyMounted : InteractionMountingRequirement.OnlyNotMounted;
    }

    public void Interact()
    {
        self.StartDialogue();
    }
}
