using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInputSet Input { get; private set; }
    private StateMachine stateMachine;
    public Player_IdleState IdleState { get; private set; }
    public Player_MoveState MoveState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DashState DashState { get; private set; }
    public Player_BasicAttackState BasicAttackState { get; private set; }
    public Player_WallSlideState WallSlideState { get; private set; }
    public Player_WallJumpState WallJumpState { get; private set; }

    public Animator Anim { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    // Forward movement applied to player when attack is initiated.
    public float attackVelocityDuration = 0.1f;
    public float comboResetTime = 0.75f;
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;

    [Header("Movement details")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = 0.5f;
    [Space]
    public float dashDuration = 0.25f;
    public float dashSpeed = 20f;

    public Vector2 MoveInput { get; private set; }
    private float xInput;
    private bool facingRight = true;
    // 1 = Right, -1 = Left.
    public int FacingDir { get; private set; } = 1;
    private bool canMove = true;
    private bool canJump = true;

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    // Part of our Wall-Slide system.
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool GroundDetected { get; private set; }
    public bool WallDetected { get; private set; }

    private void Awake()
    {
        // Must be initialized before the StateMachine.
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        Input = new PlayerInputSet();

        stateMachine = new StateMachine();
        IdleState = new Player_IdleState(this, stateMachine, "idle");
        MoveState = new Player_MoveState(this, stateMachine, "move");
        JumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        FallState = new Player_FallState(this, stateMachine, "jumpFall");
        DashState = new Player_DashState(this, stateMachine, "dash");
        BasicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        WallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        WallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
    }

    private void OnEnable()
    {
        Input.Enable();

        // Subscribe to New Input System 'Movement' action map.
        Input.Player.Movement.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        Input.Player.Movement.canceled += ctx => MoveInput = Vector2.zero;
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(IdleState);
    }

    // Update is called once per frame
    void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
        // HandleInput();
        // HandleMovement();
        // HandleAnimations();
    }

    public void CallAnimationTrigger()
    {
        stateMachine.CurrentState.CallAnimationTrigger();
    }

    public void DamageEnemies()
    {
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, whatIsEnemy);

        foreach (Collider2D enemy in enemyColliders)
        {
            enemy.GetComponent<Enemy>().TakeDamage();
        }
    }

    public void EnableMovementAndJump(bool enable)
    {
        // canMove = enable;
        // canJump = enable;
    }

    private void TryToJump()
    {
        if (GroundDetected && canJump)
        {
            Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, jumpForce);
        }
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleCollisionDetection()
    {
        GroundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        WallDetected = Physics2D.Raycast(transform.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleFlip(float xVelocity)
    {
        if (xVelocity > 0 && facingRight == false)
        {
            Flip();
        }
        else if (xVelocity < 0 && facingRight == true)
        {
            Flip();
        }
    }

    public void Flip()
    {
        // Flip the sprite around the Y-Axis.
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
        FacingDir *= -1;
    }

    private void OnDrawGizmos()
    {
        // Enable us to visualize the Raycast in the Unity Editor (does not affect gameplay).
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * FacingDir, 0));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
