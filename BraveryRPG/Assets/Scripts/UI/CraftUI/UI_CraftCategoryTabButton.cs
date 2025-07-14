using UnityEngine;

public class UI_CraftCategoryTabButton : MonoBehaviour
{
    [Tooltip("Name of this crafting category with recipes / blueprints. Example: 'Weapons' or 'Trinkets'")]
    [SerializeField] private string categoryName;
    [Tooltip("List of items or equipment that can be crafted for this category.")]
    [SerializeField] private ItemList_DataSO craftData;

    private UI_CraftRecipeListButton[] craftRecipeSlots;

    public void SetCraftSlots(UI_CraftRecipeListButton[] craftRecipeSlots)
        => this.craftRecipeSlots = craftRecipeSlots;

    /// <summary>
    /// Invoked by the Button.OnClick() event.
    /// </summary>
    public void UpdateCraftSlots()
    {
        if (craftData == null)
        {
            Debug.LogWarning("You need to assign craft list data!");
            return;
        }

        // Traverse the slots (buttons) in our ListView and enable
        // the appropriate number of visual list elements.
        foreach (var slot in craftRecipeSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < craftData.itemList.Length; i++)
        {
            Item_DataSO itemData = craftData.itemList[i];

            craftRecipeSlots[i].gameObject.SetActive(true);
            craftRecipeSlots[i].SetupRecipeButton(itemData);
        }
    }
}
