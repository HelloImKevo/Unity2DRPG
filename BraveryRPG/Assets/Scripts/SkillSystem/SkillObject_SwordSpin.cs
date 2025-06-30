using UnityEngine;

/// <summary>
/// Spinning sword that damages all nearby enemies.
/// </summary>
public class SkillObject_SwordSpin : SkillObject_Sword
{
    private int maxDistance;
    private float attacksPerSecond;
    private float attackTimer;

    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        anim?.SetTrigger("spin");

        maxDistance = swordManager.maxDistance;
        attacksPerSecond = swordManager.attacksPerSecond;

        Invoke(nameof(EnableSwordFlyBackToPlayer), swordManager.maxSpinDuration);
    }

    protected override void Update()
    {
        HandleAttackNearbyEnemies();
        HandleStoppingFlight();
        HandleComeback();
    }

    private void HandleStoppingFlight()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistance && rb.simulated)
        {
            // Stop performing physics calculations on the sword.
            rb.simulated = false;
        }
    }

    private void HandleAttackNearbyEnemies()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1);
            attackTimer = 1 / attacksPerSecond;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Stop sword from flying, once it hits a collider object (Enemy or Ground).
        rb.simulated = false;
    }
}
