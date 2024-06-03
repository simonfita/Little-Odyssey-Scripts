using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    public NPC talkNPC;

    public Transform resetPos;


    private void Awake()
    {
        talkNPC.speech.OnDialogueEnd += ResetPosition;
    }

    private void ResetPosition()
    {
        Refs.turtle.Mount();
        Refs.turtle.Teleport(resetPos.position);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (talkNPC != null)
            talkNPC.StartDialogue();
        else
            ResetPosition();

    }
}
