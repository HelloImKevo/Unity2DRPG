using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour
{
    public event Action OnInventoryChange;

    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new();

    protected virtual void Awake()
    {
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumable = itemList.Find(item => item == itemToUse);

        if (consumable == null) return;

        consumable.itemEffect.ExecuteEffect();

        if (consumable.stackSize > 1)
        {
            consumable.RemoveStack();
        }
        else
        {
            RemoveItem(consumable);
        }

        OnInventoryChange?.Invoke();
    }

    public bool CanAddItem() => itemList.Count < maxInventorySize;

    public Inventory_Item FindStackableWithSpace(Inventory_Item itemToFind)
    {
        // Find list of existing stackable items in the inventory.
        List<Inventory_Item> stackableItems = itemList.FindAll(
            item => item.itemData == itemToFind.itemData
        );

        // Identify which stackable inventory elements have room for more stacks.
        foreach (var stackableItem in stackableItems)
        {
            if (stackableItem.CanAddStack())
            {
                return stackableItem;
            }
        }

        return null;
    }

    public void AddItem(Inventory_Item itemToAdd)
    {
        Debug.Log("Inventory_Base.AddItem() -> Item added to inventory: " + itemToAdd.itemData.itemName);

        Inventory_Item existingStackable = FindStackableWithSpace(itemToAdd);

        if (existingStackable != null)
        {
            existingStackable.AddStack();
        }
        else
        {
            itemList.Add(itemToAdd);
        }

        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Inventory_Item itemToRemove)
    {
        itemList.Remove(itemToRemove);
        OnInventoryChange?.Invoke();
    }

    public Inventory_Item FindItem(Item_DataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }

    // public void TriggerUpdateUI() => OnInventoryChange?.Invoke();
}
