using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    /// <summary>
    /// Event triggered when the shard explodes, allowing observers to react to the explosion.
    /// </summary>
    /// <seealso cref="Explode"/>
    public event Action OnExplode;
    private Skill_Shard shardManager;

    [Tooltip("The VFX_ShardExplode prefab with animator.")]
    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;

    /// <summary>
    /// Updates the shard's position each frame, moving it toward the target using frame-rate
    /// independent movement. Uses <see cref="Vector3.MoveTowards"/> for smooth linear interpolation.
    /// </summary>
    /// <seealso cref="StartMovingTowardsClosestTarget"/>
    private void Update()
    {
        if (target == null) return;

        // Gradually move (drift) towards target, at a frame-rate-independent speed.
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    /// <summary>
    /// Initiates movement toward the closest enemy target within detection range.
    /// </summary>
    /// <param name="speed">Movement speed in units per second.</param>
    public void StartMovingTowardsClosestTarget(float speed, Transform newTarget = null)
    {
        target = newTarget == null ? FindClosestTarget() : newTarget;
        this.speed = speed;
    }

    /// <summary>
    /// Configures the shard with basic parameters and schedules automatic detonation.
    /// </summary>
    /// <seealso cref="SetupShard(Skill_Shard, float, bool, float)"/>
    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        playerStats = shardManager.Player.Stats;
        damageScaleData = shardManager.damageScaleData;

        float detonationTime = shardManager.GetDetonateTime();

        Invoke(nameof(Explode), detonationTime);
    }

    /// <summary>
    /// Configures the shard with extended parameters including movement and custom timing options.
    /// </summary>
    /// <param name="shardManager">The <see cref="Skill_Shard"/> instance managing this shard's behavior.</param>
    /// <param name="detonationTime">Custom time in seconds before automatic explosion.</param>
    /// <param name="canMove">Whether the shard should move toward targets automatically.</param>
    /// <param name="shardSpeed">Movement speed when <paramref name="canMove"/> is true.</param>
    /// <seealso cref="SetupShard(Skill_Shard)"/>
    /// <seealso cref="StartMovingTowardsClosestTarget"/>
    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed, Transform target = null)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.Player.Stats;
        damageScaleData = shardManager.damageScaleData;

        Invoke(nameof(Explode), detonationTime);

        if (canMove)
        {
            StartMovingTowardsClosestTarget(shardSpeed, target);
        }
    }

    /// <summary>
    /// Detonates the shard, dealing area damage and creating VFX (visual effects) prefab
    /// before self-destruction. Triggers the <see cref="OnExplode"/> event and uses
    /// <see cref="SkillObject_Base.DamageEnemiesInRadius"/> for damage calculation.
    /// </summary>
    /// <seealso cref="OnTriggerEnter2D"/>
    public void Explode()
    {
        DamageEnemiesInRadius(transform, damageRadius);
        GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        // Colorize the explosion sprite with a tint matching the dominant element.
        vfx.GetComponentInChildren<SpriteRenderer>().color = shardManager.Player.Vfx.GetElementColor(usedElement);

        // Trigger observable event action.
        OnExplode?.Invoke();
        Destroy(gameObject);
    }

    /// <summary>
    /// Handles collision detection with enemy entities, triggering immediate explosion
    /// on contact. Only responds to objects with an <see cref="Enemy"/> component attached.
    /// </summary>
    /// <param name="collision">The collider that entered the shard's trigger zone.</param>
    /// <seealso cref="Explode"/>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // We only want to explode and damage Enemies that touch this Shard (exclude other collidable objects).
        if (collision.GetComponent<Enemy>() == null) return;

        Explode();
    }
}
