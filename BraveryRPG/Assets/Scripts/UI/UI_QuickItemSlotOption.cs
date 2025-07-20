using UnityEngine.EventSystems;

/// <summary>
/// Floating options which depict items that can be assigned to the Quick Item slot.
/// </summary>
public class UI_QuickItemSlotOption : UI_ItemSlot
{
    private UI_QuickItemSlot currentQuickItemSlot;

    public void SetupOption(UI_QuickItemSlot currentQuickItemSlot, Inventory_Item itemToSet)
    {
        this.currentQuickItemSlot = currentQuickItemSlot;
        UpdateSlot(itemToSet);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        // TODO: Call base.super()?
        currentQuickItemSlot.SetupQuickSlotItem(itemInSlot);
        ui.inGameUI.HideQuickItemOptions();
    }
}
