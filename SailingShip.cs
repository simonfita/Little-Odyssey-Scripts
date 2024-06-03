using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SailingShip : MonoBehaviour, I_Interactable
{

    public ShipsManager.SailingShipType shipType;

    public bool inPort;

   // public Interactable interactable;
    public Transform sailingTransformPoint;
    public List<Transform> exitPoints;
    public int exitPointIndex;
    public TriggerCollider waitingTrigger;
    public AudioSource src;

    public bool isPlayerSailing { private set; get; }

    private void TryEnter()
    {
        if (TransporterLicences.currentLicence<TransporterLicences.LicenceType.Sailing)
            Refs.ui.hud.SpawnGameMessege("I don't have the sailing licence...");
        else if (!Refs.turtle.isMounted)
            Refs.ui.hud.SpawnGameMessege("I should get on the Turtle");
        else
            Enter();       
            
    }

    private void Enter()
    {
        isPlayerSailing = true;
        SaveSystem.DisablingFlags.Add(SaveSystem.SaveDisablingFlag.Sailing);

        Refs.turtle.Mount();
        Refs.turtle.transform.SetParent(sailingTransformPoint);
        Refs.turtle.Teleport(sailingTransformPoint.position, sailingTransformPoint.rotation);
        Refs.turtle.enabled = false;      
    }
    public void TryExit()
    {
        if (!inPort)
            return;
        
        isPlayerSailing = false;
        SaveSystem.DisablingFlags.Remove(SaveSystem.SaveDisablingFlag.Sailing);

        Refs.turtle.transform.SetParent(null);
        Refs.turtle.Teleport(exitPoints[exitPointIndex].position, exitPoints[exitPointIndex].rotation);
        Refs.turtle.enabled = true;
    }

    private void Update()
    {
        src.volume = inPort ? 0 : 1;
    }


    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.NotMountedAndMounted; }

    public bool CanBeInteracted() { return inPort; }

    public void Interact() { if (!isPlayerSailing) TryEnter(); else TryExit(); }

    public string GetInteractionText() { return isPlayerSailing ? "Exit" : "Enter"; }

    public Transform GetInteractionTransform() { return transform; }
}
