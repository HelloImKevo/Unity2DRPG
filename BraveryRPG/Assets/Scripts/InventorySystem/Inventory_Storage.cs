using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory_Storage : Inventory_Base
{
    public Inventory_Player playerInventory { get; private set; }
    public List<Inventory_Item> materialStash;

    public void SetInventory(Inventory_Player inventory) => this.playerInventory = inventory;

    public List<Inventory_Item> GetItemsInStorage() => itemList;

    /// <summary>
    /// Consume ingredients from the player inventory, storage and stash,
    /// create the item, and then add it to the player inventory.
    /// </summary>
    public void CraftItem(Inventory_Item itemToCraft)
    {
        ConsumeMaterials(itemToCraft);
        playerInventory.AddItem(itemToCraft);
    }

    public bool CanCraftItem(Inventory_Item itemToCraft)
    {
        return HasEnoughMaterials(itemToCraft)
            && playerInventory.CanAddItem(itemToCraft);
    }

    private void ConsumeMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredIngredient in itemToCraft.itemData.craftRecipe)
        {
            int amountToConsume = requiredIngredient.RequiredQuantity;

            // Consume items from the player's inventory first, to try and
            // free up available space for the crafted item.
            amountToConsume -= ConsumedMaterialsAmount(
                playerInventory.itemList, requiredIngredient
            );

            // Then try to consume ingredients from the player's Storage.
            if (amountToConsume > 0)
            {
                amountToConsume -= ConsumedMaterialsAmount(
                    GetItemsInStorage(), requiredIngredient
                );
            }

            // Lastly, try to consume ingredients from the Stash.
            if (amountToConsume > 0)
            {
                amountToConsume -= ConsumedMaterialsAmount(
                    materialStash, requiredIngredient
                );
            }
        }
    }

    private int ConsumedMaterialsAmount(List<Inventory_Item> itemList, Inventory_RecipeIngredient ingredient)
    {
        int amountNeeded = ingredient.RequiredQuantity;
        int consumedAmount = 0;

        for (int i = itemList.Count - 1; i >= 0; i--)
        {
            var item = itemList[i];

            if (item.itemData != ingredient.itemData) continue;

            int removeAmount = Mathf.Min(item.stackSize, amountNeeded - consumedAmount);
            item.stackSize -= removeAmount;
            consumedAmount += removeAmount;

            if (item.stackSize <= 0)
            {
                // C# requires indexed-based iteration to safely remove
                // elements from a collection, while iterating it.
                itemList.RemoveAt(i);
            }

            if (consumedAmount >= amountNeeded) break;
        }

        return consumedAmount;
    }

    public bool HasEnoughMaterials(Inventory_Item itemToCraft)
    {
        foreach (var requiredMaterial in itemToCraft.itemData.craftRecipe)
        {
            if (GetAvailableAmountOf(requiredMaterial.itemData) < requiredMaterial.RequiredQuantity)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Calculate the total number of materials (items) that the player has across
    /// all inventory options (player inventory and stored stash items).
    /// </summary>
    public int GetAvailableAmountOf(Item_DataSO requiredItem)
    {
        int amount = 0;

        foreach (var item in playerInventory.itemList)
        {
            if (item.itemData == requiredItem)
            {
                amount += item.stackSize;
            }
        }

        foreach (var item in itemList)
        {
            if (item.itemData == requiredItem)
            {
                amount += item.stackSize;
            }
        }

        foreach (var item in materialStash)
        {
            if (item.itemData == requiredItem)
            {
                amount += item.stackSize;
            }
        }

        return amount;
    }

    public void AddMaterialToStash(Inventory_Item itemToAdd)
    {
        // Debug.Log($"Inventory_Storage.AddMaterialToStash() -> {gameObject.name} storing Material: {itemToAdd.itemData.itemName}");

        var stackableItem = StackableInStash(itemToAdd);

        if (stackableItem != null)
        {
            stackableItem.AddStack();
        }
        else
        {
            var newItemToAdd = new Inventory_Item(itemToAdd.itemData);
            materialStash.Add(newItemToAdd);
        }

        TriggerUpdateUI();
        // Sort Materials By Name.
        materialStash = materialStash.OrderBy(item => item.itemData.name).ToList();
    }

    public Inventory_Item StackableInStash(Inventory_Item itemToAdd)
    {
        return materialStash.Find(
            item => item.itemData == itemToAdd.itemData && item.CanAddStack()
        );
    }

    public void FromPlayerToStorage(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        // If we are transferring the whole stack, traverse each item instance
        // in the Stack and transfer it. Otherwise, just transfer one instance
        // of the item using 1 loop iteration.
        for (int i = 0; i < transferAmount; i++)
        {
            if (CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                // Remove item from Player Inventory.
                playerInventory.RemoveOneItem(item);
                // Transfer the item to this Storage.
                AddItem(itemToAdd);
            }
        }

        TriggerUpdateUI();
    }

    public void FromStorageToPlayer(Inventory_Item item, bool transferFullStack)
    {
        int transferAmount = transferFullStack ? item.stackSize : 1;

        for (int i = 0; i < transferAmount; i++)
        {
            if (playerInventory.CanAddItem(item))
            {
                var itemToAdd = new Inventory_Item(item.itemData);

                // Remove item from this Storage.
                RemoveOneItem(item);
                // Transfer the item to Player Inventory.
                playerInventory.AddItem(itemToAdd);
            }
        }

        TriggerUpdateUI();
    }

    #region ISaveable

    // TODO: This logic is quite similar to the Inventory_Player.cs - we can
    // revisit this and consolidate the duplicate behavior to adhere to DRY.
    public override void SaveData(ref GameData data)
    {
        base.SaveData(ref data);

        data.storageItems.Clear();

        // Save items in persistent "player stash" storage.
        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveId;

                if (!data.storageItems.ContainsKey(saveId))
                {
                    // Assign a quantity (stack) of zero.
                    data.storageItems[saveId] = 0;
                }

                data.storageItems[saveId] += item.stackSize;
            }
        }

        data.storageMaterials.Clear();

        // Save items in the separate materials stash.
        foreach (var item in materialStash)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveId;

                if (!data.storageMaterials.ContainsKey(saveId))
                {
                    // Assign a quantity (stack) of zero.
                    data.storageMaterials[saveId] = 0;
                }

                data.storageMaterials[saveId] += item.stackSize;
            }
        }
    }

    public override void LoadData(GameData data)
    {
        base.LoadData(data);

        itemList.Clear();
        materialStash.Clear();

        foreach (var entry in data.storageItems)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning($"{GetType().Name}.LoadData() -> Item Database" +
                                 $" may be outdated! Item not found in Storage: {saveId}");
                continue;
            }

            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.storageMaterials)
        {
            string saveId = entry.Key;
            int stackSize = entry.Value;

            Item_DataSO itemData = itemDatabase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning($"{GetType().Name}.LoadData() -> Item Database" +
                                 $" may be outdated! Item not found in Materials Stash: {saveId}");
                continue;
            }

            // This is necessary to respect stack size rules, otherwise the Load Data
            // system would permit equipment to be stacked, and potions to be stacked
            // over the limit (like 500 potions in one stack).
            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new(itemData);
                AddMaterialToStash(itemToLoad);
            }
        }
    }

    #endregion
}
