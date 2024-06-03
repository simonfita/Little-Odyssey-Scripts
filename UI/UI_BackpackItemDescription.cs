using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_BackpackItemDescription : MonoBehaviour
{
    public TMP_Text itemName, itemDescription;

    private RectTransform trans;

    private void Awake()
    {
        trans = GetComponent<RectTransform>();

        HideItemLabel();
    }

    public void SetItemLabel(Item itm)
    {
        gameObject.SetActive(true);
        itemName.text = itm.displayData.displayName;
        itemDescription.text = itm.displayData.displayDescription;
        MoveToMousePosition();
    }
    public void HideItemLabel()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        MoveToMousePosition();
    }

    private void MoveToMousePosition()
    {
        Vector2 offset = new Vector2(trans.rect.width / 2, -trans.rect.height / 2);
        transform.position = Refs.controls.Other.MousePosition.ReadValue<Vector2>()+offset;
    }

}
