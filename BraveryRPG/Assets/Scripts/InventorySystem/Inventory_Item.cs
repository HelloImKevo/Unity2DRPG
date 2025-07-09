using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    private string itemId;

    public Item_DataSO itemData;
    public int stackSize = 1;

    public ItemModifier[] modifiers { get; private set; }
    public ItemEffect_DataSO itemEffect;

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
        itemEffect = itemData.itemEffect;

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

    // public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);

    // public void RemoveItemEffect() => itemEffect?.Unsubscribe();

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
}
