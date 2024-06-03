using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLabel : MonoBehaviour
{
    private UI_ObjectLabelDisplay label;

    private List<ContractColor> contracts = new List<ContractColor>();
    private bool isMainQuest;


    private void CreateLabelIfNull()
    {
        if (label != null)
            return;
        GameObject displayLabelPrefab = Resources.Load<GameObject>("ObjectLabel");
        label = Instantiate(displayLabelPrefab).GetComponent<UI_ObjectLabelDisplay>();
        label.gameObject.GetComponent<UI_WorldToScreenLabel>().toFollow = transform;
    }

    public void SetContracts(List<ContractColor> colors)
    {
        contracts = colors;
        UpdateText();
    }

    public void AddContract(ContractColor color)
    {
        contracts.Add(color);
        UpdateText();
    }

    public void RemoveContract(ContractColor color)
    {
        contracts.Remove(color);
        UpdateText();
    }

    public void SetIsMainQuest(bool isMainQuest)
    {
        this.isMainQuest = isMainQuest;
        UpdateText();
    }

    private void UpdateText()
    {
        CreateLabelIfNull();


        string text = "";

        if (isMainQuest)
            text += "<sprite=\"ImportantMark\" index=0>";

        foreach (ContractColor contract in contracts)
        {
            text += Contracts.GetFlagGlyph(contract);
        }

        label.SetText(text);
    }

    private void OnEnable()
    {
        CreateLabelIfNull();
        label.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        if (label != null) 
        label.gameObject.SetActive(false);
    }
}
