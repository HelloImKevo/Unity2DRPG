using UnityEngine;

public class SkillObject_TimeEcho : SkillObject_Base
{
    [SerializeField] private float wispMoveSpeed = 15;
    [SerializeField] private GameObject onDeathVfx;
    [SerializeField] private LayerMask whatIsGround;
    public int maxAttacks { get; private set; }

    /// <summary>
    /// Whether this object should move towards the player once it has been
    /// converted into a Wisp Trail Renderer.
    /// </summary>
    private bool shouldMoveToPlayer;

    private Transform playerTransform;
    private Skill_TimeEcho echoManager;
    private TrailRenderer wispTrail;
    private Entity_Health playerhealth;
    private Player_SkillManager skillManager;
    private Entity_StatusHandler statusHandler;

    private SkillObject_Health echoHealth;

    public void SetupEcho(Skill_TimeEcho echoManager)
    {
        this.echoManager = echoManager;
        playerStats = echoManager.Player.Stats;
        damageScaleData = echoManager.damageScaleData;
        maxAttacks = echoManager.GetMaxAttacks();
        playerTransform = echoManager.transform.root;
        playerhealth = echoManager.Player.Health;
        skillManager = echoManager.SkillManager;
        statusHandler = echoManager.Player.StatusHandler;

        Invoke(nameof(HandleDeath), echoManager.GetEchoDuration());
        FlipToTarget();

        echoHealth = GetComponent<SkillObject_Health>();
        wispTrail = GetComponentInChildren<TrailRenderer>();
        // By default, deactivate the wispy trail renderer effect.
        wispTrail.gameObject.SetActive(false);

        anim.SetBool("canAttack", maxAttacks > 0);
    }

    private void Update()
    {
        if (shouldMoveToPlayer)
        {
            HandleWispMovement();
        }
        else
        {
            anim.SetFloat("yVelocity", rb.linearVelocity.y);
            StopHorizontalMovement();
        }
    }

    private void HandleWispMovement()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            playerTransform.position,
            wispMoveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            HandlePlayerTouch();
            Destroy(gameObject);
        }
    }

    private void HandlePlayerTouch()
    {
        float healAmount = echoHealth.lastDamageTaken * echoManager.GetPercentOfDamageHealed();
        playerhealth.IncreaseHealth(healAmount);

        float amountInSeconds = echoManager.GetCooldownReduceInSeconds();
        skillManager.ReduceAllSkillCooldownBy(amountInSeconds);

        if (echoManager.CanRemoveNegativeEffects())
        {
            statusHandler.RemoveAllNegativeEffects();
        }
    }

    private void FlipToTarget()
    {
        Transform target = FindClosestTarget();

        if (target != null && target.position.x < transform.position.x)
        {
            transform.Rotate(0, 180f, 0);
        }
    }

    public void PerformAttack()
    {
        DamageEnemiesInRadius(targetCheck, damageRadius);

        // Do not replicate a duplicate Echo if an enemy was not hit.
        if (!wasTargetHit) return;

        bool canDuplicate = Random.value < echoManager.GetDuplicateChance();

        // Determine whether this Time Echo is on the left or right side
        // of the targeted enemy.
        float xOffset = transform.position.x < lastTarget.position.x ? 1 : -1;

        if (canDuplicate)
        {
            // Create another Time Echo at this clone's position.
            echoManager.CreateTimeEcho(lastTarget.position + new Vector3(xOffset, 0f));
        }
    }

    public void HandleDeath()
    {
        Instantiate(onDeathVfx, transform.position, Quaternion.identity);

        if (echoManager.ShouldBeWisp())
        {
            // Activate the Trail Renderer, and move towards the player.
            TurnIntoWisp();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void TurnIntoWisp()
    {
        shouldMoveToPlayer = true;
        anim.gameObject.SetActive(false);
        wispTrail.gameObject.SetActive(true);
        // The Wisp should not collide with anything, and should pass through
        // walls, barriers and the ground.
        rb.simulated = false;
    }

    private void StopHorizontalMovement()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.5f, whatIsGround);

        if (hit.collider != null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
}
