using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float currentHp = 100f;
    [SerializeField] protected bool isDead;

    private Entity_VFX entityVfx;

    protected virtual void Awake()
    {
        entityVfx = GetComponent<Entity_VFX>();
    }

    // Summary:
    //     [damageDealer] can be used by subclasses to acquire components, such as the
    //     Player or Enemy reference.
    public virtual void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        if (entityVfx != null) entityVfx.PlayOnDamageVfx();

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
        Debug.Log("Entity died!");
    }
}
