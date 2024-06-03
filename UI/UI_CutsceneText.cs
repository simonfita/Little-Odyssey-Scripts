using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CutsceneText : MonoBehaviour
{
    public RectTransform borderTransform;
    public TMPro.TMP_Text text;

    public Vector3 hiddenPosition, shownPosition;

    private string desiredText;
    private bool shouldBeHidden = true;

    
    public void ShowText(string text)
    { 
        desiredText = text;
    }

    public void HideText()
    {
        shouldBeHidden = true;
        desiredText = null;
    }

    private void Update()
    {
        if (shouldBeHidden)
        {
            if (IsFullyHidden())
            {
                if (desiredText != null)
                {
                    shouldBeHidden = false;
                    text.text = desiredText;
                }
            }
            else
            {

                borderTransform.localPosition = Vector3.MoveTowards(borderTransform.localPosition, hiddenPosition, Time.deltaTime*500);
            }
        }
        else
        {
            if (IsFullyShown())
            {
                if (text.text != desiredText)
                    shouldBeHidden = true;
            }
            else
            {
                borderTransform.localPosition = Vector3.MoveTowards(borderTransform.localPosition, shownPosition, Time.deltaTime * 500);
            }
        }
    }

    private bool IsFullyHidden()
    {
        return borderTransform.localPosition == hiddenPosition;
    }

    private bool IsFullyShown()
    {
        return borderTransform.localPosition == shownPosition;
    }
}
