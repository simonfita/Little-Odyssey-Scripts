using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Inventory : SaveableBehaviour
{
    public Money playerMoney = new Money();

    public System.Action<List<ItemAndAmount>> itemsChanged;
    public List<ItemAndAmount> items = new List<ItemAndAmount>();

    public bool HasItem(string name)
    {
        return HasItem(ItemsDatabase.GetItem(name));
    }
    public bool HasItem(Item item)
    {
        foreach (ItemAndAmount _item in items)
        {
            if (_item.item == item)
                return true;
        }
        return false;
    }

    public void AddItem(string name, int amount = 1)
    {
        AddItem(ItemsDatabase.GetItem(name), amount);
    }

    public void AddItem(Item item, int amount = 1)
    {

        if (item.unique)
        {
            if (HasItem(item) || amount != 1)
            {
                Debug.LogError("Trying to add already had UNIQUE item!");
                return;
            }
            items.Add(new ItemAndAmount() { item = item, amount = 1 });
            item.OnGet?.Invoke();
            itemsChanged?.Invoke(items);
            return;
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items[i].amount += amount;
                item.OnGet?.Invoke();
                itemsChanged?.Invoke(items);
                return;
            }
        }
        items.Add(new ItemAndAmount() { item = item, amount = amount });
        item.OnGet?.Invoke();
        itemsChanged?.Invoke(items);
        return;
    }

    public void RemoveItem(string name)
    {
        RemoveItem(ItemsDatabase.GetItem(name));
    }
    public void RemoveItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                if (items[i].amount > 1)
                    items[i].amount--;
                else
                    items.RemoveAt(i);

                item.OnLose?.Invoke();
                itemsChanged?.Invoke(items);
                return;

            }
        }

        Debug.LogError("Tried to remove not existing item!");
    }

    
    public override void OnSave(Save save)
    {
        save.money = playerMoney.amount;

        foreach (ItemAndAmount itm in items)
        {
            save.itemNames.Add(itm.item.name);
            save.itemAmounts.Add(itm.amount);
        }

    }

    public override void OnLoad(Save save)
    {
        playerMoney.amount =  save.money;
        items = new List<ItemAndAmount>();
        for (int i = 0; i < save.itemNames.Count; i++)
        {
            AddItem(ItemsDatabase.GetItem(save.itemNames[i]), save.itemAmounts[i]);
        }
    }
}
