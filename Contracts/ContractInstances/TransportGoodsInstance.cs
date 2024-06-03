using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransportGoodsInstance : ContractInstanceBase
{
    public LocationSettlement fromCity, toCity;
    private Stockpile from, to;
    public int minRandomAmount, maxRandomAmount;

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
        }

        from.contractStarted = true;
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

        to.OnLeft -= LeftBox;

        from.contractStarted = false;
        to.contractStarted = false;

        base.OnFinish();

        contractGoods.Clear(); //has to be after base., reward based on this
    }

    public override bool TryCleanupAndSetup()
    {
        //resseting from and to stockpiles
        if (!TryRestoreStockpiles())
            return false;

        if (!base.TryCleanupAndSetup())
            return false;

        #region Try to find new from and to stockpiles
        Stockpile newFrom = null;
        foreach (Stockpile stock in fromCity.settlementStockpiles)
        {
            if (stock.contractState == StockpileContractState.Free)
            {
                newFrom = stock;
                break;
            }
        }
        Stockpile newTo = null;
        foreach (Stockpile stock in toCity.settlementStockpiles)
        {
            if (stock.contractState == StockpileContractState.Free)
            {
                newTo = stock;
                break;
            }
        }
        #endregion

        if (newFrom == null || newTo == null)
            return false;

        from = newFrom;
        from.contractState = StockpileContractState.FromStockpile;
        from.ClearGoods();
        from.goodsType = goodsPrefab.type;
        
        to = newTo;
        to.contractState = StockpileContractState.ToStockpile;
        to.ClearGoods();
        to.goodsType = goodsPrefab.type;

        int randomAmount = Random.Range(minRandomAmount, maxRandomAmount + 1);

        for (int i = 0; i < randomAmount; i++)
        {
            Goods goods = Instantiate(goodsPrefab);
            from.LeaveIn(goods);
            contractGoods.Add(goods);

        }

        return true;

    }

    private bool TryRestoreStockpiles()
    {
        if (from == null && to == null)
            return true;       

        Vector3 playerPos = Refs.player.transform.position;
        if (from != null && Vector3.Distance(playerPos, from.transform.position) > 50)
        {
            from.ClearGoods();
            from.contractState = StockpileContractState.Free;
            from = null;
        }
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

        return "Transport " + contractGoods.Count 
            + " " + goodsPrefab.type.ToString() 
            + " from " + fromCity.locationName 
            + " to " + toCity.locationName;
    }

    public override int GetReward()
    {
        return Mathf.RoundToInt(contractGoods.Count * 1 * rewardMultiplier * firstTimeBonus * randomBonus);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(fromCity.GetPosition(), 5);
        Gizmos.DrawLine(fromCity.GetPosition(), toCity.GetPosition());
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(toCity.GetPosition(), 7);
    }

    public override void OnSave(Save save)
    {
        CustomSave custom = new CustomSave();

        if (from != null)
            custom.fromGUID = from.GetComponent<GUID>().guid;
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
        if (GUIDManager.TryGetGUID(custom.fromGUID, out var guid))
        {
            Stockpile stockpile = guid.GetComponent<Stockpile>();
            from = stockpile;
            from.contractState = StockpileContractState.FromStockpile;
            from.ClearGoods();
            from.goodsType = goodsPrefab.type;
        }

        if (GUIDManager.TryGetGUID(custom.toGUID, out guid))
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
        public string toGUID, fromGUID;
        public List<string> stockpilesGUID = new List<string>();
        public bool playerHeld;
    }
}
