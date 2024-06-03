using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_ObjectLabelDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    private void Start()
    {
        transform.SetParent(Refs.ui.labelsCanvas,false);
    }

    public void SetText(string _text)
    {
        text.text = _text;

    }
}
