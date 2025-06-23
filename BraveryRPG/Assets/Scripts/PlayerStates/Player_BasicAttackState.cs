using UnityEngine;

public class Player_BasicAttackState : PlayerState
{
    // Player will be pushed forward a couple of pixels when attacking.
    private float attackVelocityTimer;
    // We start combo index with number1, this parameter is used in the Animator.
    private const int FirstComboIndex = 1;

    private bool comboAttackQueued;
    private int attackDir;

    [Range(1, 3)]
    private int comboIndex = FirstComboIndex;
    private readonly int comboLimit = 3;

    private float lastTimeAttacked;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("Adjusted combo limit to match attack velocity array!");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();

        comboAttackQueued = false;

        ResetComboIndexIfNeeded();
        SyncAttackSpeed();

        // Ternary syntax - This assumes that X is a value between -1.0 and +1.0
        // Define attack direction according to Input.
        attackDir = player.MoveInput.x != 0 ? (int)player.MoveInput.x : player.FacingDir;

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();

        HandleAttackVelocity();

        if (input.Player.Attack.WasPressedThisFrame())
        {
            QueueNextAttack();
        }

        HandleEndOfAttackExit();
    }

    public override void Exit()
    {
        base.Exit();

        comboIndex++;

        // Remember in-game timestamp of when we last attacked.
        lastTimeAttacked = Time.time;
    }

    private void HandleEndOfAttackExit()
    {
        if (onNextComboAttackReadyTrigger || onAnimationEndedTrigger)
        {
            if (comboAttackQueued)
            {
                // We need to re-enter the BasicAttackState using a deferred Coroutine
                // that will wait until the current Frame finishes, and then immediately
                // re-enter the BasicAttackState, to avoid Attack -> Idle -> Attack, which
                // causes a very slight animation jitter.
                anim.SetBool(animBoolName, false); // We'll the boolean true on the next frame.
                player.EnterAttackStateWithDelay();
            }
            // If the animation hasn't ended, let it finish (ex: the player sheaths their sword).
            else if (onAnimationEndedTrigger)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

    private void QueueNextAttack()
    {
        // Prevents the user from being able to spam endless infinite combo chains.
        // Once the 3-attack combo is finished, the player needs to wait briefly before
        // initiating a new attack combo.
        if (comboIndex < comboLimit)
        {
            comboAttackQueued = true;
        }
    }

    private void HandleAttackVelocity()
    {
        attackVelocityTimer -= Time.deltaTime;
        // TODO: This is used in a couple places - can be refactored into a shared location.
        attackVelocityTimer = Mathf.Max(0, attackVelocityTimer);

        if (attackVelocityTimer <= 0)
        {
            player.SetVelocity(0, rb.linearVelocity.y);
        }
    }

    private void ApplyAttackVelocity()
    {
        Vector2 attackVelocity = player.attackVelocity[comboIndex - 1];

        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(attackVelocity.x * attackDir, attackVelocity.y);
    }

    private void ResetComboIndexIfNeeded()
    {
        // If time we attacked was long ago, we reset combo index.
        if (Time.time > lastTimeAttacked + player.comboResetTime)
        {
            comboIndex = FirstComboIndex;
        }

        if (comboIndex > comboLimit)
        {
            comboIndex = FirstComboIndex;
        }
    }
}
