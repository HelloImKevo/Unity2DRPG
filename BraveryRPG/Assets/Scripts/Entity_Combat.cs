using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public float damageToInflict = 10;

    [Header("Target Detection")]
    [Tooltip("Central point from which collision detection will be performed for melee attacks")]
    [SerializeField] private Transform targetCheckPoint;
    [Tooltip("Circular radius from the Target Check Point to detect collisions when a melee attack is performed")]
    [SerializeField] private float targetCheckRadius = 1f;
    [SerializeField] private LayerMask whatIsTarget;

    public void PerformAttack()
    {
        GetDetectedColliders();

        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();

            targetHealth?.TakeDamage(damageToInflict, transform);
        }
    }

    private Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheckPoint.position, targetCheckRadius, whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheckPoint.position, targetCheckRadius);
    }

    public void DamageEnemies()
    {
        Collider2D[] targetColliders = GetDetectedColliders();

        // foreach (Collider2D enemy in enemyColliders)
        // {
        //     enemy.GetComponent<Enemy>().TakeDamage();
        // }
    }
}
