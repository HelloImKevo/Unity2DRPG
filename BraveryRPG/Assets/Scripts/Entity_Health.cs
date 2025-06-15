using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float currentHp = 100f;
    [SerializeField] protected bool isDead;

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 onDamageKnockback = new(2.5f, 2f);
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Heavy Damage Knockback")]
    [Range(0, 1)]
    [Tooltip("Percentage of HP you should lose to get a heavy knockback effect")]
    [SerializeField] private float heavyDamageThreshold = 0.3f;
    [SerializeField] private float heavyKnockbackDuration = 0.5f;
    [SerializeField] private Vector2 onHeavyDamageKnockback = new(6f, 4f);

    private Entity entity;
    private Entity_VFX entityVfx;

    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    // Summary:
    //     [damageDealer] can be used by subclasses to acquire components, such as the
    //     Player or Enemy reference.
    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        float duration = CalculateDuration(damage);
        Vector2 knockback = CalculateKnockback(damage, damageDealer);

        if (entityVfx != null) entityVfx.PlayOnDamageVfx();
        if (entity != null) entity.ReceiveKnockback(knockback, duration);

        ReduceHp(damage);
    }

    protected void ReduceHp(float damage)
    {
        currentHp -= damage;

        if (currentHp <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        entity.EntityDeath();
    }

    // Summary:
    //     Determines what direction the damage is coming from (front or back)
    //     and will negate the [onDamageKnockback] X if necessary.
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        // See also: Enemy_BattleState.DirectionToPlayer()
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;
        knockback.x *= direction;

        return knockback;
    }

    private float CalculateDuration(float damage)
    {
        return IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    // Summary:
    //     Calculates the percentage of health taken by [damage] and checks if it
    //     is greater than the heavy damage threshold of 30%.
    private bool IsHeavyDamage(float damage) => damage / currentHp > heavyDamageThreshold;
}
