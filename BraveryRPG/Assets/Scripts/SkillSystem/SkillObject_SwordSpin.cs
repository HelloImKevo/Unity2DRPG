using UnityEngine;

/// <summary>
/// Spinning sword projectile that hovers in place and repeatedly damages nearby enemies
/// within its radius. Stops moving when reaching maximum distance from the player.
/// </summary>
public class SkillObject_SwordSpin : SkillObject_Sword
{
    /// <summary>Maximum distance from player before sword stops moving and begins spinning.</summary>
    private int maxDistance;

    /// <summary>Number of damage ticks per second while spinning.</summary>
    private float attacksPerSecond;

    /// <summary>Timer tracking when to perform the next damage attack.</summary>
    private float attackTimer;

    /// <summary>
    /// Configures the spinning sword with movement direction and spinning behavior.
    /// Sets up maximum distance, attack rate, and schedules return-to-player timing.
    /// </summary>
    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);

        anim?.SetTrigger("spin");

        maxDistance = swordManager.maxDistance;
        attacksPerSecond = swordManager.attacksPerSecond;

        Invoke(nameof(EnableSwordFlyBackToPlayer), swordManager.maxSpinDuration);
    }

    /// <summary>
    /// Main update loop handling spinning attack behavior, movement stopping, and
    /// return-to-player mechanics.
    /// </summary>
    protected override void Update()
    {
        HandleAttackNearbyEnemies();
        HandleStoppingFlight();
        HandleComeback();
    }

    /// <summary>
    /// Monitors distance from player and stops sword physics simulation when maximum
    /// distance is exceeded, causing the sword to hover in place.
    /// </summary>
    private void HandleStoppingFlight()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer > maxDistance && rb.simulated)
        {
            // Stop performing physics calculations on the sword.
            rb.simulated = false;
        }
    }

    /// <summary>
    /// Handles periodic damage attacks against nearby enemies based on attacks per second
    /// rate. Uses a timer to control damage frequency.
    /// </summary>
    private void HandleAttackNearbyEnemies()
    {
        attackTimer -= Time.deltaTime;

        if (attackTimer < 0)
        {
            DamageEnemiesInRadius(transform, 1);
            attackTimer = 1 / attacksPerSecond;
        }
    }

    /// <summary>
    /// Stops sword movement immediately when colliding with any object (enemies or
    /// terrain), disabling physics simulation to begin spinning behavior.
    /// </summary>
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // Stop sword from flying, once it hits a collider object (Enemy or Ground).
        rb.simulated = false;
    }
}
