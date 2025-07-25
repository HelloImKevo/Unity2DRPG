using TMPro;
using UnityEngine;

public class UI_Merchant : MonoBehaviour
{
    private Inventory_Player inventory;
    private Inventory_Merchant merchant;

    [SerializeField] private TextMeshProUGUI goldText;

    [Space]
    [Tooltip("The UI_MerchantSlots object in the Merchant UI containing all of the UI_ItemSlot children.")]
    [SerializeField] private UI_ItemSlotParent merchantSlots;

    [Tooltip("The UI_InventorySlots object in the Merchant UI containing all of the UI_ItemSlot children.")]
    [SerializeField] private UI_ItemSlotParent inventorySlots;

    [Tooltip("The UI_Equipment object in the Merchant UI containing all of the UI_EquipSlot children.")]
    [SerializeField] private UI_EquipSlotParent equipSlots;

    public void SetupMerchantUI(Inventory_Merchant merchant, Inventory_Player inventory)
    {
        this.merchant = merchant;
        this.inventory = inventory;

        this.inventory.OnInventoryChange += UpdateSlotUI;
        this.merchant.OnInventoryChange += UpdateSlotUI;
        UpdateSlotUI();

        UI_MerchantSlot[] merchantSlots = GetComponentsInChildren<UI_MerchantSlot>();

        foreach (var slot in merchantSlots)
        {
            slot.SetupMerchantSlotUI(merchant);
        }
    }

    private void UpdateSlotUI()
    {
        if (inventory == null) return;

        merchantSlots.UpdateSlots(merchant.itemList);
        inventorySlots.UpdateSlots(inventory.itemList);
        equipSlots.UpdateEquipmentSlots(inventory.equipmentList);

        goldText.text = inventory.gold.ToString("N0") + "g.";
    }
}
