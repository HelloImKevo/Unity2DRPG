using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [SerializeField] protected GameObject onHitVfx;
    [Space]
    [Tooltip("The 'Enemy' Layer")]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float damageRadius = 1f;
    [Tooltip("Radius to detect enemies within the vicinity - used for 'follow enemy' skill behaviors.")]
    [SerializeField] protected float detectionRadius = 10f;

    protected Rigidbody2D rb;
    protected Animator anim;

    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;

    protected bool wasTargetHit;
    protected Transform lastTarget;

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

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
                // Create On-Hit visual effect.
                Instantiate(onHitVfx, target.transform.position, Quaternion.identity);
            }

            Debug.Log($"{GetType().Name} dealt {physDamage} (P) + {elemDamage} (E) damage to: {damageable.GetType().Name}");

            usedElement = element;
        }
    }

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

    protected Collider2D[] EnemiesAround(Transform point, float radius)
    {
        return Physics2D.OverlapCircleAll(point.position, radius, whatIsEnemy);
    }

    protected virtual void OnDrawGizmos()
    {
        if (targetCheck == null)
            targetCheck = transform;

        Gizmos.DrawWireSphere(targetCheck.position, damageRadius);
    }
}
