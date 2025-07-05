using System;
using UnityEngine;

/// <summary>
/// Unity Scriptable Object (SO) that defines a blueprint for player Equipment
/// such as Armor and Weapons, which can be worn by the player to improve their
/// calculated stats.
/// 
/// To create a new instance in Unity, Right click, then navigate to:
/// Create > RPG Setup > Item Data > Equipment Item
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Item Data/Equipment Item", fileName = "Equipment Data - ")]
public class Equipment_DataSO : Item_DataSO
{
    [Header("Item Modifiers")]
    public ItemModifier[] modifiers;
}

[Serializable]
public class ItemModifier
{
    [Tooltip("Which Stat does this specific Modifier alter?")]
    public StatType statType;

    [Tooltip("How much is the associated Stat altered by?")]
    public float value;
}
