using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Backpack : MonoBehaviour
{
    public List<Transform> rows;

    public UI_Coins playerCoins;

    public AudioSource openSound, closeSound, coinsSound;

    private void Start()
    {
        Refs.inventory.itemsChanged += UpdateItems;
        
        UpdateItems(Refs.inventory.items);

        Refs.inventory.playerMoney.amountChanged += (Money money) => { playerCoins.UpdateAmount(money); coinsSound.Play(); };
        Refs.inventory.playerMoney.amount += 3;
    }

    private void UpdateItems(List<ItemAndAmount> items)
    {
        openSound.Play();

        List<List<ItemAndAmount>> sortedItems = SortItems(items);

        for (int i = 0; i < rows.Count; i++)
        {
            for (int j = 0; j < rows[i].childCount; j++)
            {
                UI_BackpackItem itm = rows[i].GetChild(j).GetComponent<UI_BackpackItem>();

                if (sortedItems[i].Count > j)
                {
                    itm.SetItem(sortedItems[i][j]);
                }
                else
                {
                    itm.SetItem(null);
                }

            }

        }


    }

    private List<List<ItemAndAmount>> SortItems(List<ItemAndAmount> items)
    {
        List<List<ItemAndAmount>> itemsByRow = new List<List<ItemAndAmount>>();

        //populate lists
        foreach (Transform row in rows)
        {
            itemsByRow.Add(new List<ItemAndAmount>());
        }



        foreach (ItemAndAmount item in items)
        {

            AddItemLexically(itemsByRow[item.item.row], item);

        }

        return itemsByRow;
    }

    private void AddItemLexically(List<ItemAndAmount> row, ItemAndAmount item)
    {
            
        int i = 0;
        while (i < row.Count && item.item.displayData.displayName.CompareTo(row[i].item.displayData.displayName) == 1)
        {
            i++;
        }
        row.Insert(i, item);
    }
}
