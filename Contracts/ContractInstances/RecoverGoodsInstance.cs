using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverGoodsInstance : ContractInstanceBase
{
    public LocationSettlement  toCity;
    private Stockpile to;
    public int minRandomAmount, maxRandomAmount;
    public List<Stockpile> spawnPoints;

    public Goods goodsPrefab;

    private List<Goods> contractGoods = new List<Goods>();

    public override void OnStart(ContractColor flag)
    {       
        base.OnStart(flag);              
    }

    protected override void OnStartingNPCTalked()
    {       
        to.label.AddContract(flagType);
        to.OnLeft += LeftBox;
        to.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);
        foreach (Goods goods in contractGoods)
        {
            goods.label.AddContract(flagType);
            goods.GetComponent<ContractFlagMesh>().SetFlagColor(flagType);
            goods.GetComponentInParent<Stockpile>().contractStarted = true;
        }

        to.contractStarted = true;

        base.OnStartingNPCTalked();
    }

    void LeftBox(Goods goods)
    {
        List<Goods> transported = to.GetGoods();

        foreach (Goods _good in contractGoods)
        {
            if (!transported.Contains(_good))
                return;
        }

        //all in destination
        OnFinish();
    
    }

    public override void OnFinish()
    {
        foreach (Goods goods in contractGoods)
        {
            goods.label.RemoveContract(flagType);
            goods.GetComponent<ContractFlagMesh>().ClearFlag();
        }
        to.GetComponent<ContractFlagMesh>().ClearFlag();
        to.label.RemoveContract(flagType);

        foreach (Stockpile stock in spawnPoints)
        { 
            stock.contractStarted = false;
            stock.contractState = StockpileContractState.Free;
        }

        to.OnLeft -= LeftBox;

        to.contractStarted = false;
        
        base.OnFinish();
    }

    public override bool TryCleanupAndSetup()
    {
        //resseting from and to stockpiles
        if (!TryRestoreStockpile())
            return false;

        if (!base.TryCleanupAndSetup())
            return false;

        Stockpile newTo = null;
        foreach (Stockpile stock in toCity.settlementStockpiles)
        {
            if (stock.contractState == StockpileContractState.Free)
            {
                newTo = stock;
                break;
            }
        }
        
        if (newTo == null)
            return false;

        foreach (Stockpile stock in spawnPoints)
            stock.ClearGoods();

        contractGoods.Clear();
        
        to = newTo;
        to.contractState = StockpileContractState.ToStockpile;
        to.ClearGoods();
        to.goodsType = goodsPrefab.type;

        int randomAmount = Random.Range(minRandomAmount, maxRandomAmount + 1);

        spawnPoints.Reverse(); // TODO: randomize

        for (int i = 0; i < randomAmount; i++)
        {

            Goods goods = Instantiate(goodsPrefab);
            spawnPoints[i].goodsType = goodsPrefab.type;
            spawnPoints[i].contractState = StockpileContractState.FromStockpile;
            spawnPoints[i].LeaveIn(goods);
            contractGoods.Add(goods);

        }

        return true;

    }

    private bool TryRestoreStockpile()
    {
        if (to == null)
            return true;       

        Vector3 playerPos = Refs.player.transform.position;
        if (to != null && Vector3.Distance(playerPos, to.transform.position) > 50)
        {
            to.ClearGoods();
            to.contractState = StockpileContractState.Free;
            to = null;
        }

        return false;
    }

    public override string GetDescription()
    {
        if (state == ContractState.NPCWaiting)
            return base.GetDescription();

        return "Recover " + contractGoods.Count 
            + " " + goodsPrefab.type.ToString() 
            + " to " + toCity.locationName;
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(contractGoods.Count * 1 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawSphere(fromCity.GetPosition(), 5);
        //Gizmos.DrawLine(fromCity.GetPosition(), toCity.GetPosition());
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(toCity.GetPosition(), 7);
    }

    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();
        if (to != null)
            custom.toGUID = to.GetComponent<GUID>().guid;

        foreach (var good in contractGoods)
        {
            Stockpile stockpile = good.GetComponentInParent<Stockpile>();
            if (stockpile != null)
                custom.stockpilesGUID.Add(stockpile.GetComponent<GUID>().guid);
            else if (good.GetComponentInParent<Player>())
                custom.playerHeld = true;

        }


        base.OnSave(save);

        GetThisSave(save).customData = JsonUtility.ToJson(custom);
    }

    public override void OnLoad(Save save)
    {
        contractGoods.Clear();

        CustomSave custom = JsonUtility.FromJson<CustomSave>(GetThisSave(save).customData);

        if (GUIDManager.TryGetGUID(custom.toGUID, out GUID guid))
        {
            Stockpile stockpile = guid.GetComponent<Stockpile>();
            to = stockpile;
            to.contractState = StockpileContractState.ToStockpile;
            to.ClearGoods();
            to.goodsType = goodsPrefab.type;
        }

        foreach (string _guid in custom.stockpilesGUID)
        {
            if (GUIDManager.TryGetGUID(_guid, out guid))
            {
                Stockpile stockpile = guid.GetComponent<Stockpile>();
                Goods goods = Instantiate(goodsPrefab);
                
                if (stockpile.goodsType != GoodsTypes.Other) //not turtle
                {
                    stockpile.goodsType = goodsPrefab.type;
                    if (stockpile.contractState == StockpileContractState.Free)
                        stockpile.contractState = StockpileContractState.FromStockpile;
                }

                stockpile.LeaveIn(goods);
                contractGoods.Add(goods);
            }
        }
        if (custom.playerHeld)
        {
            Goods goods = Instantiate(goodsPrefab);
            Refs.player.token.HoldGoods(goods);
            contractGoods.Add(goods);
        }

        base.OnLoad(save);
    }

    [System.Serializable]
    new class CustomSave
    {
        public string toGUID;
        public List<string> stockpilesGUID = new List<string>();
        public bool playerHeld;
    }
}
