using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private I_Interactable currentInteraction;
    private List<I_Interactable> interactions = new List<I_Interactable>();
    public SphereCollider col;

    private void Start()
    {
        Refs.turtle.OnMounted += SetMountedRadius;
        Refs.turtle.OnUnmounted += SetUnmountedRadius;
    }

    private void SetMountedRadius()
    {
        col.radius = 8;
    }
    private void SetUnmountedRadius()
    {
        col.radius = 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        I_Interactable _interaction = other.GetComponent<I_Interactable>();
        if(_interaction==null) //try to get in child
            _interaction = other.GetComponentInChildren<I_Interactable>();
        if(_interaction==null)
            return;

        interactions.Add(_interaction);
    }
    private void OnTriggerExit(Collider other)
    {
        I_Interactable _interaction = other.GetComponent<I_Interactable>();
        if (_interaction == null) //try to get in child
            _interaction = other.GetComponentInChildren<I_Interactable>();
        if (_interaction == null)
            return;


        interactions.Remove(_interaction);

    }

    private void Update()
    {
        currentInteraction = FindClosestInteraction();
        Refs.ui.hud.SetCurrentInteractable(currentInteraction);

        if (!Refs.player.token.canMove && !Refs.turtle.isMounted)
            return; 

        if (Refs.controls.Other.Interaction.WasPerformedThisFrame() && currentInteraction != null)
        {
            //Refs.controls.Other.Interaction.Reset();
            currentInteraction.Interact();
            Refs.ui.inputHints.CompletedAction("Interaction");
        }
    }

    //Can return null
    private I_Interactable FindClosestInteraction() 
    {
        if (Refs.playerCamera.inTopDownView)
            return null;

        I_Interactable best = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        interactions.RemoveAll(item => !I_Interactable.IsValid(item));
        
        for(int i = interactions.Count -1; i>=0;i --)// I_Interactable potentialTarget in interactions)
        {
            float distanceToTarget = Vector3.Distance(interactions[i].GetInteractionTransform().position, currentPosition);

            if (distanceToTarget > col.radius * 4) //fallback
            {
                interactions.RemoveAt(i);
                continue;
            }
            
            
            if (distanceToTarget >= closestDistance)
                continue;           

            if (!CanPotentiallyInteract(interactions[i]))
                continue;

            closestDistance = distanceToTarget;
            best = interactions[i];
            
        }

        return best;
    }

    private bool CanPotentiallyInteract(I_Interactable interactable)
    {
        if (!interactable.CanBeInteracted())
            return false;

        InteractionMountingRequirement mountingRequirement = interactable.GetMountingRequirement();

        if (mountingRequirement == InteractionMountingRequirement.OnlyMounted)
            return Refs.turtle.isMounted;
        if (mountingRequirement == InteractionMountingRequirement.OnlyNotMounted)
            return !Refs.turtle.isMounted;

        return true;
    }

}
