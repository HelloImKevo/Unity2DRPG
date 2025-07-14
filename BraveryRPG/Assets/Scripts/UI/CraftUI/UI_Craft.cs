using UnityEngine;

/// <summary>
/// The Crafting window, with panels to browse recipes by category, and preview
/// the list of required materials.
/// </summary>
public class UI_Craft : MonoBehaviour
{
    [Tooltip("UI_MiniInventory object, with a UI_ItemSlotParent script, within the Crafting Window.")]
    [SerializeField] private UI_ItemSlotParent miniInventoryParent;
    private Inventory_Player playerInventory;

    private UI_CraftCategoryTabButton[] craftListButtons;
    private UI_CraftRecipeListButton[] craftSlots;
    private UI_CraftPreview craftPreviewUI;

    public void SetupCraftUI(Inventory_Storage storage)
    {
        playerInventory = storage.playerInventory;
        // Subscribe and observe for changes to the inventory.
        playerInventory.OnInventoryChange += UpdateUI;
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

    private void UpdateUI()
    {
        if (playerInventory == null)
        {
            Debug.LogWarning("UI_Craft.UpdateUI() -> playerInventory reference is null - did you call SetupCraftUI?");
        }
        // Update the mini-inventory that is visible within the Crafting Window.
        miniInventoryParent.UpdateSlots(playerInventory.itemList);
    }
}
