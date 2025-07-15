using System.Collections.Generic;
using UnityEngine;

public class UI_EquipSlotParent : MonoBehaviour
{
    private UI_EquipSlot[] uiEquipSlots;

    public void UpdateEquipmentSlots(List<Inventory_EquipmentSlot> playerEquipList)
    {
        if (uiEquipSlots == null)
        {
            uiEquipSlots = GetComponentsInChildren<UI_EquipSlot>();
        }

        for (int i = 0; i < uiEquipSlots.Length; i++)
        {
            // TODO: This approach will cause slots to be overridden, because you can
            // have multiple slots with the same type, for example Two Trinkets.
            // UI_EquipSlot uiEquipSlot = uiEquipSlots[i];
            // Inventory_EquipmentSlot playerEquipSlot = playerEquipList.Find(
            //     equipmentSlot => equipmentSlot.slotType == uiEquipSlot.slotType
            // );
            // if (playerEquipSlot == null)
            // {
            //     Debug.LogWarning($"UI_Inventory.UpdateEquipmentSlots() -> Slot lookup error -> No matching {uiEquipSlot.slotType} slot found in Player Equipment List - check Inventory_Player setup!");
            //     continue;
            // }

            UI_EquipSlot uiEquipSlot = uiEquipSlots[i];
            Inventory_EquipmentSlot playerEquipSlot = playerEquipList[i];

            if (playerEquipSlot.slotType != uiEquipSlot.slotType)
            {
                Debug.LogWarning($"UI_Inventory.UpdateEquipmentSlots() -> Slot mismatch error -> " +
                                 $"{playerEquipSlot.slotType} does not match UI element: {uiEquipSlot.slotType}");
            }

            if (playerEquipSlot.HasItem())
            {
                // Update the slot with the contents of our equipment data.
                uiEquipSlot.UpdateSlot(playerEquipSlot.equippedItem);
            }
            else
            {
                // There is no item - empty equipment slot.
                uiEquipSlot.UpdateSlot(null);
            }
        }
    }
}
