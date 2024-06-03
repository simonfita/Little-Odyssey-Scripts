using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


public enum CameraModes
{ 
    Player,
    Turtle,
    Dialogue,
    TopDown,
    Custom

}

public class PlayerCamera : MonoBehaviour
{
    [Header("Components")]
    public Camera cameraComponent;

    public PlayerCameraLookAtPoint lookAtPoint;
    public Transform turtleCenterPoint;
    public Volume postProcess;
    public UnityEngine.UI.Image blackScreen;
    
    public CameraModes cameraMode { get; private set; }

    [Header("Player Camera")]
    [SerializeField]
    private Vector3 closePlayerPoint;
    [SerializeField]
    private Vector3 farPlayerPoint;
    public float moveSpeed;
    

    [Header("Turtle Camera")]
    public float turtleCameraOffset = 3;
    public float turtleMaxAdditionalOffset = 3;
    private float yaw;
    private float pitch;
    public float minPitch, maxPitch;
    public bool invertX, invertY;
    public float camSpeed = 1;

    [Header("Dialogue Camera")]
    public Vector3 offset;

    [Header("Zoom ")]
    public float zoomSpeed;

    [Header("TopDown")]
    public List<Transform> topDowns;
    public bool inTopDownView { get { return cameraMode == CameraModes.TopDown; } }

    public float culDistance;

    private float currentZoom = -1; //from -1 to 1
    private Transform player;

    private void Start()
    {
        float[] distances = new float[32];
        distances[7] = culDistance;
        cameraComponent.layerCullDistances = distances;
      
        player = Refs.player.transform;

        transform.position = player.position; //for development
    }


    private void Update()
    {

        ChangeZoom(Refs.controls.Other.Zoom.ReadValue<float>());

        if (Refs.controls.Other.MoveCamera.IsPressed()|| (Refs.gamepadController.usingGamepad && !Refs.gamepadController.pointingMode))
        {
            Vector2 mouseInput = Refs.controls.Movement.Camera.ReadValue<Vector2>() * new Vector2(invertX ? -1 : 1, invertY ? -1 : 1) * camSpeed;
            yaw += mouseInput.x * 0.3f;
            pitch = Mathf.Clamp(pitch + mouseInput.y * 0.1f, minPitch, maxPitch);
            if (mouseInput.sqrMagnitude > 0.1f)
                Refs.ui.inputHints.CompletedAction("Camera");

            if(!Refs.gamepadController.usingGamepad)
                Cursor.visible = false;

        }
        else if(!Refs.gamepadController.usingGamepad)
        {
            Cursor.visible = true;
        }

        if (Refs.controls.UI.Map.WasPerformedThisFrame() && cameraMode != CameraModes.Dialogue)
        {
            Refs.ui.inputHints.CompletedAction("Map");

            if (cameraMode == CameraModes.TopDown)
                cameraMode = Refs.turtle.isMounted ? CameraModes.Turtle : CameraModes.Player;
            else
                cameraMode = CameraModes.TopDown;
        
        }
    }
    //THIS SCRIPT HAS CHANGED EXECUTION ORDER SO IT'S EXECUTED AFTER PlayerCameraLookAtPoint
    public void LateUpdate()
    {
        Vector3 desiredPos = GetDesiredPosition();

        transform.position = Vector3.MoveTowards(transform.position, desiredPos, moveSpeed * Time.smoothDeltaTime);

        if (cameraMode == CameraModes.TopDown)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, topDowns[(int)RegionBounds.GetPlayerRegion()].rotation, Time.deltaTime*100);
            return;
        }

        Quaternion desiredRot = Quaternion.LookRotation(lookAtPoint.transform.position - transform.position);
        transform.rotation = desiredRot;     
    }

    private Vector3 GetDesiredPosition()
    {
        Vector3 position = Vector3.zero;

        switch (cameraMode)
        {
            case CameraModes.Player:
                return GetDesiredPositionPlayer();
            case CameraModes.Turtle:
                return GetDesiredPositionTurtle();
            case CameraModes.Dialogue:
                return GetDesiredPositionDialogue();
            case CameraModes.TopDown:
                return topDowns[(int)RegionBounds.GetPlayerRegion()].position;
        
        }
        return position;
    
    }
    private Vector3 GetDesiredPositionPlayer()
    {
        Vector3 offset = Vector3.Lerp(closePlayerPoint, farPlayerPoint, (currentZoom + 1) / 2);
        Refs.player.walkRot = Quaternion.AngleAxis(yaw+Refs.turtle.transform.rotation.eulerAngles.y + 180, Vector3.up);
        offset = Quaternion.AngleAxis(yaw + Refs.turtle.transform.rotation.eulerAngles.y + 180, Vector3.up) * offset;
        return player.position + offset;
    }

    private Vector3 GetDesiredPositionTurtle()
    {
        float rot = yaw + Refs.turtle.transform.rotation.eulerAngles.y+180;

        rot *= Mathf.Deg2Rad;

        float offset = turtleCameraOffset + Mathf.Lerp(0, turtleMaxAdditionalOffset, (currentZoom + 1) / 2);

        Vector3 offsetPos = new Vector3(Mathf.Sin(rot),0,Mathf.Cos(rot))* offset;

        offsetPos.y = pitch;       
        return turtleCenterPoint.position + offsetPos+Vector3.up*5; 

    }

    private Vector3 GetDesiredPositionDialogue()
    {
        Vector3 dir = Refs.player.transform.position - Refs.player.talkedToNPC.transform.position;
        dir.y = 0;
        dir.Normalize();

        Vector3 rotatedOffset =  Quaternion.LookRotation(dir, Vector3.up) * offset;
        return Refs.player.talkedToNPC.transform.position + rotatedOffset;
    }

        

    private void ChangeZoom(float value)
    {
        if (Mathf.Abs(value) > 0.1f)
            Refs.ui.inputHints.CompletedAction("Zoom");

        currentZoom = Mathf.Clamp(currentZoom+value * zoomSpeed, -1,  1);

    }

    public void ChangeCameraMode(CameraModes newMode, bool teleport)
    {
        cameraMode = newMode;
        currentZoom = -1;
        if (teleport)
        {
            TeleportToDesired();
        }
        Refs.playerCamera.SetUseDOF(newMode != CameraModes.TopDown && newMode != CameraModes.Dialogue);
    }

    public void TeleportToDesired()
    {
        transform.position = GetDesiredPosition();
        transform.rotation = Quaternion.LookRotation(lookAtPoint.transform.position - transform.position);
        lookAtPoint.TeleportToDesired();
    }


    public IEnumerator Fade(float fadeTime,float blackTime, System.Action onBlack)
    {


        for (float i = 0; i < fadeTime/2; i+=Time.deltaTime)
        {
            blackScreen.color = new Color(0,0,0, Mathf.Lerp(0, 1, i / (fadeTime / 2)));
            yield return new WaitForEndOfFrame();
        }
        blackScreen.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(blackTime/2);
        onBlack?.Invoke();
        yield return new WaitForSeconds(blackTime/2);

        for (float i = 0; i < fadeTime / 2; i += Time.deltaTime)
        {
            blackScreen.color = new Color(0, 0, 0, Mathf.Lerp(1, 0, i / (fadeTime / 2)));
            yield return new WaitForEndOfFrame();
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        yield break;
    }
    public void FadeOneParam(float time)
    {
        StartCoroutine(Fade(time * (2 / 3), time * (1 / 3), null));
    }

    public void SetUseDOF(bool use)
    {
        postProcess.profile.TryGet<DepthOfField>(out var dof);
        dof.active = use;
    }

}
