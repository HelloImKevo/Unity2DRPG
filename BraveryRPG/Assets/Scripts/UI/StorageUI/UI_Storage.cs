using UnityEngine;

public class UI_Storage : MonoBehaviour
{
    private Inventory_Player playerInventory;
    private Inventory_Storage storage;

    [SerializeField] private UI_ItemSlotParent storageParent;
    [SerializeField] private UI_ItemSlotParent playerInventoryParent;
    [SerializeField] private UI_ItemSlotParent materialStashParent;

    public void SetupStorageUI(Inventory_Storage storage)
    {
        this.storage = storage;
        playerInventory = storage.playerInventory;

        // Subscribe to listen to events when the Storage Inventory changes.
        storage.OnInventoryChange += UpdateUI;
        UpdateUI();

        UI_StorageSlot[] storageSlots = GetComponentsInChildren<UI_StorageSlot>();

        foreach (var slot in storageSlots)
        {
            slot.SetStorage(storage);
        }
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (storage == null) return;

        storageParent.UpdateSlots(storage.itemList);
        playerInventoryParent.UpdateSlots(playerInventory.itemList);
        materialStashParent.UpdateSlots(storage.materialStash);
    }
}
