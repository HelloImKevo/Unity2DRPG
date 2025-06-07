using UnityEngine;

public class Player_DashState : EntityState
{
    private float originalGravityScale;
    // Used to prevent a rare race condition bug.
    private int dashDir;

    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        dashDir = player.FacingDir;
        stateTimer = player.dashDuration;

        originalGravityScale = rb.gravityScale;
        // Prevent player from gradually descending while dashing (this maintains
        // a constant horizontal linear movement from ledges).
        rb.gravityScale = 0;
    }

    public override void Update()
    {
        base.Update();

        CancelDashIfNeeded();

        player.SetVelocity(player.dashSpeed * dashDir, 0);

        if (stateTimer <= 0)
        {
            if (player.GroundDetected)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.FallState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();

        // Prevent rapid acceleration of horizontal velocity when player is not Grounded,
        // and we transition to the Falling state.
        player.SetVelocity(0, 0);
        rb.gravityScale = originalGravityScale;
    }

    private void CancelDashIfNeeded()
    {
        if (player.WallDetected)
        {
            if (player.GroundDetected)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
        }
    }
}
