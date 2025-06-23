using UnityEngine;

public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();

    [Header("Open Details")]
    [SerializeField] private Vector2 knockback = new(0, 3f);

    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        vfx.PlayOnDamageFlashWhiteVfx();

        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;

        // This is for fun, and should be fine-tuned. It's rotating the
        // rigidbody, but not the box collider, which results in some
        // odd physics.
        rb.angularVelocity = Random.Range(-120, 120);

        return true;

        // Later on: Drop Items.
    }
}
