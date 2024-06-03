using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Readable : MonoBehaviour
{
    public TMP_Text text;

    public void OpenReadable(string _text)
    {
        //text.text = _text;
    }

    public void CloseReadable()
    {
        Refs.ui.panels.CloseStaticPanel();
    }

    public void OpenKickstarter()
    {
        Application.OpenURL("https://www.kickstarter.com/projects/simonfita/little-odyssey?ref=bmt82j");
    }
}
