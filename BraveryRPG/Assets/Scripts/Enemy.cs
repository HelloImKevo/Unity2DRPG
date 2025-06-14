using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState IdleState { get; protected set; }
    public Enemy_MoveState MoveState { get; protected set; }
    public Enemy_AttackState AttackState { get; protected set; }
    public Enemy_BattleState BattleState { get; protected set; }

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

    public RaycastHit2D PlayerDetection()
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

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (FacingDir * playerCheckDistance), playerCheck.position.y));
    }
}
