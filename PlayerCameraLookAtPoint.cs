using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraLookAtPoint : MonoBehaviour
{
    public Transform turtlePoint;

    public float minSpeed = 1;
    public float speedMult = 1;
    public float dialogueVerticalOffset;

    private void Start()
    {
        transform.position = Refs.player.transform.position;
    }

    
    private void LateUpdate()
    {
        float dist = Vector3.Distance(transform.position, GetDesiredPosition());
        float speed = Mathf.Max(minSpeed, dist*speedMult)*Time.deltaTime;

       
        transform.position= Vector3.MoveTowards(transform.position, GetDesiredPosition(), speed);
    }

    private Vector3 GetDesiredPosition()
    {
        switch (Refs.playerCamera.cameraMode) 
        {
            case CameraModes.Player:
                return Refs.player.transform.position;
            case CameraModes.Turtle:
                return turtlePoint.position;
            case CameraModes.Dialogue:
               return Refs.player.talkedToNPC.transform.position+Vector3.up* dialogueVerticalOffset;
            case CameraModes.TopDown:
                return Refs.player.transform.position;

        }
        Debug.LogError("Not handled camera mode");
        return Vector3.zero;
    }
    public void TeleportToDesired()
    {
        transform.position = GetDesiredPosition();
    }
}
