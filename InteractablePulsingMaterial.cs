using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(I_Interactable))]
public class InteractablePulsingMaterial : MonoBehaviour
{
    [SerializeField]
    private List<Renderer> pulseMaterials = new List<Renderer>();

    I_Interactable interactable;

    private bool _lastCanBeInteracted;
    
    void Awake()
    {
        interactable = GetComponent<I_Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        bool canBeInteracted = interactable.CanBeInteracted();
        if (_lastCanBeInteracted != canBeInteracted)
        {
            foreach (Renderer rend in pulseMaterials)
            {
                rend.material.SetInt("_ShouldPulse", canBeInteracted ? 1 : 0);
            }
            _lastCanBeInteracted = canBeInteracted;
        }
    }


    public void AddPulsingMaterial(Renderer rend)
    {
        if (pulseMaterials.Contains(rend))
            return;

        if (interactable.CanBeInteracted())
            rend.material.SetInt("_ShouldPulse", 1);
        pulseMaterials.Add(rend);
    }

    public void RemovePulsingMaterial(Renderer rend)
    {
        if (!pulseMaterials.Contains(rend))
            return;

        if (!interactable.CanBeInteracted())
            rend.material.SetInt("_ShouldPulse", 0);
        pulseMaterials.Remove(rend);
    }
}
