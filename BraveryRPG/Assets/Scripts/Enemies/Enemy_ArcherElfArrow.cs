using UnityEngine;

public class Enemy_ArcherElfArrow : MonoBehaviour, ICounterable
{
    [Tooltip("Layers that can be hit by the Arrow, like Player and Ground.")]
    [SerializeField] private LayerMask whatIsTarget;

    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    public bool CanBeCountered => true;

    public void SetupArrow(float xVelocity, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        this.combat = combat;
        rb.linearVelocity = new Vector2(xVelocity, 0);

        if (rb.linearVelocity.x < 0)
        {
            // Flip the arrow to face the other direction.
            transform.Rotate(0, 180, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // See also: Entity_VisionCone.CanSeePlayer()

        // Check if collided object is on a layer we want to damage.
        // Layers are a 32-bit integer with 32 zeroes (Layer 0 - 31):
        // 00000000000000000000000000000000
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            combat.PerformAttackOnTarget(collision.transform, null, true);
            StickArrowIntoTarget(collision.transform);
        }
    }

    /// <summary>See also: SkillObject_Sword.StickSwordToCollider()</summary>
    private void StickArrowIntoTarget(Transform target)
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        col.enabled = false;
        anim.enabled = false;

        GetComponentInChildren<TrailRenderer>().gameObject.SetActive(false);

        transform.parent = target;

        Destroy(gameObject, 3f);
    }

    public void OnReceiveCounterattack()
    {
        // When player parries the arrow, flip its direction and velocity back towards
        // the enemy that fired this arrow.
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * -1, 0);
        transform.Rotate(0, 180, 0);

        int enemyLayer = LayerMask.NameToLayer("Enemy");

        whatIsTarget = whatIsTarget | (1 << enemyLayer);
    }
}
