using System;
using UnityEngine;

public class SkillObject_Shard : SkillObject_Base
{
    public event Action OnExplode;
    private Skill_Shard shardManager;

    [Tooltip("The VFX_ShardExplode prefab with animator.")]
    [SerializeField] private GameObject vfxPrefab;

    private Transform target;
    private float speed;

    private void Update()
    {
        if (target == null) return;

        // Gradually move (drift) towards target, at a frame-rate-independent speed.
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
    }

    public void StartMovingTowardsClosestTarget(float speed)
    {
        target = FindClosestTarget();
        this.speed = speed;
    }

    public void SetupShard(Skill_Shard shardManager)
    {
        this.shardManager = shardManager;

        playerStats = shardManager.Player.Stats;
        damageScaleData = shardManager.damageScaleData;

        float detonationTime = shardManager.GetDetonateTime();

        Invoke(nameof(Explode), detonationTime);
    }

    public void SetupShard(Skill_Shard shardManager, float detonationTime, bool canMove, float shardSpeed)
    {
        this.shardManager = shardManager;
        playerStats = shardManager.Player.Stats;
        damageScaleData = shardManager.damageScaleData;

        Invoke(nameof(Explode), detonationTime);

        if (canMove)
        {
            StartMovingTowardsClosestTarget(shardSpeed);
        }
    }

    /// <summary>
    /// Deals damage to nearby enemies within the radius, creates a 'Shard Explosion'
    /// VFX prefab, and then destroys self.
    /// </summary>
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // We only want to explode and damage Enemies that touch this Shard (exclude other collidable objects).
        if (collision.GetComponent<Enemy>() == null) return;

        Explode();
    }
}
