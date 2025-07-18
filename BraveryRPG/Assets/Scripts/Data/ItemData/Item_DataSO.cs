using UnityEngine;

/// <summary>
/// Unity Scriptable Object (SO) that defines a blueprint for Inventory items
/// that can be acquired and stored in the player's backpack.
/// 
/// To create a new instance in Unity, Right click, then navigate to:
/// Create > RPG Setup > Item Data > Material Item
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Item Data/Material Item", fileName = "Material Data - ")]
public class Item_DataSO : ScriptableObject
{
    // public string saveId { get; private set; }

    [Header("Item Details")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int maxStackSize = 1;

    [Header("Merchant Details")]
    [Range(0, 10000)]
    public int itemPrice = 100;
    public int minStackSizeAtShop = 1;
    public int maxStackSizeAtShop = 1;

    [Header("Drop Details")]
    [Range(0, 1000)]
    [Tooltip("Item power: 0 = Common, low value item, 1000 = Legendary, very rare item.")]
    public int itemRarity = 100;
    [Range(0, 100)]
    public float dropChance;
    [Range(0, 100)]
    public float maxDropChance = 65f;

    [Header("Item Effect")]
    public ItemEffect_DataSO itemEffect;

    [Header("Craft Details")]
    public Inventory_RecipeIngredient[] craftRecipe;

    private void OnValidate()
    {
        dropChance = GetDropChance();

        // #if UNITY_EDITOR
        //         string path = AssetDatabase.GetAssetPath(this);
        //         saveId = AssetDatabase.AssetPathToGUID(path);
        // #endif
    }

    public float GetDropChance()
    {
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100;

        return Mathf.Min(chance, maxDropChance);
    }
}
