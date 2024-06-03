using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TokenSlot : MonoBehaviour, I_Interactable
{
    public Transform slotPoint, exitSpot;
    private Vector3 startingSlotLocalPosition;

    public Action OnEntered;
    public Action OnExited;

    public bool canPlayerUse = true;
    public float jumpInTime;
    public float jumpInHeight;
    
    private Token tokenInSlot;


    private void Awake()
    {
        startingSlotLocalPosition = slotPoint.transform.localPosition;
    }

    private void OnInteract()
    {
        if (!Refs.player.token.inSlot && tokenInSlot == null)
        {
            EnterSlot(Refs.player.token);
        }
        else
        {
            ExitSlot();
        }
    }

    public void EnterSlot(Token token)
    {
        tokenInSlot = token;

        StartCoroutine(JumpOnAnimation());

        token.EnterSlot(slotPoint);
        OnEntered?.Invoke();
    }

    public void ExitSlot()
    {
            StartCoroutine(JumpOffAnimation());

    }

    private IEnumerator JumpOnAnimation()
    {

        Vector3 startingPosition = slotPoint.parent.InverseTransformPoint(tokenInSlot.transform.position);
        Vector3 endPosition = startingSlotLocalPosition;

        for (float i = 0; i <= jumpInTime; i += Time.deltaTime)
        {

            Vector3 dist = Vector3.Lerp(startingPosition,endPosition, i / jumpInTime);
            Vector3 additionalHeight = jumpInHeight* Vector3.up * (float)Math.Sin(i / jumpInTime *Math.PI);
            additionalHeight = slotPoint.parent.InverseTransformVector(additionalHeight);

            slotPoint.localPosition = dist + additionalHeight;

            yield return new WaitForEndOfFrame();
        }
        slotPoint.localPosition = endPosition;
        yield break;
    }

    private IEnumerator JumpOffAnimation()
    {
        float jumpOffSpeed = jumpInTime / 2;

        Vector3 startingPosition = startingSlotLocalPosition;
        Vector3 endPosition = slotPoint.parent.InverseTransformPoint(exitSpot.position);;

        for (float i = 0; i <= jumpOffSpeed; i += Time.deltaTime)
        {

            Vector3 dist = Vector3.Lerp(startingPosition, endPosition, i / jumpOffSpeed);
            Vector3 additionalHeight = jumpInHeight/2 * Vector3.up * (float)Math.Sin(i / jumpOffSpeed * Math.PI);
            additionalHeight = slotPoint.parent.InverseTransformVector(additionalHeight);

            slotPoint.localPosition = dist + additionalHeight;

            yield return new WaitForEndOfFrame();
        }
        slotPoint.localPosition = endPosition;

        tokenInSlot.ExitSlot(exitSpot.position);

        OnExited?.Invoke();

        tokenInSlot = null;

        slotPoint.localPosition = startingPosition;
        yield break;
    }


    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.OnlyNotMounted; }

    public bool CanBeInteracted() { return enabled && canPlayerUse; }

    public void Interact() { OnInteract(); }

    public string GetInteractionText() { return "Enter"; }

    public Transform GetInteractionTransform() { return transform; }
}
