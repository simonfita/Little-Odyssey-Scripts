using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoCrane : MonoBehaviour, I_Interactable
{
    public Transform cargoParent;
    public ObjectLabel label;
    public AudioSource src;
    
    public bool isFull;
    public bool contractSelected;
    public bool isInteractable;

    public System.Action OnLoadedFrom, OnUnloadedTo;
    
    public HeavyCargo currentCargo;

    public LocationSettlement location;

    private void Awake()
    {
        if (location == null)
            location = GetComponentInParent<LocationSettlement>();
    }


    public HeavyCargo CreateCargo(HeavyCargo cargoPrefab)
    {
        isFull = true;
        currentCargo = Instantiate(cargoPrefab);
        currentCargo.transform.SetParent(cargoParent);
        currentCargo.transform.localPosition = Vector3.zero;
        return currentCargo;
    }

    public void LoadCargo()
    {
        if (!Refs.turtle.TryLoadHeavyCargo(currentCargo))
        {
            Refs.ui.hud.SpawnGameMessege("I need more free space!");
            return;
        }

        isFull = false;
        currentCargo = null;
        src.Play();
        OnLoadedFrom?.Invoke();     
    }

    private void UnloadCargo()
    {
        isFull = true;
        currentCargo = Refs.turtle.UnloadHeavyCargo();
        currentCargo.transform.SetParent(cargoParent);
        currentCargo.transform.localPosition = Vector3.zero;
        currentCargo.transform.localRotation = Quaternion.identity;
        src.Play();
        OnUnloadedTo?.Invoke();
    }

    public void Clear()
    {
        if (isFull)
        {
            isFull = false;
            Destroy(currentCargo.gameObject);
            currentCargo = null;

        }
    
    }

    public bool CanBeInteracted()
    {
        return isInteractable && (isFull || Refs.turtle.currentHeavyCargo!=null);
    }

    public string GetInteractionText()
    {
        return isFull ? "Load" : "Unload";
    }

    public Transform GetInteractionTransform()
    {
        return transform;
    }

    public InteractionMountingRequirement GetMountingRequirement()
    {
        return InteractionMountingRequirement.OnlyMounted;
    }

    public void Interact()
    {
        if (isFull)
            LoadCargo();
        else
            UnloadCargo();
    }
}
