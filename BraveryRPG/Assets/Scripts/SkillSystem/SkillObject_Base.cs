using UnityEngine;

/// <summary>
/// Base class for all skill projectiles and objects, providing common functionality for
/// damage calculation, enemy detection, and visual effects management.
/// </summary>
public class SkillObject_Base : MonoBehaviour
{
    /// <summary>VFX prefab spawned when hitting targets.</summary>
    [Tooltip("Prefab with a contact sparks animation.")]
    [SerializeField] protected GameObject onHitVfx;

    [Space]

    /// <summary>Layer mask defining what counts as an enemy for collision detection.</summary>
    [Tooltip("The 'Enemy' Layer")]
    [SerializeField] protected LayerMask whatIsEnemy;

    /// <summary>Transform used as center point for damage radius calculations.</summary>
    [Tooltip("Transform used as center point for damage radius calculations.")]
    [SerializeField] protected Transform targetCheck;

    /// <summary>Radius around the skill object where enemies take damage.</summary>
    [Tooltip("Radius around the skill object where enemies take damage.")]
    [SerializeField] protected float damageRadius = 1f;

    /// <summary>Detection radius for finding nearby enemies for targeting behaviors.</summary>
    [Tooltip("Radius to detect enemies within the vicinity - used for 'follow enemy' skill behaviors.")]
    [SerializeField] protected float detectionRadius = 10f;

    /// <summary>Rigidbody component for physics-based movement.</summary>
    protected Rigidbody2D rb;

    /// <summary>Animator component for visual animations.</summary>
    protected Animator anim;

    /// <summary>Player's stat system for damage calculation.</summary>
    protected Entity_Stats playerStats;

    /// <summary>Damage scaling configuration data.</summary>
    protected DamageScaleData damageScaleData;

    /// <summary>Elemental type used for the most recent attack.</summary>
    protected ElementType usedElement;

    /// <summary>Flag indicating if the last attack successfully hit a target.</summary>
    protected bool wasTargetHit;

    /// <summary>Reference to the most recently targeted enemy.</summary>
    protected Transform lastTarget;

    /// <summary>
    /// Initializes component references for rigidbody and animator on awakening.
    /// </summary>
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Deals damage to all enemies within the specified radius around a given point.
    /// Calculates physical and elemental damage, applies status effects, and spawns
    /// hit VFX for successful attacks.
    /// </summary>
    protected void DamageEnemiesInRadius(Transform point, float radius)
    {
        foreach (var target in EnemiesAround(point, radius))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null) continue;

            AttackData attackData = playerStats.GetAttackData(damageScaleData);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physDamage = attackData.physicalDamage;
            float elemDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            wasTargetHit = damageable.TakeDamage(physDamage, elemDamage, element, transform);

            if (element != ElementType.None)
            {
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);
            }

            if (wasTargetHit)
            {
                // Keep track of the last target that this skill hit.
                lastTarget = target.transform;
                // Create On-Hit visual effect.
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
            }

            Debug.Log($"{GetType().Name} dealt {physDamage} (P) + {elemDamage} (E) damage to: {damageable.GetType().Name}");

            usedElement = element;
        }
    }

    /// <summary>
    /// Finds the closest enemy within detection radius and returns its transform.
    /// Returns null if no enemies are found within range.
    /// </summary>
    protected Transform FindClosestTarget()
    {
        Transform target = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in EnemiesAround(transform, detectionRadius))
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);

            if (distance < closestDistance)
            {
                target = enemy.transform;
                closestDistance = distance;
            }
        }

        return target;
    }

    /// <summary>
    /// Returns all enemy colliders within the specified radius around a given point
    /// using Physics2D overlap detection.
    /// </summary>
    protected Collider2D[] EnemiesAround(Transform point, float radius)
    {
        return Physics2D.OverlapCircleAll(point.position, radius, whatIsEnemy);
    }

    /// <summary>
    /// Draws visual gizmos in the scene view showing the damage radius for debugging
    /// and level design purposes.
    /// </summary>
    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
        {
            targetCheck = transform;
        }

        Gizmos.DrawWireSphere(targetCheck.position, damageRadius);
    }
}
