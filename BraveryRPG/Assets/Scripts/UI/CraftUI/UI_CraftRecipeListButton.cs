using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Formerly: UI_CraftSlot
public class UI_CraftRecipeListButton : MonoBehaviour
{
    [Tooltip("Recipe preview window that shows list of materials needed to craft this item.")]
    [SerializeField] private UI_CraftPreview craftPreview;

    [Tooltip("Icon of the item that will be crafted by this recipe.")]
    [SerializeField] private Image craftItemIcon;
    [Tooltip("Label showing the display name of the item that will be crafted.")]
    [SerializeField] private TextMeshProUGUI craftItemName;

    private Item_DataSO itemToCraft;

    public void SetupRecipeButton(Item_DataSO craftData)
    {
        itemToCraft = craftData;
        craftItemIcon.sprite = craftData.itemIcon;
        craftItemName.text = craftData.itemName;
    }

    /// <summary>
    /// Invoked by the Button.OnClick() event.
    /// </summary>
    public void UpdateCraftPreview() => craftPreview.UpdateCraftPreview(itemToCraft);
}
