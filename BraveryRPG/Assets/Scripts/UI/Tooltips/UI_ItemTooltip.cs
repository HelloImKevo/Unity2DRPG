using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [Tooltip("Text label that displays special Merchant controls, like 'LMB = Sell the item'")]
    [SerializeField] private Transform merchantInfo;
    [SerializeField] private Transform inventoryInfo;

    public void ShowTooltip(
        bool show,
        RectTransform targetRect,
        Inventory_Item itemToShow,
        bool buyPrice = false,
        bool showMerchantInfo = false
    )
    {
        base.ShowTooltip(show, targetRect);

        // Toggle the Displayed 'Special Controls' (Equip Item, Buy Stack, Delete Item, etc.)
        merchantInfo.gameObject.SetActive(showMerchantInfo);
        inventoryInfo.gameObject.SetActive(!showMerchantInfo);

        int price = buyPrice ? itemToShow.buyPrice : Mathf.FloorToInt(itemToShow.sellPrice);
        int totalPrice = price * itemToShow.stackSize;

        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = itemToShow.GetItemInfo();

        // TODO: Use an if condition to avoid allocation of the unnecessary string reference.
        string fullStackPrice = $"Price: {price}x{itemToShow.stackSize} - {totalPrice}g.";
        string singleStackPrice = $"Price: {price}g.";

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singleStackPrice;

        string color = GetColorByRarity(itemToShow.itemData.itemRarity);
        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }

    private string GetColorByRarity(int rarity)
    {
        if (rarity <= 100) return "white";  // Common
        if (rarity <= 300) return "green";  // Uncommon
        if (rarity <= 600) return "blue";   // Rare
        if (rarity <= 850) return "purple"; // Epic
        return "orange";                    // Legendary
    }
}
