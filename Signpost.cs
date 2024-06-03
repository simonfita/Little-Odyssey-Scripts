using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signpost : MonoBehaviour, I_Interactable
{
    [TextArea]
    public string directions;

    public bool CanBeInteracted()
    {
        return true;
    }

    public string GetInteractionText()
    {
        return "Read";
    }

    public Transform GetInteractionTransform()
    {
        return transform;
    }

    public InteractionMountingRequirement GetMountingRequirement()
    {
        return InteractionMountingRequirement.NotMountedAndMounted;
    }

    public void Interact()
    {
        Refs.ui.OpenReadable(directions);
    }

    
}
