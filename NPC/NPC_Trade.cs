using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class NPC_Trade : MonoBehaviour
{
    [System.Serializable]
    public struct ItemOffer
    {
        public Item item;
        public int price;
        public bool onlyOnce;
    }

    public List<ItemOffer> offerts;


    public void FillVariables(Story story)
    {
        story.variablesState["firstItem"] = offerts.Count >= 1 ? offerts[0].item.name : "";
        
        story.variablesState["secondItem"] = offerts.Count >= 2 ? offerts[1].item.name : "";
        
        story.variablesState["thirdItem"] = offerts.Count >= 3 ? offerts[2].item.name : "";
    }

    public bool CanBuyItem(string id)
    {
        return offerts[IDToIndex(id)].price <= Refs.inventory.playerMoney.amount;
    }


    public void BuyItem(string id)
    {
        Refs.inventory.playerMoney.amount -= offerts[IDToIndex(id)].price;

        Refs.inventory.AddItem(offerts[IDToIndex(id)].item);

        if (offerts[IDToIndex(id)].onlyOnce)
            offerts.RemoveAt(IDToIndex(id));
    }
    public string GetItemName(string id)
    {
        return offerts[IDToIndex(id)].item.displayData.displayName;
    }

    public int GetItemPrice(string id)
    {
        return offerts[IDToIndex(id)].price;
    }

    private int IDToIndex(string id)
    {
        for (int i = 0; i < offerts.Count; i++)
        {
            if (offerts[i].item.name == id)
                return i;
        }
        Debug.LogError("Invalid bought item name!");
        return -1;
    }

}
