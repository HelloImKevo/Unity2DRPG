using UnityEngine;

// TODO: The Player can still 'push' the Chest around. We want the chest to
// be subject to attack-based physics, but it should not be pushable by the
// player, but the player still needs to be able to attack it.
public class Object_Chest : MonoBehaviour, IDamageable
{
    private Rigidbody2D rb => GetComponent<Rigidbody2D>();
    private Animator anim => GetComponentInChildren<Animator>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();

    [Header("Open Details")]
    [SerializeField] private Vector2 knockback = new(0, 3f);
    [SerializeField] private bool canDropItems = true;

    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (!canDropItems) return false;

        canDropItems = false;
        dropManager.DropItems();
        vfx.PlayOnDamageFlashWhiteVfx();

        anim.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;

        // This is for fun, and should be fine-tuned. It's rotating the
        // rigidbody, but not the box collider, which results in some
        // odd physics.
        rb.angularVelocity = Random.Range(-120, 120);

        return true;
    }
}
