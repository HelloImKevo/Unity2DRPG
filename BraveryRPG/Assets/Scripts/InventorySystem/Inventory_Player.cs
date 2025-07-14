using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public Inventory_Storage storage { get; private set; }

    [Tooltip("Equipment that the player is currently wearing.")]
    public List<Inventory_EquipmentSlot> equipmentList;

    // [Header("Quick Item Slots")]
    // public Inventory_Item[] quickItems = new Inventory_Item[2];

    [Header("Gold Info")]
    public int gold = 10000;

    protected override void Awake()
    {
        base.Awake();
        storage = FindFirstObjectByType<Inventory_Storage>();
    }

    // public void SetQuickItemInSlot(int slotNumber, Inventory_Item itemToSet)
    // {
    //     quickItems[slotNumber - 1] = itemToSet;
    //     TriggerUpdateUI();
    // }

    // public void TryUseQuickItemInSlot(int passedSlotNumber)
    // {
    //     int slotNumber = passedSlotNumber - 1;
    //     var itemToUse = quickItems[slotNumber];

    //     if (itemToUse == null)
    //         return;

    //     TryUseItem(itemToUse);

    //     if (FindItem(itemToUse) == null)
    //     {
    //         quickItems[slotNumber] = FindSameItem(itemToUse);
    //     }

    //     TriggerUpdateUI();
    //     OnQuickSlotUsed?.Invoke(slotNumber);
    // }

    public void TryEquipItem(Inventory_Item item)
    {
        var inventoryItem = FindItem(item.itemData);
        List<Inventory_EquipmentSlot> matchingSlots = equipmentList.FindAll(
            slot => slot.slotType == item.itemData.itemType
        );

        // STEP 1: Try to find empty slot and equip item
        foreach (var slot in matchingSlots)
        {
            if (!slot.HasItem())
            {
                Debug.Log($"TryEquipItem() -> Matching slot: {slot.slotName} {slot.slotType} - Equipping: {inventoryItem.itemData?.itemName}");
                EquipItem(inventoryItem, slot);
                return;
            }
        }

        if (matchingSlots.Count == 0)
        {
            Debug.LogWarning($"Inventory_Player -> TryEquipItem() -> No matching equipment slots for item: {item.itemData?.itemName}");
            return;
        }

        // STEP 2: No empty slots? Replace first one
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equippedItem;
        Debug.Log($"TryEquipItem() -> Slot to replace: {slotToReplace.slotName} - Unequipping: {itemToUnequip.itemData?.itemName}");

        // Prevents a bug where if you have a full inventory, and swap equipment
        // for items that have the same item effect, then the ItemEffect Player
        // reference will trigger a NullReference exception.
        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        Debug.Log($"EquipItem() -> Equipping: {itemToEquip.itemData?.itemName} in Slot: {slot.slotName} - Unequipping: ");

        // When equipping / unequipping items that modify the player's health,
        // we want to adjust their current health accordingly, since this is
        // a fast-paced sidescrolling platformer game; otherwise, the player
        // would have like 25% of their remaining health when equipping an
        // item that adds +250 Max Health.
        float savedHealthPercent = player.Health.GetHealthPercent();

        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.Stats);
        slot.equippedItem.AddItemEffect(player);

        player.Health.SetHealthToPercent(savedHealthPercent);
        RemoveOneItem(itemToEquip);
    }

    /// <param name="replacingItem">Part of a fix to prevent a NullReference when re-equipping
    /// items that have the same special effect, resulting in the ItemEffect.Unsubscribe
    /// function call triggering the Player reference to be nullified.</param>
    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (!CanAddItem(itemToUnequip) && !replacingItem)
        {
            Debug.Log("UnequipItem -> No space in inventory!");
            return;
        }

        float savedHealthPercent = player.Health.GetHealthPercent();
        var slotToUnequip = equipmentList.Find(slot => slot.equippedItem == itemToUnequip);

        if (slotToUnequip != null)
        {
            slotToUnequip.equippedItem = null;
        }

        // IMPORTANT: Prevents equipment modifiers from accumulating, which
        // would allow the stats to continue to increase indefinitely.
        itemToUnequip.RemoveModifiers(player.Stats);
        itemToUnequip.RemoveItemEffect();

        // It's critical that we update the Health percent AFTER removing modifiers.
        player.Health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }

    // public override void SaveData(ref GameData data)
    // {
    //     data.gold = gold;
    //     data.inventory.Clear();
    //     data.equipedItems.Clear();

    //     foreach (var item in itemList)
    //     {
    //         if (item != null && item.itemData != null)
    //         {
    //             string saveId = item.itemData.saveId;

    //             if (!data.inventory.ContainsKey(saveId))
    //             {
    //                 data.inventory[saveId] = 0;
    //             }

    //             data.inventory[saveId] += item.stackSize;
    //         }
    //     }

    //     foreach (var slot in equipList)
    //     {
    //         if (slot.HasItem())
    //         {
    //             data.equipedItems[slot.equipedItem.itemData.saveId] = slot.slotType;
    //         }
    //     }
    // }

    // public override void LoadData(GameData data)
    // {
    //     gold = data.gold;

    //     foreach (var entry in data.inventory)
    //     {
    //         string saveId = entry.Key;
    //         int stackSize = entry.Value;

    //         ItemDataSO itemData = itemDataBase.GetItemData(saveId);

    //         if (itemData == null)
    //         {
    //             Debug.LogWarning("Item not found: " + saveId);
    //             continue;
    //         }

    //         Inventory_Item itemToLoad = new Inventory_Item(itemData);

    //         for (int i = 0; i < stackSize; i++)
    //         {
    //             AddItem(itemToLoad);
    //         }
    //     }

    //     foreach (var entry in data.equipedItems)
    //     {
    //         string saveId = entry.Key;
    //         ItemType equipemntSlotType = entry.Value;

    //         Item_DataSO itemData = itemDataBase.GetItemData(saveId);
    //         Inventory_Item itemToLoad = new Inventory_Item(itemData);

    //         var slot = equipmentList.Find(
    //             slot => slot.slotType == equipemntSlotType && !slot.HasItem()
    //         );

    //         slot.equippedItem = itemToLoad;
    //         slot.equippedItem.AddModifiers(player.Stats);
    //         slot.equippedItem.AddItemEffect(player);
    //     }

    //     TriggerUpdateUI();
    // }
}
