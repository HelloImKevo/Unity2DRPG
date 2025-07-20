using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MerchantSlot : UI_ItemSlot
{
    public UISlotType slotType;

    private Inventory_Merchant merchant;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        bool rightButton = eventData.button == PointerEventData.InputButton.Right;
        bool leftButton = eventData.button == PointerEventData.InputButton.Left;

        if (UISlotType.PlayerSlot == slotType)
        {
            if (rightButton)
            {
                bool sellFullStack = Input.GetKey(KeyCode.LeftControl);
                merchant.TrySellItem(itemInSlot, sellFullStack);
            }
            else if (leftButton)
            {
                base.OnPointerDown(eventData);
            }

        }
        else if (UISlotType.MerchantSlot == slotType)
        {
            if (leftButton) return; // Left click does nothing

            bool buyFullStack = Input.GetKey(KeyCode.LeftControl);
            merchant.TryBuyItem(itemInSlot, buyFullStack);
        }

        ui.itemTooltip.HideTooltip();
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        ui.itemTooltip.ShowTooltip(true, rect, itemInSlot, slotType, true);
    }

    public void SetupMerchantSlotUI(Inventory_Merchant merchant) => this.merchant = merchant;
}
