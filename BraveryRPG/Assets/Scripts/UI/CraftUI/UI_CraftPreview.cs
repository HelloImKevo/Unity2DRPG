using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CraftPreview : MonoBehaviour
{
    private Inventory_Item itemToCraft;
    private Inventory_Storage storage;
    private UI_CraftPreviewSlot[] craftPreviewSlots;

    [Header("Item Preview Setup")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private Sprite defaultItemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI buttonText;

    void Awake()
    {
        itemName.text = "";
        itemIcon.sprite = defaultItemIcon;

        StringBuilder sb = new();

        sb.AppendLine("1. Pick a Category");
        sb.AppendLine("2. Select a Recipe to craft");
        sb.AppendLine("3. Click CRAFT to craft the item");

        itemInfo.text = sb.ToString();
    }

    public void SetupCraftPreview(Inventory_Storage storage)
    {
        this.storage = storage;

        craftPreviewSlots = GetComponentsInChildren<UI_CraftPreviewSlot>();
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            buttonText.text = "Pick an item.";
            return;
        }

        // if (storage.CanCraftItem(itemToCraft))
        // {
        //     storage.CraftItem(itemToCraft);
        // }

        UpdateCraftPreviewSlots();
    }

    public void UpdateCraftPreview(Item_DataSO itemData)
    {
        itemToCraft = new Inventory_Item(itemData);

        itemIcon.sprite = itemData.itemIcon;
        itemName.text = itemData.itemName;
        itemInfo.text = itemToCraft.GetItemInfo();
        UpdateCraftPreviewSlots();
    }

    private void UpdateCraftPreviewSlots()
    {
        // Hide all the required materials list elements.
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }

        for (int i = 0; i < itemToCraft.itemData.craftRecipe.Length; i++)
        {
            Inventory_RecipeIngredient requiredIngredient = itemToCraft.itemData.craftRecipe[i];
            int avaliableAmount = storage.GetAvailableAmountOf(requiredIngredient.itemData);
            int requiredAmount = requiredIngredient.RequiredQuantity;

            craftPreviewSlots[i].gameObject.SetActive(true);
            craftPreviewSlots[i].SetupPreviewSlot(
                requiredIngredient.itemData,
                avaliableAmount,
                requiredAmount
            );
        }
    }
}
