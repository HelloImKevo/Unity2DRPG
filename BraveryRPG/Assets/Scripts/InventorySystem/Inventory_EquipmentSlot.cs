using System;
using UnityEngine;

/// <summary>
/// Equipment slot component in the player object inventory data collection.
/// 
/// Pairs with visual companion: <see cref="UI_EquipSlot"/>.
/// </summary>
[Serializable]
public class Inventory_EquipmentSlot
{
    [Tooltip("Name identifier for this equipment slot - mainly for debugging.")]
    public string slotName;
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
