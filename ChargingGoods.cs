using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingGoods : Goods
{
    public float requiredCharge;
    public Transform spinningMesh;
    public Vector3 spinningSpeed;

    public System.Action OnCharged;
    public AudioSource src;

    [HideInInspector]
    public bool charged;

    public float currentCharge;

    private void Awake()
    {
        OnCharged += src.Play;
    }

    private void Update()
    {
        spinningMesh.Rotate(spinningSpeed * Time.deltaTime * Mathf.Pow(currentCharge/requiredCharge,3));
        
        if (charged)
            return;

        currentCharge += Refs.turtle.controller.velocity.magnitude * Time.deltaTime;

        if (currentCharge >= requiredCharge)
        {
            charged = true;
            OnCharged?.Invoke();
        }
    }
}
