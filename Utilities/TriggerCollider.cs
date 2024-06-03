using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerCollider : MonoBehaviour
{
    public bool OnlyPlayer;

    public event Action<Collider> OnTriggerEntered;
    public event Action<Collider> OnTriggerExited;

    public bool playerIn;

    private void OnTriggerEnter(Collider other)
    {
        if (OnlyPlayer && !other.gameObject.CompareTag("Player"))
            return;

        playerIn = OnlyPlayer;
        OnTriggerEntered?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnlyPlayer && !other.gameObject.CompareTag("Player"))
            return;

        playerIn = false;
        OnTriggerExited?.Invoke(other);
    }
}
