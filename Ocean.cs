using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    public float WavesHeight,WavesSpeed;

    private Vector3 defaultPosition;

    private void Awake()
    {
        defaultPosition = transform.position;
    }

    private void Update()
    {
        transform.position = defaultPosition + Vector3.up * WavesHeight*Mathf.Sin(Time.time*WavesSpeed);
    }
}
