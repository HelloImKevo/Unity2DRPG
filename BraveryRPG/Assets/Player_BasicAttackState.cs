using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    // Player will be pushed forward a couple of pixels when attacking.
    private float attackVelocityTimer;
    // We start combo index with number1, this parameter is used in the Animator.
    private const int FirstComboIndex = 1;
    [Range(1, 3)]
    private int comboIndex = FirstComboIndex;
    private readonly int comboLimit = 3;

    private float lastTimeAttacked;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        if (comboLimit != player.attackVelocity.Length)
        {
            Debug.LogWarning("Adjusted combo limit, according to attack velocity array!");
            comboLimit = player.attackVelocity.Length;
        }
    }

    public override void Enter()
    {
        base.Enter();

        ResetComboIndexIfNeeded();

        anim.SetInteger("basicAttackIndex", comboIndex);
        ApplyAttackVelocity();
    }

    public override void Update()
    {
        base.Update();

        HandleAttackVelocity();

        if (triggerCalled)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        comboIndex++;

        // Remember in-game timestamp of when we last attacked.
        lastTimeAttacked = Time.time;
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
        player.SetVelocity(attackVelocity.x * player.FacingDir, attackVelocity.y);
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
