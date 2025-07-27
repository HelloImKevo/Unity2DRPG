using System;
using System.Collections;
using UnityEngine;

/// Summary:
///     Base class for Player and Enemy actors.
public class Entity : MonoBehaviour
{
    public event Action OnFlipped;

    public Animator Anim { get; protected set; }
    public Rigidbody2D Rb { get; protected set; }

    public Entity_SFX Sfx { get; protected set; }

    protected StateMachine stateMachine;

    private bool facingRight = true;
    // 1 = Right, -1 = Left.
    public int FacingDir { get; private set; } = 1;

    [Header("Collision detection")]
    public LayerMask whatIsGround;
    [SerializeField] private float groundCheckDistance;
    // Part of our Wall-Slide system.
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private Transform primaryGroundCheck;
    [SerializeField] private Transform primaryWallCheck;
    [SerializeField] private Transform secondaryWallCheck;
    public bool GroundDetected { get; private set; }
    public bool WallDetected { get; private set; }

    private Coroutine knockbackCoroutine;
    private bool isKnockedBack;

    public float activeSlowMultiplier { get; protected set; } = 1f;

    private Coroutine slowDownCoroutine;

    protected virtual void Awake()
    {
        // Must be initialized before the StateMachine.
        Anim = GetComponentInChildren<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        Sfx = GetComponent<Entity_SFX>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Collision detections should be calculated before instructing the
        // FSM mechanism to evaluate state transitions.
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

    public virtual void EntityDeath()
    {
        // Override in subclasses as needed.
    }

    /// <summary>
    /// Slow down the Entity with a Time-Stop / Time-Dilation effect.
    /// Alters the enemy move, attack and animation speeds.
    /// </summary>
    public void SlowDownEntity(float duration, float slowMultiplier, bool canOverrideSlowEffect = false)
    {
        if (slowDownCoroutine != null)
        {
            if (canOverrideSlowEffect)
            {
                StopCoroutine(slowDownCoroutine);
            }
            else
            {
                // There is an existing slow-down effect applied to this entity,
                // and the existing effect cannot be overridden.
                return;
            }
        }

        Debug.Log($"{gameObject.name} -> Starting 'SlowDownEntityCo' Coroutine, Multiplier = {slowMultiplier} ...");
        slowDownCoroutine = StartCoroutine(SlowDownEntityCo(duration, slowMultiplier));
    }

    protected virtual IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        // Should override in subclasses.
        // By default, yield return null just waits for the next frame.
        yield return null;
    }

    /// <summary>
    /// Immediately stops the 'Slow Down the Entity' coroutine, alleviating
    /// effects from chill, freeze, and time dilation.
    /// </summary>
    public virtual void StopSlowDown()
    {
        slowDownCoroutine = null;
    }

    #region: Knockback Effect When Damaged

    public void ReceiveKnockback(Vector2 knockback, float duration)
    {
        // Check if task is already running, and stop it.
        if (knockbackCoroutine != null) StopCoroutine(knockbackCoroutine);

        // Spawn the new coroutine.
        knockbackCoroutine = StartCoroutine(LaunchKnockbackTask(knockback, duration));
    }

    private IEnumerator LaunchKnockbackTask(Vector2 knockback, float duration)
    {
        isKnockedBack = true;
        Rb.linearVelocity = knockback;

        yield return new WaitForSeconds(duration);

        // Reset velocity when coroutine is finished.
        Rb.linearVelocity = Vector2.zero;
        isKnockedBack = false;
    }

    #endregion

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnockedBack) return;

        Rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlip(xVelocity);
    }

    protected virtual void HandleCollisionDetection()
    {
        GroundDetected = Physics2D.Raycast(primaryGroundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        WallDetected = Physics2D.Raycast(primaryWallCheck.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);

        if (secondaryWallCheck != null)
        {
            WallDetected = WallDetected
                    || Physics2D.Raycast(secondaryWallCheck.position, Vector2.right * FacingDir, wallCheckDistance, whatIsGround);
        }
    }

    // Summary:
    //     1 = Right, -1 = Left.
    public void HandleFlip(float xVelocity)
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

        // Notify subscribers that this entity has flipped.
        OnFlipped?.Invoke();
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
