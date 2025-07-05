using System.Collections.Generic;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Base inventory;
    // private Inventory_Player inventory;
    private UI_ItemSlot[] uiItemSlots;
    // private UI_EquipSlot[] uiEquipSlots;

    [Tooltip("The UI_Inventory object containing all of the UI_ItemSlot children.")]
    [SerializeField] private Transform uiItemSlotsParent;
    // [SerializeField] private Transform uiEquipSlotsParent;

    private void Awake()
    {
        uiItemSlots = uiItemSlotsParent.GetComponentsInChildren<UI_ItemSlot>();
        // uiEquipSlots = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Base>();
        // inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        UpdateInventorySlots();
        // UpdateEquipmentSlots();
    }

    // private void UpdateEquipmentSlots()
    // {
    //     List<Inventory_EquipmentSlot> playerEquipList = inventory.equipList;

    //     for (int i = 0; i < uiEquipSlots.Length; i++)
    //     {
    //         var playerEquipSlot = playerEquipList[i];

    //         if (!playerEquipSlot.HasItem())
    //         {
    //             uiEquipSlots[i].UpdateSlot(null);
    //         }
    //         else
    //         {
    //             uiEquipSlots[i].UpdateSlot(playerEquipSlot.equipedItem);
    //         }
    //     }
    // }

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
