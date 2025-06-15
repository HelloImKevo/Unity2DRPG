using UnityEngine;

public class Entity_Health : MonoBehaviour
{
    [SerializeField] protected float currentHp = 100f;
    [SerializeField] protected bool isDead;

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;

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
