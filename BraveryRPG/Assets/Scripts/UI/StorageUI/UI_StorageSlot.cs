using UnityEngine;
using UnityEngine.EventSystems;

public class UI_StorageSlot : UI_ItemSlot
{
    public enum StorageSlotType
    {
        StorageSlot,
        PlayerInventorySlot
    }

    public StorageSlotType slotType;
    private Inventory_Storage storage;

    public void SetStorage(Inventory_Storage storage) => this.storage = storage;

    public override void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        // TODO: Use old Unity Input system as a temporary Proof-of-Concept.
        // This will allow the player to hold LEFT CONTROL key to transfer
        // a full stack of items.
        bool transferFullStack = Input.GetKey(KeyCode.LeftControl);

        if (StorageSlotType.StorageSlot == slotType)
        {
            storage.FromStorageToPlayer(itemInSlot, transferFullStack);
        }

        if (StorageSlotType.PlayerInventorySlot == slotType)
        {
            storage.FromPlayerToStorage(itemInSlot, transferFullStack);
        }

        ui.itemTooltip.ShowTooltip(false, null);
    }
}
