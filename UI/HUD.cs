using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HUD : MonoBehaviour
{


    
    public Transform gameMessageSpawn;
    public GameObject automovePrompt;

    public UI_InputFormatter interactablePrompt,mountingPrompt;
    public Vector2 interactablePromptOffset;

    public HUD_HydrationMeter hydrationMeter;
    public HUD_ContractSummary contractSummary;
    public HUD_LocationDisplay locationDisplay;

    [SerializeField]
    private HUD_GameMessage gameMessagePrefab;

    private I_Interactable currentInteractable;

    private void Start()
    {
        mountingPrompt.GetComponent<UI_WorldToScreenLabel>().toFollow = Refs.turtle.transform;
    }

    private void LateUpdate()
    {
        UpdateInteractablePrompt();
        UpdateMountingPrompt();
    }

    #region Interactions
    public void SetCurrentInteractable(I_Interactable interactable)
    {
        CheckForWaterable(interactable);

        if (interactable != null)
        {
            currentInteractable = interactable;
            interactablePrompt.text = "{0} " + interactable.GetInteractionText();
        
        }
        else
            currentInteractable = null;
    }

    private void CheckForWaterable(I_Interactable interactable)
    {
        if (interactable != null && interactable.CanBeInteracted() && interactable.GetInteractionTransform().TryGetComponent<Waterable>(out Waterable waterable))
        {
            float left = (Refs.turtle.hydration.currentHydration - waterable.leftWater) /Refs.turtle.hydration.maxHydration;
            float mult = 1 - left;

            hydrationMeter.additionalImage.rectTransform.sizeDelta = new Vector2(200, mult*400);
        }
        else
            hydrationMeter.additionalImage.rectTransform.sizeDelta = new Vector2(200, 0);
    }

    private void UpdateInteractablePrompt()
    {

        if (!I_Interactable.IsValid(currentInteractable))
        {
            interactablePrompt.gameObject.SetActive(false);
            interactablePrompt.GetComponent<UI_WorldToScreenLabel>().toFollow = null;
            return;
        }

        interactablePrompt.GetComponent<UI_WorldToScreenLabel>().toFollow = currentInteractable.GetInteractionTransform();
        interactablePrompt.gameObject.SetActive(true);
           
       
    }
    #endregion

    private void UpdateMountingPrompt()
    {
        mountingPrompt.gameObject.SetActive(!Refs.turtle.isMounted && Refs.turtle.GetIsPlayerInMountingRange());
    }

    public void SpawnGameMessege(string text)
    {
        Instantiate(gameMessagePrefab,gameMessageSpawn).SetUp(text);

    }

    public void SetShowAutomovePrompt(bool show)
    {
        automovePrompt.SetActive(show);

    }

}
