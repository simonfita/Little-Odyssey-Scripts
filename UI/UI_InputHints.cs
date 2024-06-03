using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_InputHints : MonoBehaviour
{
    [System.Serializable]
    public struct InputHintsStage
    {
        public string action;
        public string actionTwo;
        public string text;
    }

    public InputHintsStage[] stages;

    private int currentStage = 0;

    public UI_InputFormatter text;


    private void Awake()
    {
        UpdateText();
    }

    private void UpdateText()
    { 
        text.text = stages[currentStage].text;
        text.actions[0] = stages[currentStage].action;
        text.actions[1] = stages[currentStage].actionTwo;
    }

    public void CompletedAction(string action)
    {
        if (!gameObject.activeSelf)
            return;

        if (stages[currentStage].action != action)
            return;
        currentStage++;
        if (stages[currentStage].action == "END")
        {
            Deactivate();
            return;
        }

        UpdateText();
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
