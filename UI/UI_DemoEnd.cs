using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_DemoEnd : MonoBehaviour
{
    private float delay = 0;
    void Update()
    {
        delay += Time.deltaTime;
        if (delay > 3f && (Keyboard.current.anyKey.wasPressedThisFrame || Gamepad.current.buttonSouth.wasPressedThisFrame))
            Application.Quit();
        
    }
}
