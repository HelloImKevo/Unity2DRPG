using System.Text;
using TMPro;
using UnityEngine;

public class UI_ItemTooltip : UI_Tooltip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;

    public void ShowTooltip(bool show, RectTransform targetRect, Inventory_Item itemToShow)
    {
        base.ShowTooltip(show, targetRect);

        itemName.text = itemToShow.itemData.itemName;
        itemType.text = itemToShow.itemData.itemType.ToString();
        itemInfo.text = GetItemInfo(itemToShow);
    }

    public string GetItemInfo(Inventory_Item item)
    {
        if (ItemType.Material == item.itemData.itemType)
        {
            return "Used for crafting.";
        }

        // if (ItemType.Consumable == item.itemData.itemType)
        // {
        //     return item.itemData.itemEffect.effectDescription;
        // }

        StringBuilder sb = new();

        sb.AppendLine("");

        foreach (var mod in item.modifiers)
        {
            string modType = StatUtils.GetStatNameByType(mod.statType);
            string modValue = StatUtils.IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine("+ " + modValue + " " + modType);
        }

        // if (item.itemEffect != null)
        // {
        //     sb.AppendLine("");
        //     sb.AppendLine("Unique effect:");
        //     sb.AppendLine(item.itemEffect.effectDescription);
        // }

        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }
}
