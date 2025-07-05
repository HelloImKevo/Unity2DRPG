using System.Collections.Generic;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    private Player player;

    [Tooltip("Equipment that the player is currently wearing.")]
    public List<Inventory_EquipmentSlot> equipmentList;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

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

        UnequipItem(itemToUnequip, slotToReplace != null);
        EquipItem(inventoryItem, slotToReplace);
    }

    private void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        Debug.Log($"EquipItem() -> Equipping: {itemToEquip.itemData?.itemName} in Slot: {slot.slotName} - Unequipping: ");

        // float savaedHealthPercent = player.Health.GetHealthPercent();

        slot.equippedItem = itemToEquip;
        slot.equippedItem.AddModifiers(player.Stats);
        // slot.equipedItem.AddItemEffect(player);

        // player.Health.SetHealthToPercent(savaedHealthPercent);
        RemoveItem(itemToEquip);
    }

    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (!CanAddItem() && !replacingItem)
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

        itemToUnequip.RemoveModifiers(player.Stats);
        // itemToUnequip.RemoveItemEffect();

        player.Health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }
}
