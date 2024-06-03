using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DreamPlayer : MonoBehaviour
{
    public float playerSpeed;

    private DreamControls controls;
    private void Awake()
    {
        controls = new DreamControls();
        controls.Enable();
    }

    private void Update()
    {
        float input = controls.DreamMovement.Move.ReadValue<float>();
        float movement = input * playerSpeed * Time.deltaTime;

        transform.localPosition = new Vector2(Mathf.Clamp(transform.localPosition.x + movement,-400,400), transform.localPosition.y);
    }
}
