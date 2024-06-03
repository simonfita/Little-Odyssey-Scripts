using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windmill : MonoBehaviour,IDudResponder
{
    public Transform blades;

    [Range(1,30)]
    public float defaultRotSpeed;
    public float maxRotSpeed;

    private float rotSpeed;

    private void Awake()
    {
        rotSpeed = defaultRotSpeed;
    }

    private void Update()
    {
        blades.Rotate(Vector3.forward, Time.deltaTime*rotSpeed,Space.Self);

        rotSpeed = Mathf.Max(defaultRotSpeed, rotSpeed - Time.deltaTime*10);
    }

    public void OnDuded()
    {
        rotSpeed = Mathf.Min(maxRotSpeed, rotSpeed + 30);
    }
}
