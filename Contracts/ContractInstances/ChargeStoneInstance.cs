using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeStoneInstance : ContractInstanceBase
{
    public ChargingGoods stonePrefab;
    public Stockpile spawnStockpile;
    public Well wellToCharge;

    private ChargingGoods stone;


    public override void OnStart( ContractColor flag)
    {
        base.OnStart(flag);      
    }

    protected override void OnStartingNPCTalked()
    {      
        base.OnStartingNPCTalked();
        stone = Instantiate(stonePrefab);
        spawnStockpile.goodsType = stonePrefab.type;
        spawnStockpile.contractState = StockpileContractState.FromStockpile;
        spawnStockpile.contractStarted = true;
        spawnStockpile.LeaveIn(stone);


        stone.OnCharged += () => { Refs.contracts.OnContractsUpdated.Invoke(); };

        startingNPC.involvedInContracts.Add(this);
        startingNPC.speech.OnTalkedAboutContract += DeliveredStone;
    }

    private void DeliveredStone(ContractInstanceBase contract)
    {
        if (contract != this)
            return;
        
        if (stone.charged && Refs.player.token.heldGoods == stone)
            OnFinish();

    }

    public override void OnFinish()
    {
        startingNPC.involvedInContracts.Remove(this);
        startingNPC.speech.OnTalkedAboutContract -= DeliveredStone;

        Refs.player.token.DestroyGoods();

        spawnStockpile.contractStarted = false;
        spawnStockpile.contractState = StockpileContractState.Free;


        if (wellToCharge != null)
        {
            wellToCharge.UnlockCallbacking();
            Refs.inventory.AddItem("CallbackStone");
        }
        
        base.OnFinish();
    }

    public override bool TryCleanupAndSetup()
    {
        stone = null;

        if (!base.TryCleanupAndSetup())
            return false;

        return true;

    }

    public override string GetDialogueStartingKnot(NPC npc)
    {
        if (state == ContractState.InProgress)
        {
            if (stone.charged && Refs.player.token.heldGoods == stone)
                return base.GetDialogueStartingKnot(npc) + "Deliver";
            else
                return base.GetDialogueStartingKnot(npc) + "Waiting";
        }

        else
            return base.GetDialogueStartingKnot(npc);
    }


    public override string GetDescription()
    {
        if (state == ContractState.NPCWaiting)
            return base.GetDescription();
        if(stone != null && stone.charged)
            return "Return stone";
        return "Charge stone by walking";
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(3 * rewardMultiplier * firstTimeBonus * randomBonus);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
    }

    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();

        if (stone != null)
        {
            Stockpile stockpile = stone.GetComponentInParent<Stockpile>();
            if (stockpile != null)
                custom.stockpileGUID =stockpile.GetComponent<GUID>().guid;
            else if (stone.GetComponentInParent<Player>())
                custom.playerHeld = true;

            custom.chargeAmount = stone.currentCharge;
        }

        base.OnSave(save);

        GetThisSave(save).customData = JsonUtility.ToJson(custom);
    }

    public override void OnLoad(Save save)
    {

        base.OnLoad(save);
        spawnStockpile.ClearGoods();

        CustomSave custom = JsonUtility.FromJson<CustomSave>(GetThisSave(save).customData);


        if (GUIDManager.TryGetGUID(custom.stockpileGUID, out var guid))
        {
            Stockpile stockpile = guid.GetComponent<Stockpile>();

            stone = Instantiate(stonePrefab);
            stone.currentCharge = custom.chargeAmount;
            if (stockpile.goodsType != GoodsTypes.Other) //not turtle
            {
                stockpile.goodsType = stonePrefab.type;
            }
            stockpile.LeaveIn(stone);
        }
        else if (custom.playerHeld)
        {
            stone = Instantiate(stonePrefab);
            stone.currentCharge = custom.chargeAmount;
            Refs.player.token.HoldGoods(stone);
        }

 
    }

    [System.Serializable]
    new class CustomSave
    {
        public string stockpileGUID;
        public bool playerHeld;
        public float chargeAmount;
    }
}
