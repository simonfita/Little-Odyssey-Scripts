using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Coins : MonoBehaviour
{
    public TMP_Text goldText, silverText, bronzeText;

    public void UpdateAmount(Money money)
    {
        goldText.text = money.gold + "";
        silverText.text = money.silver + "";
        bronzeText.text = money.bronze + "";
    }
}
