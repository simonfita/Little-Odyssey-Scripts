using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BringAnimalInstance : ContractInstanceBase
{
    public Goods animalPrefab;

    public Stockpile spawningStockpile;
    private Goods animal;

    public override void OnStart(ContractColor flag)
    {       
        base.OnStart(flag);              
    }

    protected override void OnStartingNPCTalked()
    {       
        animal.label.AddContract(flagType);

        animal.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);
        spawningStockpile.contractStarted = true;      

        base.OnStartingNPCTalked();

        startingNPC.involvedInContracts.Add(this);
        startingNPC.speech.OnTalkedAboutContract += DeliveredPet;
    }

    void DeliveredPet(ContractInstanceBase contract)
    {
        if (contract != this)
            return;

        if(Refs.player.token.heldGoods == animal)
            OnFinish();
    }

    public override void OnFinish()
    {
        animal.label.RemoveContract(flagType);
        animal.GetComponent<ContractFlagMesh>().ClearFlag();
        Refs.player.token.DestroyGoods();

        startingNPC.involvedInContracts.Remove(this);
        startingNPC.speech.OnTalkedAboutContract -= DeliveredPet;

        spawningStockpile.contractStarted = false;
        spawningStockpile.contractState = StockpileContractState.Free;

        base.OnFinish();
    }

    public override bool TryCleanupAndSetup()
    {

        if (!base.TryCleanupAndSetup())
            return false;

        Goods goods = Instantiate(animalPrefab,spawningStockpile.transform.position,animalPrefab.transform.rotation);
        spawningStockpile.goodsType = animalPrefab.type;
        spawningStockpile.contractState = StockpileContractState.FromStockpile;
        spawningStockpile.LeaveIn(goods);
        animal = goods;

        return true;

    }

    public override string GetDescription()
    {
        if (state == ContractState.NPCWaiting)
            return base.GetDescription();

        return "Bring pet " //+ contractGoods.Count 
            + " " + animalPrefab.type.ToString() 
            + " to " + startingNPC.name;
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(10 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    public override string GetDialogueStartingKnot(NPC npc)
    {
        if (state == ContractState.InProgress)
        { 
            if(Refs.player.token.heldGoods == animal)
                return base.GetDialogueStartingKnot(npc) + "Deliver"; 
            else
                return base.GetDialogueStartingKnot(npc) + "Waiting";
        }

        else
            return base.GetDialogueStartingKnot(npc);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(spawningStockpile.transform.position, 5);
        Gizmos.DrawLine(spawningStockpile.transform.position, startingNPC.locations.home.GetPosition());
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startingNPC.locations.home.GetPosition(), 7);
    }


    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();

        if (animal != null)
        {
            if (animal.GetComponentInParent<Player>())
                custom.heldByPlayer = true;
            else
            {
                Stockpile stockpile = animal.GetComponentInParent<Stockpile>();
                custom.stockpile = stockpile.GetComponent<GUID>().guid;
            }
        }

        base.OnSave(save);

        GetThisSave(save).customData = JsonUtility.ToJson(custom);
    }

    public override void OnLoad(Save save)
    {
        CustomSave custom = JsonUtility.FromJson<CustomSave>(GetThisSave(save).customData);

        if (custom.heldByPlayer)
        {
            Goods goods = Instantiate(animalPrefab);
            Refs.player.token.HoldGoods(goods);
            animal = goods;
        }
        else if(GUIDManager.TryGetGUID(custom.stockpile, out var stockpileGUID))
        {          
            Stockpile stockpile = stockpileGUID.GetComponent<Stockpile>();
            Goods goods = Instantiate(animalPrefab);
            
            if (stockpile.goodsType != GoodsTypes.Other) //not turtle
            {
                stockpile.contractState = StockpileContractState.FromStockpile;
                stockpile.goodsType = animalPrefab.type;
            }

            stockpile.LeaveIn(goods);
            animal = goods;
        }

        base.OnLoad(save);
    }

    [System.Serializable]
    new class CustomSave
    {
        public bool heldByPlayer = false;
        public string stockpile = null;
    }
}
