using UnityEngine;

[System.Serializable]
public class Inventory_RecipeIngredient
{
    public Item_DataSO itemData;

    [Tooltip("Quantity of materials required to craft a recipe.")]
    [SerializeField] private int requiredQuantity;

    public int RequiredQuantity => requiredQuantity;
}
