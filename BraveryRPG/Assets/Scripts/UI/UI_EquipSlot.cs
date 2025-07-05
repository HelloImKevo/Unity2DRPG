using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Backed by data companion: <see cref="Inventory_EquipmentSlot"/>.
/// </summary>
public class UI_EquipSlot : UI_ItemSlot
{
    public ItemType slotType;

    private void OnValidate()
    {
        gameObject.name = "UI_EquipmentSlot - " + slotType.ToString();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        Debug.Log($"UI_EquipSlot.OnPointerDown() -> Unequipping item: {itemInSlot.itemData?.name}");

        inventory.UnequipItem(itemInSlot);
    }
}
