using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;

    [Tooltip("The UI_Inventory object containing all of the UI_ItemSlot children for the Inventory.")]
    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;

    [Tooltip("The UI_Equipment object containing all of the UI_EquipSlot children.")]
    [SerializeField] private UI_EquipSlotParent uiEquipSlotsParent;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        uiEquipSlotsParent.UpdateEquipmentSlots(inventory.equipmentList);
    }
}
