using UnityEngine;

/// <summary>
/// The Crafting window, with panels to browse recipes by category, and preview
/// the list of required materials.
/// </summary>
public class UI_Craft : MonoBehaviour
{
    [SerializeField] private UI_ItemSlotParent inventoryParent;
    private Inventory_Player inventory;

    private UI_CraftCategoryTabButton[] craftListButtons;
    private UI_CraftRecipeListButton[] craftSlots;
    private UI_CraftPreview craftPreviewUI;

    private void Awake()
    {
        SetupCraftListButtons();
    }

    public void SetupCraftUI(Inventory_Storage storage)
    {
        inventory = storage.playerInventory;
        inventory.OnInventoryChange += UpdateUI;
        UpdateUI();

        craftPreviewUI = GetComponentInChildren<UI_CraftPreview>(true);
        craftPreviewUI.SetupCraftPreview(storage);
        SetupCraftListButtons();
    }

    private void SetupCraftListButtons()
    {
        craftListButtons = GetComponentsInChildren<UI_CraftCategoryTabButton>(true);
        craftSlots = GetComponentsInChildren<UI_CraftRecipeListButton>(true);

        foreach (var slot in craftSlots)
        {
            slot.gameObject.SetActive(false);
        }

        foreach (var button in craftListButtons)
        {
            button.SetCraftSlots(craftSlots);
        }
    }

    private void UpdateUI() => inventoryParent.UpdateSlots(inventory.itemList);
}
