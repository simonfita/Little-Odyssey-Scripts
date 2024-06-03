using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Well : MonoBehaviour, I_Interactable
{

    public AudioSource src;

    public float hydratingSpeed;

    public Transform callbackPoint;
    public GameObject callbackStone;

    public WorldRegion region;

    private bool drinking;

    public bool CanBeInteracted()
    {
        return true;
    }

    public string GetInteractionText()
    {
        return "Drink";
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
        if (!Refs.turtle.isMounted)
        {
            Refs.ui.hud.SpawnGameMessege("I should water my Turtle here!");
            return;
        }

        drinking = true;
        Refs.turtle.anim.SetBool("Drinking", true);
        Refs.turtle.Unmount();

        Vector3 dir = transform.position - Refs.turtle.transform.position;
        Refs.turtle.transform.forward = dir;

        src.Play();
    }

    private void Awake()
    {
        callbackStone.SetActive(false);
    }

    private void Update()
    {
        if (!drinking)
            return;

        Refs.turtle.hydration.currentHydration = Mathf.Min(Refs.turtle.hydration.maxHydration, Refs.turtle.hydration.currentHydration + Time.deltaTime * hydratingSpeed);

        if (Refs.turtle.isMounted)
        {
            drinking = false;
            Refs.turtle.anim.SetBool("Drinking", false);
        }
    }

    public void UnlockCallbacking()
    {
        callbackStone.SetActive(true);
    }

    public Vector3? GetCallbackPosition()
    {
        if (!callbackStone.activeSelf)
        {          
            return null;
        }
        return callbackPoint.position;

    }
}
