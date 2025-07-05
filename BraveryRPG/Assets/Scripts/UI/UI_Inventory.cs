using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;
    private UI_ItemSlot[] uiItemSlots;
    private UI_EquipSlot[] uiEquipSlots;

    [Tooltip("The UI_Inventory object containing all of the UI_ItemSlot children.")]
    [SerializeField] private Transform uiItemSlotsParent;
    [Tooltip("The UI_Equipment object containing all of the UI_EquipSlot children.")]
    [SerializeField] private Transform uiEquipSlotsParent;

    private void Awake()
    {
        uiItemSlots = uiItemSlotsParent.GetComponentsInChildren<UI_ItemSlot>();
        uiEquipSlots = uiEquipSlotsParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateInventorySlots();
        UpdateEquipmentSlots();
    }

    private void UpdateEquipmentSlots()
    {
        List<Inventory_EquipmentSlot> playerEquipList = inventory.equipmentList;

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
                Debug.LogWarning($"UI_Inventory.UpdateEquipmentSlots() -> Slot mismatch error -> {playerEquipSlot.slotType} does not match UI element: {uiEquipSlot.slotType}");
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

    private void UpdateInventorySlots()
    {
        List<Inventory_Item> itemsList = inventory.itemList;

        // The number of items in our inventory can be less than the number
        // of UI slots in our Inventory UI.
        for (int i = 0; i < uiItemSlots.Length; i++)
        {
            if (i < itemsList.Count)
            {
                // Update the slot with the contents of our item data.
                uiItemSlots[i].UpdateSlot(itemsList[i]);
            }
            else
            {
                // Empty item slot.
                uiItemSlots[i].UpdateSlot(null);
            }
        }
    }
}
