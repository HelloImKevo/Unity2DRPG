using System;

[Serializable]
public class Inventory_EquipmentSlot
{
    public ItemType slotType;
    public Inventory_Item equippedItem;

    /// <summary>
    /// Null, if the slot is empty or the equipment has been removed.
    /// </summary>
    public Inventory_Item GetEquippedItem() => equippedItem;

    /// <summary>
    /// Checks whether this is an empty slot, or whether an item has been equipped in it.
    /// </summary>
    public bool HasItem() => equippedItem != null && equippedItem.itemData != null;
}
