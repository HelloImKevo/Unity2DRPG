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
    [Tooltip("Must have a child object that implements TextMeshProUGUI.")]
    [SerializeField] private Button craftButton;

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

    /// <summary>
    /// Invoked by the Button.OnClick() event for the CRAFT button.
    /// </summary>
    public void ConfirmCraft()
    {
        if (itemToCraft == null)
        {
            craftButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pick an item.";
            return;
        }

        if (storage.CanCraftItem(itemToCraft))
        {
            storage.CraftItem(itemToCraft);
        }
        else
        {
            // You cannot craft the selected recipe - show an error Toast.
            string validationMessage = "";

            if (!storage.HasEnoughMaterials(itemToCraft))
            {
                validationMessage = "Not enough materials!";
            }
            else if (!storage.playerInventory.CanAddItem(itemToCraft))
            {
                validationMessage = "No space in inventory!";
            }

            ToastStyle errorStyle = new()
            {
                textColor = Color.white,
                backgroundColor = new Color(0.2f, 0, 0),
                blinkColor = Color.red,
                enableBlink = true,
                duration = 2f
            };
            ToastManager.Instance.ShowToast(
                validationMessage,
                errorStyle,
                ToastAnchor.Below,
                craftButton.transform
            );
        }

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
        Debug.Log($"UI_CraftPreview.UpdateCraftPreviewSlots() -> Hiding {craftPreviewSlots.Length} ingredient list items.");

        // Hide all the required materials list elements.
        foreach (var slot in craftPreviewSlots)
        {
            slot.gameObject.SetActive(false);
        }

        Inventory_RecipeIngredient[] requiredIngredients = itemToCraft.itemData.craftRecipe;

        Debug.Log($"UI_CraftPreview.UpdateCraftPreviewSlots() -> There are {requiredIngredients.Length} " +
                  $"distinct ingredients required to craft {itemToCraft.itemData.name}.");

        for (int i = 0; i < requiredIngredients.Length; i++)
        {
            Inventory_RecipeIngredient requiredIngredient = requiredIngredients[i];
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
