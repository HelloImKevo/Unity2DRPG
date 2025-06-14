using UnityEngine;

/// Summary:
///     Base class for Player and Enemy actors.
public class Entity : MonoBehaviour
{
    public Animator Anim { get; protected set; }
    public Rigidbody2D Rb { get; protected set; }

    protected StateMachine stateMachine;

    private bool facingRight = true;
    // 1 = Right, -1 = Left.
    public int FacingDir { get; private set; } = 1;

    [Header("Collision detection")]
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    // Part of our Wall-Slide system.
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform primaryGroundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool GroundDetected { get; private set; }
    public bool WallDetected { get; private set; }

    protected virtual void Awake()
    {
        // Must be initialized before the StateMachine.
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        HandleCollisionDetection();
        stateMachine.UpdateActiveState();
    }

    public void CallOnNextActionInputReadyTrigger()
    {
        stateMachine.CurrentState.CallOnNextActionInputReadyTrigger();
    }

    public void CallOnAnimationEndedTrigger()
    {
        stateMachine.CurrentState.CallOnAnimationEndedTrigger();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    private void HandleCollisionDetection()
    {
        GroundDetected = Physics2D.Raycast(primaryGroundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        WallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);

        if (secondaryWallCheck != null)
        {
            WallDetected = WallDetected
                    && Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);
        }
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

    protected virtual void OnDrawGizmos()
    {
        // Enable us to visualize the Raycast in the Unity Editor (does not affect gameplay).
        Gizmos.DrawLine(primaryGroundCheck.position, primaryGroundCheck.position + new Vector3(0, -groundCheckDistance));
        Gizmos.DrawLine(primaryWallCheck.position, primaryWallCheck.position + new Vector3(wallCheckDistance * FacingDir, 0));
        if (secondaryWallCheck != null)
        {
            Gizmos.DrawLine(secondaryWallCheck.position, secondaryWallCheck.position + new Vector3(wallCheckDistance * FacingDir, 0));
        }
    }
}
