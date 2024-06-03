using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[ExecuteAlways]
public class UI_InputFormatter : MonoBehaviour
{

    [TextArea]
    public string text;

    public TMPro.TMP_Text TMP_Text;

    public string[] actions;

    private Controls GetControls()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return new Controls();
#endif
        return Refs.controls;
    }
    private bool IsUsingGamepad()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return false;
#endif
        return Refs.gamepadController.usingGamepad;
    }

    private void Update()
    {
        string modifiedText = text;

        for (int i = 0; i < actions.Length; i++)
        {
            string actionKey = IsUsingGamepad() ? GetActionGamepadKey(GetControls().FindAction(actions[i])) : GetActionKeyboardKey(GetControls().FindAction(actions[i]));
            modifiedText = modifiedText.Replace("{" + i + "}", actionKey);
        }


        modifiedText = modifiedText.Replace("/ ", "");
        modifiedText = modifiedText.Replace("+ ", "");


        TMP_Text.text = modifiedText;
    }

    private string GetActionKeyboardKey(InputAction action)
    {
        string returns = "";
        foreach (var binding in action.bindings)
        {
            if (binding.path.Contains("Keyboard")|| binding.path.Contains("Mouse"))
                returns += string.Format("<sprite=\"KeyboardKeys\" name=\"{0}\">", binding.path.Substring(binding.path.IndexOf("/") + 1));
        }
        return returns;
    }
    private string GetActionGamepadKey(InputAction action)
    {
        string returns = "";
        foreach (var binding in action.bindings)
        {
            if (binding.path.Contains("Gamepad"))
                returns+= string.Format("<sprite=\"KeyboardKeys\" name=\"{0}\">" ,binding.path.Substring(binding.path.IndexOf("/")+1));
        }
        return returns;
    }
}
