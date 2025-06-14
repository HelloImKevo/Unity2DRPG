using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState IdleState { get; protected set; }
    public Enemy_MoveState MoveState { get; protected set; }
    public Enemy_AttackState AttackState { get; protected set; }
    public Enemy_BattleState BattleState { get; protected set; }

    [Header("Enemy Battle Details")]
    public float battleMoveSpeed = 3f;
    public float attackDistance = 2;
    public float battleTimeDuration = 5f;
    public float minRetreatDistance = 1f;
    [Tooltip("Distance of the backstep retreat effect")]
    public Vector2 retreatVelocity = new(5f, 3f);

    [Tooltip("Draws a downward Raycast to prevent enemy from pursuing the player off a ledge and falling into the abyss")]
    [SerializeField] private Transform primaryFallCheck;
    [SerializeField] private Transform secondaryFallCheck;
    public float fallCheckDistance = 10f;
    public bool BelowLedgeDetected { get; private set; }

    [Header("Enemy Movement Details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;

    [Range(0, 2)]
    [Tooltip("Used to speed up or slow down animations for faster or slower enemies")]
    public float moveAnimSpeedMultiplier = 1;

    [SerializeField] private float redColorDuration = 1;

    [Header("Player Detection")]
    [Tooltip("The 'Player' Layer")]
    [SerializeField] private LayerMask whatIsPlayer;
    [Tooltip("The point from which to draw the Raycast for LOS Player detection")]
    [SerializeField] private Transform playerCheck;
    [SerializeField] private float playerCheckDistance = 10;

    public float timer;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        ChangeColorIfNeeded();
    }

    protected override void HandleCollisionDetection()
    {
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

    private void ChangeColorIfNeeded()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(0, timer);

        // if (timer <= 0 && sr.color != Color.white)
        // {
        //     TurnWhite();
        // }
    }

    public void TakeDamage()
    {
        Debug.Log(gameObject.name + " took some damage!");

        // sr.color = Color.red;

        // Alternative example to Invoke function after time elapsed:
        // Invoke(nameof(TurnWhite), redColorDuration);

        // Reset cooldown timer back to default.
        timer = redColorDuration;
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
