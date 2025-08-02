using UnityEngine;

public class Enemy_Health : Entity_Health
{
    // This is a dynamic, mutable getter. This will always perform a Component
    // lookup, and return the Enemy instance. This approach would not be performant
    // if we need to call it multiple times.
    private Enemy enemy;

    private Player_QuestManager questManager;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<Enemy>();
        questManager = Player.GetInstance().questManager;
    }

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

    protected override void Die()
    {
        base.Die();

        // Attempt to add progress to the player's Quest Manager,
        // like "Eliminated 5 out of 10 Skeletons"
        questManager.TryAddProgress(enemy.questTargetId);
    }
}
