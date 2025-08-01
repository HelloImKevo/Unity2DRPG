using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [Tooltip("Text label that displays special Merchant controls, like 'LMB = Sell the item'")]
    [SerializeField] private TextMeshProUGUI merchantInfo;
    [SerializeField] private Transform inventoryInfo;

    public void ShowTooltip(
        bool show,
        RectTransform targetRect,
        Inventory_Item itemToShow,
        UISlotType slotType,
        bool showMerchantInfo
    )
    {
        base.ShowTooltip(show, targetRect);

        if (UISlotType.None == slotType)
        {
            // Hide the displayed 'Special Controls'
            merchantInfo.gameObject.SetActive(false);
            inventoryInfo.gameObject.SetActive(false);
        }
        else
        {
            // Toggle the Displayed 'Special Controls' (Equip Item, Buy Stack, Delete Item, etc.)
            merchantInfo.gameObject.SetActive(showMerchantInfo);
            inventoryInfo.gameObject.SetActive(!showMerchantInfo);
        }

        int price = GetDisplayedPrice(slotType, itemToShow);
        int totalPrice = price * itemToShow.stackSize;

        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = itemToShow.GetItemInfo();

        merchantInfo.text = GetMerchantControlsText(slotType);

        // TODO: Use an if condition to avoid allocation of the unnecessary string reference.
        string fullStackPrice = $"Price: {price}x{itemToShow.stackSize} - {totalPrice}g.";
        string singleStackPrice = $"Price: {price}g.";

        itemPrice.text = itemToShow.stackSize > 1 ? fullStackPrice : singleStackPrice;

        string color = GetColorByRarity(itemToShow.itemData.itemRarity);
        itemName.text = GetColoredText(color, itemToShow.itemData.itemName);
    }

    private int GetDisplayedPrice(UISlotType slotType, Inventory_Item item)
    {
        if (UISlotType.MerchantSlot == slotType)
        {
            return item.buyPrice;
        }
        else if (UISlotType.PlayerSlot == slotType)
        {
            return Mathf.FloorToInt(item.sellPrice);
        }
        else
        {
            return 0;
        }
    }

    private string GetMerchantControlsText(UISlotType slotType)
    {
        if (UISlotType.MerchantSlot == slotType)
        {
            return "RMB: Buy\n" +
                "L.Ctrl+RMB: Buy Full Stack";
        }
        else if (UISlotType.PlayerSlot == slotType)
        {
            return "LMB: Equip/Use\n" +
                "RMB: Sell\n" +
                "L.Ctrl+RMB: Sell Full Stack";
        }
        else
        {
            return "";
        }
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
