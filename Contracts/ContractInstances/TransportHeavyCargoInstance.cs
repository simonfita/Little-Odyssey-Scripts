using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportHeavyCargoInstance : ContractInstanceBase
{
    public CargoCrane fromCrane, toCrane;

    public HeavyCargo cargoPrefab;

    [HideInInspector]
    public HeavyCargo contractCargo;

    public bool neverClear;

    

    public override void OnStart( ContractColor flag)
    {       
        base.OnStart(flag);              
    }

    protected override void OnStartingNPCTalked()
    {
        toCrane.label.AddContract(flagType);
        toCrane.OnUnloadedTo += Unloaded;
        //to.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);

        contractCargo.label.AddContract(flagType);
        //contractCargo.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);


        fromCrane.isInteractable = true;
        toCrane.isInteractable = true;

        base.OnStartingNPCTalked();
    }

    void Unloaded()
    {

        //all in destination
        OnFinish();
    
    }

    public override void OnFinish()
    {
        contractCargo.label.RemoveContract(flagType);
        //contractCargo.GetComponent<ContractFlagMesh>().ClearFlag();

        //to.GetComponent<ContractFlagMesh>().ClearFlag();
        toCrane.label.RemoveContract(flagType);

        toCrane.OnUnloadedTo -= Unloaded;

        fromCrane.isInteractable = false;
        toCrane.isInteractable = false;

        fromCrane.contractSelected = false;
        toCrane.contractSelected = false;
        
        base.OnFinish();
    }

    public override bool TryCleanupAndSetup()
    {
        if (neverClear && !isNew)
            return false;

        if (!base.TryCleanupAndSetup())
            return false;

        //resseting from and to cranes
        if (!TryRestoreCranes())
            return false;   

        if (fromCrane.isFull || toCrane.isFull)
            return false;

        contractCargo = fromCrane.CreateCargo(cargoPrefab);
        fromCrane.contractSelected = true;
        toCrane.contractSelected = true;

        return true;

    }

    private bool TryRestoreCranes()
    {
        if (fromCrane.contractSelected || toCrane.contractSelected)
            return false;

        if (!fromCrane.isFull && !toCrane.isFull)
            return true;       

        Vector3 playerPos = Refs.player.transform.position;
        if (fromCrane.isFull && Vector3.Distance(playerPos, fromCrane.transform.position) > 50)
        {
            fromCrane.Clear();
        }
        if (toCrane.isFull && Vector3.Distance(playerPos, toCrane.transform.position) > 50)
        {
            toCrane.Clear();
        }

        return false;
    }

    public override string GetDescription()
    {
        if (state == ContractState.NPCWaiting)
            return base.GetDescription();

        return "Transport heavy cargo from "
            + fromCrane.location.locationName
            + " to " + toCrane.location.locationName;
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(5 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(fromCrane.transform.position, 5);
        Gizmos.DrawLine(fromCrane.transform.position, toCrane.transform.position);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(toCrane.transform.position, 7);
    }

    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();

        if (contractCargo != null)
        {
            custom.onTurtle = contractCargo.GetComponentInParent<Turtle>() != null;
        }
        base.OnSave(save);

        GetThisSave(save).customData = JsonUtility.ToJson(custom);
    }

    public override void OnLoad(Save save)
    {
        CustomSave custom = JsonUtility.FromJson<CustomSave>(GetThisSave(save).customData);

        if ((ContractState)GetThisSave(save).contractState != ContractState.Finished)
        {
            contractCargo = fromCrane.CreateCargo(cargoPrefab);
            fromCrane.contractSelected = true;
            toCrane.contractSelected = true;

            if ((ContractState)GetThisSave(save).contractState == ContractState.InProgress && custom.onTurtle)
            {
                fromCrane.LoadCargo();
            }
        }
        base.OnLoad(save);
    }

    [System.Serializable]
    new class CustomSave
    {
        public bool onTurtle = false;
    }
}
