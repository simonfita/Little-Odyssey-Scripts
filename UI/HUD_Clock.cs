using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Clock : MonoBehaviour
{
    public TMP_Text text;
    private void Update()
    {
        int hour = ((int)WorldTime.CurrentDayTime / 100);
        int minutes = ((int)((WorldTime.CurrentDayTime % 100) * (6 / 10f))) / 30 * 30;
        text.text = hour.ToString("D2") + ":" + minutes.ToString("D2");

    }
}
