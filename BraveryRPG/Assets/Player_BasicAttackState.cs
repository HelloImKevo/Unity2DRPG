using UnityEngine;

public class Player_BasicAttackState : EntityState
{
    // Player will be pushed forward a couple of pixels when attacking.
    private float attackVelocityTimer;

    public Player_BasicAttackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        GenerateAttackVelocity();
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

    private void GenerateAttackVelocity()
    {
        attackVelocityTimer = player.attackVelocityDuration;
        player.SetVelocity(player.attackVelocity.x * player.FacingDir, player.attackVelocity.y);
    }
}
