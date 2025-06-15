using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();

    [Header("Open Details")]
    [SerializeField] private Vector2 knockback = new(0, 3f);

    public void TakeDamage(float damage, Transform damageDealer)
    {
        vfx.PlayOnDamageVfx();

        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;

        // This is for fun, and should be fine-tuned. It's rotating the
        // rigidbody, but not the box collider, which results in some
        // odd physics.
        rb.angularVelocity = Random.Range(-120, 120);

        // Later on: Drop Items.
    }
}
