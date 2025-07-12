using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Heal on Doing Damage", fileName = "Item Effect Data - Heal on Doing Phys Damage")]
public class ItemEffect_HealOnDoingDamage : ItemEffect_DataSO
{
    [SerializeField] private float percentHealedOnAttack = 0.2f;

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        // When we observe an event where the player Deals Damage, try to execute
        // the unique item effect (check whether it is on cooldown, and whether
        // other conditions are satisfied).
        player.Combat.OnDoingPhysicalDamage += HealOnDoingDamage;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.Combat.OnDoingPhysicalDamage -= HealOnDoingDamage;
        player = null;
    }

    private void HealOnDoingDamage(float damage)
    {
        player.Health.IncreaseHealth(damage * percentHealedOnAttack);
    }
}
