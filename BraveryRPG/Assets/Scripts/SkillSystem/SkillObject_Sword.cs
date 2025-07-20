using UnityEngine;

public class SkillObject_Sword : SkillObject_Base
{
    protected Skill_ThrowSword swordManager;

    protected Transform playerTransform;
    protected bool shouldComeback;
    protected float comebackSpeed = 20f;
    protected float maxAllowedDistance = 25f;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log($"Created instance of {GetType().Name}");
    }

    protected virtual void Update()
    {
        // Make the sword initially point in the direction it is flying.
        transform.right = rb.linearVelocity;
        HandleComeback();
    }

    public virtual void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        rb.linearVelocity = direction;

        this.swordManager = swordManager;

        // Establish reference to player root object position.
        playerTransform = swordManager.transform.root;
        playerStats = swordManager.PlayerRef.Stats;
        damageScaleData = swordManager.damageScaleData;
    }

    public void EnableSwordFlyBackToPlayer() => shouldComeback = true;

    protected void HandleComeback()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > maxAllowedDistance)
        {
            EnableSwordFlyBackToPlayer();
        }

        if (!shouldComeback) return;

        // Make the sword start flying back towards the player (like Thor's hammer).
        // Use Time.deltaTime to make the speed independent of the framerate.
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            comebackSpeed * Time.deltaTime
        );

        // Once the sword is quite close to the player, destroy this object so
        // that the player can throw the sword again.
        if (distance < 0.5f)
        {
            // NOTE: This will nullify the Skill_ThrowSword.currentSword reference,
            // held in the 'swordManager' instance, which is why this event will
            // permit the sword to be thrown again.
            Destroy(gameObject);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        StickSwordToCollider(collision);
        DamageEnemiesInRadius(transform, 1f);
    }

    protected void StickSwordToCollider(Collider2D collision)
    {
        // Indicates whether the rigid body should be simulated or not by the physics system.
        rb.simulated = false;
        // Make the sword a child of the object it collided with. This will make
        // the sword 'stick to' enemies and follow them as the walk around.
        transform.parent = collision.transform;
    }
}
