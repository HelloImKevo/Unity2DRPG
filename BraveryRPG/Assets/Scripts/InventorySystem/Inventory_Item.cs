using System;
using System.Text;
using UnityEngine;

[Serializable]
public class Inventory_Item
{
    // private string itemId;

    public Item_DataSO itemData;
    public int stackSize = 1;

    // public ItemModifier[] modifiers { get; private set; }
    // public ItemEffect_DataSO itemEffect;

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;
        // itemEffect = itemData.itemEffect;

        // modifiers = EquipmentData()?.modifiers;
        // itemId = itemData.itemName + " - " + Guid.NewGuid();
    }

    // public void AddModifiers(Entity_Stats playerStats)
    // {
    //     foreach (var mod in modifiers)
    //     {
    //         Stat statToModify = playerStats.GetStatByType(mod.statType);
    //         statToModify.AddModifier(mod.value, itemId);
    //     }
    // }

    // public void RemoveModifiers(Entity_Stats playerStats)
    // {
    //     foreach (var mod in modifiers)
    //     {
    //         Stat statToModify = playerStats.GetStatByType(mod.statType);
    //         statToModify.RemoveModifier(itemId);
    //     }
    // }

    // public void AddItemEffect(Player player) => itemEffect?.Subscribe(player);

    // public void RemoveItemEffect() => itemEffect?.Unsubscribe();

    // private EquipmentDataSO EquipmentData()
    // {
    //     if (itemData is EquipmentDataSO equipment)
    //     {
    //         return equipment;
    //     }

    //     return null;
    // }

    // public bool CanAddStack() => stackSize < itemData.maxStackSize;

    // public void AddStack() => stackSize++;

    // public void RemoveStack() => stackSize--;
}
