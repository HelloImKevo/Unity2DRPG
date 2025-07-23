using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Base : MonoBehaviour, ISaveable
{
    public event Action OnInventoryChange;

    protected Player player;

    public int maxInventorySize = 10;
    public List<Inventory_Item> itemList = new();

    [Header("ITEM DATABASE")]
    [Tooltip("The 'ITEM DATABASE' scriptable object containing all item data unity objects.\n\nSee ItemList_DataSO.CollectItemsData() for more details.")]
    [SerializeField] protected ItemList_DataSO itemDatabase;

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

    public void AddItem(Inventory_Item itemToAdd)
    {
        // Debug.Log($"Inventory_Base.AddItem() -> Item added to {gameObject.name}: {itemToAdd.itemData.itemName}");

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

    public Inventory_Item FindStackableWithSpace(Inventory_Item itemToFind)
    {
        // Find list of existing stackable items in the inventory, and
        // identify which stackable inventory elements have room for more stacks.
        return itemList.Find(
            item => item.itemData == itemToFind.itemData && item.CanAddStack()
        );
    }

    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public Inventory_Item FindSameItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item.itemData == itemToFind.itemData);
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();

    #region ISaveable

    public virtual void LoadData(GameData data)
    {
        // Override in subclasses to load game data.
    }

    public virtual void SaveData(ref GameData data)
    {
        // Override in subclasses to save game data.
    }

    #endregion
}
