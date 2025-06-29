using UnityEngine;

public class SkillObject_Base : MonoBehaviour
{
    [Tooltip("The 'Enemy' Layer")]
    [SerializeField] protected LayerMask whatIsEnemy;
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float damageRadius = 1f;
    [SerializeField] protected float detectionRadius = 10f;

    protected Entity_Stats playerStats;
    protected DamageScaleData damageScaleData;
    protected ElementType usedElement;

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

            damageable.TakeDamage(physDamage, elemDamage, element, transform);

            if (element != ElementType.None)
            {
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);
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
