using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum StockpileContractState
{ 
    Free,
    FromStockpile,
    ToStockpile,
    NPCStockpile,
}

public class Stockpile : MonoBehaviour, I_Interactable
{
    public GoodsTypes goodsType;
    public StockpileContractState contractState;
    public bool contractStarted;

    public ObjectLabel label;
    

    public List<Transform> itemPlaces;

    public System.Action<Goods> OnTaken, OnLeft;

    public Transform npcPoint;
    public AudioSource src;
    public AudioClip takeAudio;

    
    public bool isEmpty{get{return currentAmount<=0;}}
    public bool isFull{get{return currentAmount>=Size;}}


    [HideInInspector]
    public int Size;


    public int currentAmount { get; private set; }
    public int leftPlaces { get { return Size - currentAmount; } }

    protected virtual void Awake()
    {       
        Size = itemPlaces.Count;
        
        for (currentAmount = 0; currentAmount<Size&&itemPlaces[currentAmount].childCount!=0; currentAmount++); //count current amount on awake
       
    }

    private void OnInteract()
    {
        if (Refs.player.token.heldGoods == null)
        {
            TakeFrom(Refs.player.token);
        }
        else
        {
            Refs.player.token.LeaveGoods(this);  
        }
    }

    public void TakeFrom(Token token)
    {
        if (currentAmount <= 0)
            return;

        Goods _goods = GetGoods()[currentAmount - 1];

        if (token.HoldGoods(_goods))
        {
            currentAmount--;

            src.clip = takeAudio;
            src.Play();

            OnTaken?.Invoke(_goods);
        }

    }

    public bool LeaveIn(Goods goods)
    {
        if (currentAmount >= Size)
            return false;

        if (goods.type != goodsType && goodsType != GoodsTypes.Other)
            return false;

        goods.transform.parent = itemPlaces[currentAmount];
        goods.transform.localPosition = Vector3.zero;
        goods.transform.localRotation = Quaternion.identity;
        currentAmount++;

        src.clip = goods.leaveAudio;
        src.Play();
        OnLeft?.Invoke(goods);

        return true;
    }

    public List<Goods> GetGoods()
    {
        List<Goods> returns = new List<Goods>();

        for (int i = 0; i < currentAmount; i++)
        {
            returns.Add(itemPlaces[i].GetChild(0).GetComponent<Goods>());
        }
        
        return returns;
    }

    public void ClearGoods()
    {
        for (int i = 0; i < currentAmount; i++)
        {
           Destroy(itemPlaces[i].GetChild(0).gameObject);
        }
        currentAmount = 0;
    }

    public InteractionMountingRequirement GetMountingRequirement() { return InteractionMountingRequirement.OnlyNotMounted; }

    public bool CanBeInteracted() 
    {
        bool amountCorrect =
            (Refs.player.token.heldGoods && currentAmount != Size) || (currentAmount > 0 && !Refs.player.token.heldGoods);

        bool alwaysAvailable = goodsType == GoodsTypes.Other;        

        bool contractAvailable = 
            (contractState != StockpileContractState.Free && contractState != StockpileContractState.NPCStockpile) && contractStarted;

        return (alwaysAvailable || contractAvailable) && amountCorrect;    
    }

    public void Interact() { OnInteract(); }

    public string GetInteractionText() { return Refs.player.token.heldGoods ? "Leave" : "Take"; }

    public Transform GetInteractionTransform() { return transform; }
}
