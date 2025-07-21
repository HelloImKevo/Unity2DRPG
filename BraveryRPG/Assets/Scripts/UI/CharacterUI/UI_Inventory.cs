using TMPro;
using UnityEngine;

public class UI_Inventory : MonoBehaviour
{
    private Inventory_Player inventory;

    [Tooltip("The UI_Inventory object containing all of the UI_ItemSlot children for the Inventory.")]
    [SerializeField] private UI_ItemSlotParent inventorySlotsParent;

    [Tooltip("The UI_Equipment object containing all of the UI_EquipSlot children.")]
    [SerializeField] private UI_EquipSlotParent uiEquipSlotsParent;

    [Tooltip("TextView that displays how much Gold the Player possesses.")]
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory_Player>();
        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }

    void OnEnable()
    {
        if (inventory == null) return;

        UpdateUI();
    }

    private void UpdateUI()
    {
        inventorySlotsParent.UpdateSlots(inventory.itemList);
        uiEquipSlotsParent.UpdateEquipmentSlots(inventory.equipmentList);
        goldText.text = inventory.gold.ToString("N0") + "g.";
    }
}
