using UnityEngine;

/// <summary>
/// A consumable inventory item that heals the player.
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal Effect", fileName = "Item Effect Data - Heal")]
public class ItemEffect_Heal : ItemEffect_DataSO
{
    [Tooltip("Percent of player Max Health applied as healing to the player.")]
    [SerializeField] private float healPercent = 0.1f;

    public override void ExecuteEffect()
    {
        Player player = FindFirstObjectByType<Player>();

        float healAmount = player.Stats.GetMaxHealth() * healPercent;

        player.Health.IncreaseHealth(healAmount);
    }
}
