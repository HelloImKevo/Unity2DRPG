using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;

    public UI UserInterface { get; private set; }

    public PlayerInputSet Input { get; private set; }

    public Player_VFX Vfx { get; private set; }
    public Player_Stats Stats { get; private set; }
    public Player_Health Health { get; private set; }
    public Player_SkillManager SkillManager { get; private set; }
    public Entity_StatusHandler StatusHandler { get; private set; }
    public Player_Combat Combat { get; private set; }
    public Inventory_Player Inventory { get; private set; }
    public Player_QuestManager questManager { get; private set; }

    #region FSM State Variables

    public Player_IdleState IdleState { get; private set; }
    public Player_MoveState MoveState { get; private set; }
    public Player_JumpState JumpState { get; private set; }
    public Player_FallState FallState { get; private set; }
    public Player_DashState DashState { get; private set; }
    public Player_BasicAttackState BasicAttackState { get; private set; }
    public Player_JumpAttackState JumpAttackState { get; private set; }
    public Player_WallSlideState WallSlideState { get; private set; }
    public Player_WallJumpState WallJumpState { get; private set; }
    public Player_DeadState DeadState { get; private set; }
    public Player_CounterattackState CounterattackState { get; private set; }
    public Player_ThrowSwordState ThrowSwordState { get; private set; }
    public Player_DomainExpansionState domainExpansionState { get; private set; }

    #endregion

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity = new(3f, 4f);
    // Forward movement applied to player when attack is initiated.
    public float attackVelocityDuration = 0.1f;
    // Lower values mean the player needs to press the attack button more quickly.
    // Higher values mean the combo can be continued after a longer window.
    public float comboResetTime = 0.6f;
    // Reminder: Coroutines require MonoBehaviour (so we can't put this in the EntityState).
    private Coroutine queuedAttackWorker;

    [Header("Ultimate ability details")]
    public float riseSpeed = 20f;
    public float riseMaxDistance = 3f;

    [Header("Player Movement Details")]
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
    public Vector2 MousePosition { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        // This performs some heavy lifting - never do this in Update()
        UserInterface = FindFirstObjectByType<UI>();

        Input = new PlayerInputSet();
        UserInterface.SetupControlsUI(Input);

        Vfx = GetComponent<Player_VFX>();
        Stats = GetComponent<Player_Stats>();
        Health = GetComponent<Player_Health>();
        SkillManager = GetComponent<Player_SkillManager>();
        StatusHandler = GetComponent<Entity_StatusHandler>();
        Combat = GetComponent<Player_Combat>();
        Inventory = GetComponent<Inventory_Player>();
        questManager = GetComponent<Player_QuestManager>();

        IdleState = new Player_IdleState(this, stateMachine, "idle");
        MoveState = new Player_MoveState(this, stateMachine, "move");
        JumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        FallState = new Player_FallState(this, stateMachine, "jumpFall");
        DashState = new Player_DashState(this, stateMachine, "dash");
        BasicAttackState = new Player_BasicAttackState(this, stateMachine, "basicAttack");
        JumpAttackState = new Player_JumpAttackState(this, stateMachine, "jumpAttack");
        WallSlideState = new Player_WallSlideState(this, stateMachine, "wallSlide");
        WallJumpState = new Player_WallJumpState(this, stateMachine, "jumpFall");
        DeadState = new Player_DeadState(this, stateMachine, "dead");
        CounterattackState = new Player_CounterattackState(this, stateMachine, "counterattack");
        ThrowSwordState = new Player_ThrowSwordState(this, stateMachine, "throwSword");
        domainExpansionState = new Player_DomainExpansionState(this, stateMachine, "jumpFall");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }

    /// <summary>Use sparingly - this lookup is not highly-performant!</summary>
    public static Player GetInstance(int playerNumber = 1)
    {
        // If we ever add multiplayer support, add additional logic ...

        return FindFirstObjectByType<Player>();
    }

    // TODO: Add some kind of teleportation VFX like a glow or puff of smoke.
    public void TeleportPlayer(Vector3 position)
    {
        Debug.Log($"{GetType().Name}.TeleportPlayer() -> Teleporting to Position = {position}");
        transform.position = position;
    }

    // TODO: Implement the same pattern used to slow down the enemy - requires a lot of refactor.
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        // Speeds should be DECREASED to simulate player slowness.
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = Anim.speed;
        Vector2 originalWallJumpForce = wallJumpForce;
        Vector2 originalJumpAttackVelocity = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = Vector2Utils.DeepCopy(attackVelocity);

        // 20% Slow Multiplier should reduce speed to 80%
        float speedMultiplier = 1 - slowMultiplier;
        moveSpeed *= speedMultiplier;
        jumpForce *= speedMultiplier;
        Anim.speed *= speedMultiplier;
        wallJumpForce *= speedMultiplier;
        jumpAttackVelocity *= speedMultiplier;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        }

        // Apply the slow effect for duration seconds.
        yield return new WaitForSeconds(duration);

        // After slow effect has worn off, restore original values.
        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        Anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJumpForce;
        jumpAttackVelocity = originalJumpAttackVelocity;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            // Reset each Vector2 to the deep copy references.
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public override void EntityDeath()
    {
        base.EntityDeath();

        // Emit player death event to all subscribers.
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(DeadState);
    }

    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackWorker != null)
        {
            StopCoroutine(queuedAttackWorker);
        }

        queuedAttackWorker = StartCoroutine(EnterAttackStateWithDelayWorker());
    }

    private IEnumerator EnterAttackStateWithDelayWorker()
    {
        // We want to make the Attack animation boolean true on the Next Frame.
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(BasicAttackState);
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    private void TryInteract()
    {
        Transform closest = null;
        float closestDistance = Mathf.Infinity;
        // NOTE: This detection radius should roughly match the Collision Box 2D
        // for interactive NPCs like the Blacksmith and Merchant.
        Collider2D[] objectsAround = Physics2D.OverlapCircleAll(transform.position, 1f);

        foreach (var target in objectsAround)
        {
            IInteractable interactable = target.GetComponent<IInteractable>();

            if (interactable == null) continue;

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = target.transform;
            }
        }

        if (closest == null) return;

        closest.GetComponent<IInteractable>().Interact();
    }

    #region Input Controls

    // Entry point for the new Unity Input System.
    private void OnEnable()
    {
        Input.Enable();

        // Capture mouse position (used to calculate parabolic trajectory of sword throw).
        Input.Player.Mouse.performed += ctx => MousePosition = ctx.ReadValue<Vector2>();

        // Subscribe to New Input System 'Movement' action map.
        Input.Player.Movement.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
        Input.Player.Movement.canceled += ctx => MoveInput = Vector2.zero;

        // Currently, you can only have one of these spells unlocked:
        // Shard or Time Echo - if this system changes, we need to implement
        // a spell cycle system to keep track of which spell is selected.
        Input.Player.Spell.performed += ctx => SkillManager.Shard.TryUseSkill();
        Input.Player.Spell.performed += ctx => SkillManager.TimeEcho.TryUseSkill();

        // Interact with NPCs, Chests, Doors, Environment Objects, etc.
        Input.Player.Interact.performed += ctx => TryInteract();

        // Quick Slot Items - Potions, Scrolls, etc.
        Input.Player.QuickItemSlot_1.performed += ctx => Inventory.TryUseQuickItemInSlot(1);
        Input.Player.QuickItemSlot_2.performed += ctx => Inventory.TryUseQuickItemInSlot(2);
    }

    private void OnDisable()
    {
        Input.Disable();
    }

    #endregion
}
