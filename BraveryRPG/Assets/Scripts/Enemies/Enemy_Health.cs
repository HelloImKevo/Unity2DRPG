using UnityEngine;

public class Enemy_Health : Entity_Health
{
    // This is a dynamic, mutable getter. This will always perform a Component
    // lookup, and return the Enemy instance. This approach would not be performant
    // if we need to call it multiple times.
    private Enemy enemy => GetComponent<Enemy>();

    public override void TakeDamage(float damage, Transform damageDealer)
    {
        base.TakeDamage(damage, damageDealer);

        if (isDead)
        {
            // Do not enter the Battle state.
            return;
        }

        // Alternative approach:
        // damageDealer.CompareTag("Player")
        if (damageDealer.GetComponent<Player>() != null)
        {
            enemy.TryEnterBattleState(damageDealer);
        }
    }
}
