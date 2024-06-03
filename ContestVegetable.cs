using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContestVegetable : MonoBehaviour
{
    public Waterable waterable;

    public float maxScale;

    public CargoCrane crane;

    private void Awake()
    {
        waterable.OnWatered += OnWatered;
    }

    private void OnWatered()
    {
        transform.localScale *= 1.3f;
    }

    private void Update()
    {
        if (!waterable.enabled && transform.localScale.x < maxScale && Vector3.Distance(transform.position, Refs.player.transform.position) > 50)
        {
            waterable.enabled = true;
        }

    }
}
