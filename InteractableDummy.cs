using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InteractableDummy : MonoBehaviour, I_Interactable
{
    public Action OnInteracted;

    public string interactionText;

    public InteractionMountingRequirement mountingRequirement;

    
    public bool CanBeInteracted()
    {
        return enabled;
    }

    public string GetInteractionText()
    {
        return interactionText;
    }

    public Transform GetInteractionTransform()
    {
        return transform;
    }

    public InteractionMountingRequirement GetMountingRequirement()
    {
        return mountingRequirement;
    }

    public void Interact()
    {   
        OnInteracted?.Invoke();
    }

    

}
