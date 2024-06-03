using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waterable : MonoBehaviour, I_Interactable
{
    public float waterRequired;
    private float currentWater;
    public float leftWater { get { return waterRequired - currentWater; } }
    public float wateringSpeed;
    public GameObject wateringCan;
    public AudioSource src;
    public Quaternion startCanRotation, endCanRotation;
    public System.Action OnWatered;

    public bool CanBeInteracted()
    {
        return enabled;
    }

    public string GetInteractionText()
    {
        return "Water";
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
        if (!Refs.inventory.HasItem("WateringCan"))
        {
            Refs.ui.hud.SpawnGameMessege("If I only had something to water it with...");
            return;
        }
        if (Refs.turtle.hydration.currentHydration < leftWater)
        {
            Refs.ui.hud.SpawnGameMessege("I don't have enough water!");
            return;
        }

        StartCoroutine(WateringProcess());
        

    }

    private IEnumerator WateringProcess()
    {
        wateringCan.SetActive(true);
        src.Play();
        wateringCan.transform.rotation = startCanRotation;
        while (Refs.controls.Other.Interaction.IsPressed())
        {
            currentWater += Time.deltaTime * wateringSpeed;
            Refs.turtle.hydration.currentHydration -= Time.deltaTime * wateringSpeed;
            wateringCan.transform.rotation = Quaternion.RotateTowards(wateringCan.transform.rotation, endCanRotation, Time.deltaTime * 20);
            if (leftWater <= 0)
            {
                Water();
                wateringCan.SetActive(false);
                src.Stop();
                yield break;
            }
            yield return null;
        }
        wateringCan.SetActive(false);
        src.Stop();
        yield break;
    }

    private void Water()
    {
        currentWater = 0;
        enabled = false;
        OnWatered?.Invoke();
    }

}
