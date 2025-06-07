using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerInputSet Input { get; private set; }
    private StateMachine stateMachine;
    public Player_IdleState IdleState { get; private set; }
    public Player_MoveState MoveState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }

    public Animator Anim { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    [Header("Attack details")]
    [SerializeField] private float attackRadius;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask whatIsEnemy;

    [Header("Movement details")]
    public float moveSpeed = 8f;
    public float jumpForce = 12f;
    [Range(0, 1)]
    public float inAirMoveMultiplier = 0.7f;
    public Vector2 MoveInput { get; private set; }
    private float xInput;
    private bool facingRight = true;
    private bool canMove = true;
    private bool canJump = true;

    [Header("Collision details")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool GroundDetected { get; private set; }

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

    private void HandleInput()
    {
        xInput = UnityEngine.Input.GetAxisRaw("Horizontal");

        if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
        {
            TryToJump();
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.Mouse0))
        {
            TryToAttack();
        }
    }

    private void TryToAttack()
    {
        if (GroundDetected)
        {
            Anim.SetTrigger("attack");
        }
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

    private void HandleMovement()
    {
        if (canMove)
        {
            Rb.linearVelocity = new Vector2(xInput * moveSpeed, Rb.linearVelocity.y);
        }
        else
        {
            // This should stop the character when it attacks.
            Rb.linearVelocity = new Vector2(0, Rb.linearVelocity.y);
        }
    }

    private void HandleCollisionDetection()
    {
        GroundDetected = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
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

    [ContextMenu("Flip")]
    private void Flip()
    {
        // Flip the sprite around the Y-Axis.
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }

    private void OnDrawGizmos()
    {
        // Enable us to visualize the Raycast in the Unity Editor (does not affect gameplay).
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
