using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Quest Info")]
    public string questTargetId;

    public Entity_Stats Stats { get; private set; }
    public Enemy_Health health { get; private set; }

    public Enemy_IdleState IdleState { get; protected set; }
    public Enemy_MoveState MoveState { get; protected set; }
    public Enemy_AttackState AttackState { get; protected set; }
    public Enemy_BattleState BattleState { get; protected set; }
    public Enemy_DeadState DeadState { get; protected set; }
    public Enemy_StunnedState StunnedState { get; protected set; }

    [Header("Enemy Battle Details")]
    [SerializeField] private float battleMoveSpeed = 3f;
    public float attackDistance = 2;
    // TODO: Consider adding a range, like random 0.5 - 2.0 second delay.
    [Tooltip("Delay (seconds) in between attacks while in Battle state (currently tracked at the start of an attack animation)")]
    [SerializeField] private float attackDelay = 1.5f;
    [Tooltip("How long the enemy remains engaged in pursuit of player, after losing Line of Sight")]
    public float battleTimeDuration = 5f;
    public float minRetreatDistance = 1f;
    [Tooltip("Distance of the backstep retreat effect")]
    public Vector2 retreatVelocity = new(5f, 3f);

    [Tooltip("Draws a downward Raycast to prevent enemy from pursuing the player off a ledge and falling into the abyss")]
    [SerializeField] private Transform primaryFallCheck;
    [SerializeField] private Transform secondaryFallCheck;
    public float fallCheckDistance = 10f;
    public bool BelowLedgeDetected { get; private set; }

    [Header("Stunned State Details")]
    public float stunnedDuration = 1f;
    public Vector2 stunnedVelocity = new(6f, 7f);
    [SerializeField] protected bool canBeStunned;

    [Header("Enemy Movement Details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;

    [Range(0, 2)]
    [Tooltip("Used to speed up or slow down animations for faster or slower enemies")]
    public float moveAnimSpeedMultiplier = 1;

    [Header("Player Detection")]
    [Tooltip("The 'Player' Layer")]
    [SerializeField] private LayerMask whatIsPlayer;
    [Tooltip("The point from which to draw the Raycast for LOS Player detection")]
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;
    public Transform PlayerRef { get; private set; }

    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;

    public float GetAttackDelay()
    {
        // Attack delay should be INCREASED to simulate enemy slowness.
        // 80% Active Slow Multiplier means the enemy has been slowed
        // by 20% and should increase attack delay to 120%
        float appliedSlowMultiplier = 1 - activeSlowMultiplier;
        return attackDelay * (1 + appliedSlowMultiplier);
    }

    protected override void Awake()
    {
        base.Awake();

        Debug.Log($"{GetType().Name} Is Awake");

        Stats = GetComponent<Entity_Stats>();
        health = GetComponent<Enemy_Health>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void OnEnable()
    {
        // Subscribe to Action Event and observe.
        Player.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        // Unsubscribe from event.
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }

    protected override void HandleCollisionDetection()
    {
        // NOTE: The base Entity class uses nearly-identical boolean condition checks.
        base.HandleCollisionDetection();

        // Detect if there is a nearby ledge that is safe for the enemy to fall onto,
        // while the enemy is actively pursuing the player.
        if (primaryFallCheck != null)
        {
            BelowLedgeDetected = Physics2D.Raycast(primaryFallCheck.position, Vector2.down, fallCheckDistance, whatIsGround);
        }

        if (secondaryFallCheck != null)
        {
            BelowLedgeDetected = BelowLedgeDetected
                    || Physics2D.Raycast(secondaryFallCheck.position, Vector2.down, fallCheckDistance, whatIsGround);
        }
    }

    /// <summary>Must be called after <see cref="HandleCollisionDetection"/>.</summary>
    public virtual bool CanAggressivelyPursuePlayer()
    {
        // Enemies can pursue the player as long as there is a below ledge (so they don't blindly
        // walk off a cliff and fall to their death) and there isn't a wall obstructing them.
        return BelowLedgeDetected && !WallDetected;
    }

    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        // Movement Speeds should be DECREASED to simulate enemy slowness.
        // 20% Slow Multiplier should reduce speed to 80%
        activeSlowMultiplier = 1 - slowMultiplier;

        Anim.speed *= activeSlowMultiplier;

        yield return new WaitForSeconds(duration);

        // After slow effect has worn off, restore original values.
        StopSlowDown();
    }

    public override void StopSlowDown()
    {
        activeSlowMultiplier = 1f;
        Anim.speed = 1f;
        base.StopSlowDown();
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(DeadState);
    }

    private void HandlePlayerDeath()
    {
        stateMachine.ChangeState(IdleState);
    }

    // Determines whether the enemy can be counterattacked and stunned.
    public void EnableCounterWindow(bool enable) => canBeStunned = enable;

    public void TryEnterBattleState(Transform player)
    {
        if (stateMachine.CurrentState == BattleState) return;

        if (stateMachine.CurrentState == AttackState) return;

        PlayerRef = player;
        stateMachine.ChangeState(BattleState);
    }

    public Transform GetPlayerReference()
    {
        if (PlayerRef == null)
        {
            // Attempt to acquire Player reference from LOS raycast.
            // Does not work if the enemy is attacked from behind.
            PlayerRef = PlayerDetected().transform;
        }

        return PlayerRef;
    }

    public RaycastHit2D PlayerDetected()
    {
        // Detects whether the enemy has a direct Line of Sight (LOS) to the player.
        // If there is a wall breaking the LOS, then we return default (no player detected).
        RaycastHit2D hit = Physics2D.Raycast(
            playerCheck.position, Vector2.right * FacingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return default;
        }

        return hit;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (FacingDir * playerCheckDistance), playerCheck.position.y));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (FacingDir * attackDistance), playerCheck.position.y));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (FacingDir * minRetreatDistance), playerCheck.position.y));

        if (primaryFallCheck != null)
        {
            Gizmos.color = Color.hotPink;
            Gizmos.DrawLine(primaryFallCheck.position, primaryFallCheck.position + new Vector3(0, -fallCheckDistance));
        }

        if (secondaryFallCheck != null)
        {
            Gizmos.color = Color.hotPink;
            Gizmos.DrawLine(secondaryFallCheck.position, secondaryFallCheck.position + new Vector3(0, -fallCheckDistance));
        }
    }
}
