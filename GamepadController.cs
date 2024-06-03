using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;

public class GamepadController : MonoBehaviour
{
    public UIInputs uiControls;
    public bool usingGamepad;
    public bool pointingMode;
    InputSystemUIInputModule inputSystem;
    PlayerInput playerInput;
    private Vector2 desiredMousePos;
    public float controllerSensitivity;

    private HashSet<string> objectesRequestingPointing = new HashSet<string>();


    private void Start()
    {
        inputSystem = GetComponent<InputSystemUIInputModule>();
        playerInput = GetComponent<PlayerInput>();
       

    }

    private void Update()
    {
        CheckForGamepadUse();
        PointerWarpInput();

        //fixes button seleciton
        if (usingGamepad && EventSystem.current.currentSelectedGameObject != null)
            EventSystem.current.SetSelectedGameObject(null);

        if (pointingMode && !usingGamepad)
        {
            SetPointingMode(false);
        }
    }

    private void CheckForGamepadUse()
    {
       

        bool usingGamepadScheme = InputUser.all[0].controlScheme.Value.deviceRequirements[0].controlPath == "<Gamepad>";

        if (usingGamepad)
        {
            if (!usingGamepadScheme)
            {

                usingGamepad = false;
            }

            if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame|| Mouse.current.delta.ReadValue().magnitude>0.5f)
            {

                usingGamepad = false;
                playerInput.SwitchCurrentControlScheme("KeyboardMouse");

            }

        }
        else if (usingGamepadScheme)
        {
            usingGamepad = true;
            SetPointingMode(objectesRequestingPointing.Count > 0);
        }
    }

    private void PointerWarpInput()
    {
        if (usingGamepad && pointingMode)
            PointerMove(inputSystem.actionsAsset["PointerWarp"].ReadValue<Vector2>()*Time.unscaledDeltaTime);
    }

    public void PointerMove(Vector2 move)
    {
        if (!pointingMode || !usingGamepad)
        {
            Debug.Log("fast return");
            return;
        }
        Vector2 diff = move * controllerSensitivity;
        desiredMousePos += diff;
        Mouse.current.WarpCursorPosition(desiredMousePos);
    }

    public void PointerWarp(Vector2 pos)
    {
        if (!usingGamepad)
        {
            Debug.Log("fast return");
            return;
        }
        desiredMousePos = pos;
        Mouse.current.WarpCursorPosition(desiredMousePos);
    }


    public void RequestPointingMode(string _name)
    {
        objectesRequestingPointing.Add(_name);
        if (!pointingMode)
            SetPointingMode(true);
    }

    public void UnrequestPointingMode(string _name)
    { 
        objectesRequestingPointing.Remove(_name);
        if (pointingMode && objectesRequestingPointing.Count == 0)
        {
            SetPointingMode(false);
        }
    }

    private void SetPointingMode(bool isPointing)
    {
        
        if (!usingGamepad)
            return;

        pointingMode = isPointing;
        Cursor.visible = pointingMode;
        if (pointingMode)
        {
            inputSystem.actionsAsset["PointerWarp"].Enable();
            PointerWarp(new Vector2(Screen.width/2,Screen.height/2));
        }
        else
        {
            PointerWarp(new Vector2(Screen.width / 2, Screen.height * 0.65f));
            inputSystem.actionsAsset["PointerWarp"].Disable();
        }

    }
    private void OnDestroy()
    {
        //Refs.controls.Other.GamepadModeSwitch.performed -= GamepadModeSwitch;
    }

}
