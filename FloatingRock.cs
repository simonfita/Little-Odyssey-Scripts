using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingRock : MonoBehaviour, IDudResponder
{
    public void OnDuded()
    {
        RandomizeRotation();
    }

    private void Awake()
    {
        RandomizeRotation();
    }

    private void RandomizeRotation()
    {
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        GetComponent<Rigidbody>().AddTorque(Random.rotation.eulerAngles * Random.Range(0f, 0.4f));
    }
}
