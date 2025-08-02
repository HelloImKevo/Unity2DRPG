using UnityEngine.EventSystems;

public class UI_QuestRewardSlot : UI_ItemSlot
{
    public override void OnPointerDown(PointerEventData eventData)
    {
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        ui.itemTooltip.ShowTooltip(true, rect, itemInSlot, UISlotType.None, false);
    }
}
