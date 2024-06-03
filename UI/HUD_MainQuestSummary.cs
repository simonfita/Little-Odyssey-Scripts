using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD_MainQuestSummary : MonoBehaviour
{
    private const int distance = 160;
    private bool shown;
    private RectTransform rect;
    public TMPro.TMP_Text text;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        float posY = Mathf.MoveTowards(rect.anchoredPosition.y, shown ? -distance : distance, Time.deltaTime * 250);
        rect.anchoredPosition = new Vector2(0,posY);
    }

    public void Show()
    {
        shown = true;
        text.text = "<sprite=\"ImportantMark\" index=0> " + Refs.mainQuest.GetDescription();
        Invoke(nameof(Hide), 10f);
    }

    private void Hide()
    {
        shown = false;
    
    }
}
