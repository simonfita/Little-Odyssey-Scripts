using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Noticeboard : MonoBehaviour
{
    public Transform entriesParent;

    public UI_NoticeBoardContractEntry entryPrefab;

    public TMP_Text contractCount;

    public void OpenNoticeBoard(ContractInstanceBase[] contracts)
    {
        Refs.gamepadController.RequestPointingMode("Noticeboard");

        contractCount.text = Refs.contracts.currentContracts.Count + "/"+TransporterLicences.GetMaxContractAmount();

        foreach (ContractInstanceBase contract in contracts)
        {
            if (contract == null)
                continue;

            UI_NoticeBoardContractEntry created = Instantiate(entryPrefab,entriesParent);

            created.button.onClick.AddListener(() => SelectedContract(contract,created.button));

            UI_NoticeBoardContractEntry.NoticeBoardEntryColor contractColor = UI_NoticeBoardContractEntry.NoticeBoardEntryColor.Normal;
            if (contract.IsMainQuest())
                contractColor = UI_NoticeBoardContractEntry.NoticeBoardEntryColor.Quest;
            else if (contract.isNew)
                contractColor = UI_NoticeBoardContractEntry.NoticeBoardEntryColor.First;

            created.SetUp(contract.GetDescription(), new Money(contract.GetReward()), contractColor);
        }
    }

    private void SelectedContract(ContractInstanceBase contract, Button button)
    {
        if (Refs.contracts.AddContract(contract))
        {
            Destroy(button.gameObject);
            contractCount.text = Refs.contracts.currentContracts.Count + "/" + TransporterLicences.GetMaxContractAmount();
        }
    }

    public void CloseNoticeBoard()
    {
        Refs.gamepadController.UnrequestPointingMode("Noticeboard");

        Refs.ui.panels.CloseStaticPanel();

        for (int i = entriesParent.childCount-1; i >= 0; i--)
        {
            Destroy(entriesParent.GetChild(i).gameObject);
        }
        
        
    }
}
