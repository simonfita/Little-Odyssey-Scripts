using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContractGarden : MonoBehaviour
{
    public Waterable waterable;

    public string locationName;

    public Material dryMaterial, wateredMaterial;

    public List<Renderer> renderers;

    private void Awake()
    {
        waterable.OnWatered +=OnWatered;
        waterable.enabled = false;
    }

    public void SetWatered(bool _watered)
    {

        foreach (Renderer renderer in renderers)
        {
            //trick for specific model, change later
            Material[] mats = renderer.materials;
            mats[1] = _watered ? wateredMaterial : dryMaterial;
            mats[3] = _watered ? wateredMaterial : dryMaterial;

            renderer.materials = mats; 
        }
    }

    public void OnWatered()
    {       
        SetWatered(true);
    }
}
