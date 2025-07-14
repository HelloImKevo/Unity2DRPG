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
    [Header("Item Details")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;
    public int maxStackSize = 1;

    [Header("Item Effect")]
    public ItemEffect_DataSO itemEffect;

    [Header("Craft Details")]
    public Inventory_RecipeIngredient[] craftRecipe;
}
