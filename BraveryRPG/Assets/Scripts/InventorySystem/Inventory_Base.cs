using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour
{
    public event Action OnInventoryChange;

    protected Player player;

    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new();

    protected virtual void Awake()
    {
        player = GetComponent<Player>();
    }

    public void TryUseItem(Inventory_Item itemToUse)
    {
        Inventory_Item consumable = itemList.Find(item => item == itemToUse);

        if (consumable == null) return;

        if (!consumable.itemEffect.CanBeUsed(player)) return;

        consumable.itemEffect.ExecuteEffect();

        if (consumable.stackSize > 1)
        {
            consumable.RemoveStack();
        }
        else
        {
            RemoveOneItem(consumable);
        }

        OnInventoryChange?.Invoke();
    }

    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        bool hasStackable = FindStackableWithSpace(itemToAdd) != null;
        return hasStackable || itemList.Count < maxInventorySize;
    }

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
        Debug.Log($"Inventory_Base.AddItem() -> Item added to {gameObject.name}: {itemToAdd.itemData.itemName}");

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

    public void RemoveOneItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize > 1)
        {
            itemInInventory.RemoveStack();
        }
        else
        {
            itemList.Remove(itemToRemove);
        }

        OnInventoryChange?.Invoke();
    }

    public void RemoveFullStack(Inventory_Item itemToRemove)
    {
        for (int i = 0; i < itemToRemove.stackSize; i++)
        {
            RemoveOneItem(itemToRemove);
        }
    }

    public Inventory_Item FindItem(Item_DataSO itemData)
    {
        return itemList.Find(item => item.itemData == itemData);
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();
}
