using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_BackpackItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image img;
    public Button butt;
    public TMP_Text text;

    public UI_BackpackItemDescription description;

    private ItemAndAmount item;

    public void SetItem(ItemAndAmount itemAndAmount)
    {
        item = itemAndAmount;
        gameObject.SetActive(item != null);

        if (item != null)
        {
            img.sprite = item.item.displayData.image;

            butt.interactable = item.item.clickable;

            text.enabled = !item.item.unique;
            text.text = item.amount+"";

        }
    
    }

    public void OnClick()
    {
        if (item.item.clickSound != null)
            Sounds.PlayPlayerSound(item.item.clickSound);

        item.item.OnClick.Invoke(); //can invalidate item
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        description.SetItemLabel(item.item);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        description.HideItemLabel();
    }
}
