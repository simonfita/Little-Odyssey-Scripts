using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGardenInstance : ContractInstanceBase
{
    public ContractGarden garden;

    public override void OnStart( ContractColor flag)
    {
        base.OnStart(flag);      
    }

    protected override void OnStartingNPCTalked()
    {
        garden.waterable.enabled = true;
        garden.waterable.OnWatered += OnFinish;

        garden.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);
        
        base.OnStartingNPCTalked();
    }

    public override void OnFinish()
    {
        garden.GetComponent<ContractFlagMesh>().ClearFlag();

        garden.waterable.enabled = false;
        garden.waterable.OnWatered -= OnFinish;

        base.OnFinish();
    }

    public override bool TryCleanupAndSetup()
    {
        if (!base.TryCleanupAndSetup())
            return false;

        Vector3 playerPos = Refs.player.transform.position;

        //Check distance
        if (Vector3.Distance(playerPos,garden.transform.position)<50)
            return false;

        garden.SetWatered(false);

        return true;

    }

    public override string GetDescription()
    {
        if (state == ContractState.NPCWaiting)
            return base.GetDescription();
        return "Water garden " + garden.locationName;
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(3 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    public override void OnLoad(Save save)
    {
        ContractInstanceBase.CustomSave contract = GetThisSave(save);
        
        garden.SetWatered((ContractState)contract.contractState != ContractState.Finished);

        base.OnLoad(save);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(garden.transform.position, 7);
    }
}
