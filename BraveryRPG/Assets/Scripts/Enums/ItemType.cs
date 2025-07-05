using UnityEngine;

public enum ItemType
{
    [Tooltip("Materials used as components for crafting other items and equipment")]
    Material,

    [Tooltip("Offensive weapon equipped by the player")]
    Weapon,

    [Tooltip("Protective equipment worn by the player")]
    Armor,

    [Tooltip("Rings, Amulets, Accessories, Belts")]
    Trinket,

    [Tooltip("Potions, Buffs, Scrolls")]
    Consumable
}
