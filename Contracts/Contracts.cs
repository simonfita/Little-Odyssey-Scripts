using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ContractColor
{ 
    RedContract,
    GreenContract,
    BlueContract,
    YellowContract,
    PurpleContract
}

public class Contracts : MonoBehaviour
{
    public Action OnContractsUpdated;
    public Action<ContractInstanceBase> OnContractCompleted;

    public List<ContractInstanceBase> currentContracts = new List<ContractInstanceBase>();
    private static Dictionary<ContractColor,Material> contractMaterials;

    private List<ContractColor> availableContractFlags = new List<ContractColor>();

    private void Awake()
    {
        contractMaterials = new Dictionary<ContractColor, Material>();
        //fill array with all contract types
        foreach (ContractColor flag in Enum.GetValues(typeof(ContractColor)))
        {
            availableContractFlags.Add(flag);

            Material mat = Resources.Load<Material>(Enum.GetName(typeof(ContractColor), flag));
            contractMaterials[flag] = mat;
            
        }        

    }

    public bool AddContract(ContractInstanceBase _contract) //called normally
    {
        return AddContract(_contract,availableContractFlags[0]);
    }

    public bool AddContract(ContractInstanceBase _contract, ContractColor forceColor) //called by saves
    {
        if (currentContracts.Count >= TransporterLicences.GetMaxContractAmount())
        {
            Refs.ui.hud.SpawnGameMessege("I can't have more contracts with current licence!");
            return false;
        }

        currentContracts.Add(_contract);
        _contract.OnFinished += AwardContract;

        _contract.OnStart(forceColor);
        availableContractFlags.Remove(forceColor);

        OnContractsUpdated?.Invoke();

        return true;
    }


    private void AwardContract(ContractInstanceBase _contract)
    {
        _contract.OnFinished -= AwardContract;

        availableContractFlags.Insert(0,_contract.flagType);
        currentContracts.Remove(_contract);
        Refs.inventory.playerMoney.amount += _contract.GetReward();
        Refs.ui.hud.contractSummary.ShowReward(_contract);
        
        OnContractCompleted?.Invoke(_contract);
        OnContractsUpdated?.Invoke();


    }

    public static string GetFlagGlyph(ContractColor flag)
    {
        string returns = "<sprite=\"Flags\" index=0 ";

        switch (flag)
        {
            case ContractColor.RedContract:
                returns += "color=#FF45458F>";
                break;
            case ContractColor.BlueContract:
                returns += "color=#45A8FF8F>";
                break;
            case ContractColor.GreenContract:
                returns += "color=#45FF838F>";
                break;
            case ContractColor.YellowContract:
                returns += "color=#FFD0458F>";
                break;
            case ContractColor.PurpleContract:
                returns += "color=#A945FF8F>";
                break;
        }

        return returns;
    
    }

    public static Material GetMaterialForContractColor(ContractColor color)
    {
        return contractMaterials[color];
    }

    public static ContractInstanceBase GetContract(string contractName)
    {

        foreach (ContractInstanceBase contract in GameObject.FindObjectsOfType<ContractInstanceBase>())
        {
            if (contract.gameObject.name == contractName)
                return contract;
        }
        Debug.LogError("Can't find contract: " + contractName);
        return null;
    }
    
}
