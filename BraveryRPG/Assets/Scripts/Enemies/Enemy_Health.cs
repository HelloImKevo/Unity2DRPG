using UnityEngine;

public class Enemy_Health : Entity_Health
{
    // This is a dynamic, mutable getter. This will always perform a Component
    // lookup, and return the Enemy instance. This approach would not be performant
    // if we need to call it multiple times.
    private Enemy enemy => GetComponent<Enemy>();

    public override bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        // Check whether the enemy is currently invulnerable / invincible.
        if (!canTakeDamage) return false;

        bool wasHit = base.TakeDamage(damage, elementalDamage, element, damageDealer);

        if (!wasHit)
        {
            // If enemy evaded the attack, do not enter the Battle state.
            return false;
        }

        // Alternative approach:
        // damageDealer.CompareTag("Player")
        if (damageDealer.GetComponent<Player>() != null)
        {
            enemy.TryEnterBattleState(damageDealer);
        }

        return true;
    }
}
