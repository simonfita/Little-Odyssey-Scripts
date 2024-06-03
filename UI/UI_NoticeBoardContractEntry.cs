using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_NoticeBoardContractEntry : MonoBehaviour
{
    public enum NoticeBoardEntryColor
    { 
        Normal,
        First,
        Quest
    }

    public TMP_Text descriptionText;
    public UI_Coins coins;
    public Image image;
    public Button button;

    public Color normalColor, firstColor, questColor;


    public void SetUp(string description, Money reward, NoticeBoardEntryColor color)
    {
        descriptionText.text = description;
        coins.UpdateAmount(reward);
        switch (color)
        {
            case NoticeBoardEntryColor.Normal:
                image.color = normalColor;
                break;
            case NoticeBoardEntryColor.First:
                image.color = firstColor;
                break;
            case NoticeBoardEntryColor.Quest:
                image.color = questColor;
                descriptionText.text = "<sprite=\"ImportantMark\" index=0>" + description;
                break;

        }
    
    }

}
