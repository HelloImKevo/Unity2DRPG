using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private string itemId;

    public Item_DataSO itemData;

    [Tooltip("Quantity of the same item that can fit into a stack.")]
    public int stackSize = 1;

    public ItemModifier[] modifiers { get; private set; }
    public ItemEffect_DataSO itemEffect;

    public int buyPrice { get; private set; }
    public float sellPrice { get; private set; }

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
        itemEffect = itemData.itemEffect;

        // Establish item price values.
        buyPrice = itemData.itemPrice;
        // Items sell for 35% of their purchase price.
        sellPrice = itemData.itemPrice * 0.35f;

        modifiers = EquipmentData()?.modifiers;
        // Allows modifiers from multiple instances of the same item
        // (such as two "Ring of Luck") to be applied to the entity Stats.
        itemId = itemData.itemName + " - " + Guid.NewGuid();
    }

    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemId);
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemId);
        }
    }

    public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);

    public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    /// <summary>
    /// Checks whether the <see cref="itemData"/> is of type <see cref="Equipment_DataSO"/>
    /// and casts it to that type. Otherwise, returns null.
    /// </summary>
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
        {
            return equipment;
        }

        return null;
    }

    public bool CanAddStack() => stackSize < itemData.maxStackSize;

    public void AddStack() => stackSize++;

    public void RemoveStack() => stackSize--;

    public string GetItemInfo()
    {
        StringBuilder sb = new();

        if (ItemType.Material == itemData.itemType)
        {
            sb.AppendLine("");
            sb.AppendLine("Used for crafting.");
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        if (ItemType.Consumable == itemData.itemType)
        {
            sb.AppendLine("");
            sb.AppendLine(itemEffect.effectDescription);
            sb.AppendLine("");
            sb.AppendLine("");
            return sb.ToString();
        }

        sb.AppendLine("");

        foreach (var mod in modifiers)
        {
            string modType = StatUtils.GetStatNameByType(mod.statType);
            string modValue = StatUtils.IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
            sb.AppendLine("+ " + modValue + " " + modType);
        }

        if (itemEffect != null)
        {
            sb.AppendLine("");
            sb.AppendLine("Unique effect:");
            sb.AppendLine(itemEffect.effectDescription);
        }

        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }
}
