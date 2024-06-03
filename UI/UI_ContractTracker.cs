using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_ContractTracker : MonoBehaviour
{
    public TMP_Text text;

    private void Start()
    {
        Refs.contracts.OnContractsUpdated += UpdateText;
    }

    private void UpdateText()
    {
        if (Refs.contracts.currentContracts.Count == 0) 
        {
            text.text = "Find notice board to get a contract";
            return;
        }

        string contracts = "";

        foreach (ContractInstanceBase con in Refs.contracts.currentContracts)
        {
            string txt = con.GetDescription() + Contracts.GetFlagGlyph(con.flagType) + "\n";
            if (con.IsMainQuest())
                txt = "<sprite=\"ImportantMark\" index=0> " + txt;
            contracts += txt;
        }

        text.text = contracts;

    }
}
