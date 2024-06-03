using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionMountingRequirement
{ 
    OnlyNotMounted,
    OnlyMounted,
    NotMountedAndMounted,
}

public interface I_Interactable
{
    public InteractionMountingRequirement GetMountingRequirement();
    public bool CanBeInteracted();
    public void Interact();
    public string GetInteractionText();
    public Transform GetInteractionTransform();


    //removing if does not exist
    //interaction is INTERFACES, not COMPONENT, I cannot null-check it normally
    //I can't think of better solution, maybe TODO: better
    public static bool IsValid(I_Interactable interaction)
    {
        if (interaction == null)
            return false;

        try
        {
            interaction.GetInteractionTransform();
        }
        catch
        {
            return false;
        }
        return true;
    }
}