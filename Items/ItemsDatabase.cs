using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemsDatabase
{
    [SerializeField]
    private static List<Item> items;


    static ItemsDatabase()
    {
        items = Resources.LoadAll<Item>("Items").ToList();
    }

    public static Item GetItem(string name)
    {
        foreach (Item item in items)
        { 
            if(item.name == name)
                return item;
        }
        Debug.LogError("Can't find item: " + name);
        return null;
    
    }
}
