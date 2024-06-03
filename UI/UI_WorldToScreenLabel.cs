using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_WorldToScreenLabel : MonoBehaviour
{
    public Transform toFollow;

    public Vector2 screenOffset;

    public MonoBehaviour rend;

    private void Update()
    {
        if (toFollow == null)
        {
            rend.enabled = false;
            return;
        }

        Vector3 offset = screenOffset;

        Vector3 position = Refs.playerCamera.cameraComponent.WorldToScreenPoint(toFollow.position) + offset;

        transform.position = position;

        rend.enabled = (position.z < 100 && position.z > 0) || Refs.playerCamera.inTopDownView;

    }
}
