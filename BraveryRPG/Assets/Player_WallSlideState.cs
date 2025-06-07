public class Player_WallSlideState : EntityState
{
    public Player_WallSlideState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        HandleWallSlide();

        if (!player.WallDetected)
        {
            // Detach from Wall and start Falling.
            stateMachine.ChangeState(player.FallState);
        }

        // If the player touches the ground, detach from Wall and transition to Idle.
        if (player.GroundDetected)
        {
            stateMachine.ChangeState(player.IdleState);
            player.Flip();
        }
    }

    private void HandleWallSlide()
    {
        if (player.MoveInput.y < 0)
        {
            // If player is pressing 'down' Input, maintain downward velocity.
            player.SetVelocity(player.MoveInput.x, rb.linearVelocity.y);
        }
        else
        {
            // Slow the player's descent, and allow player to detach from wall.
            player.SetVelocity(player.MoveInput.x, rb.linearVelocity.y * player.wallSlideSlowMultiplier);
        }
    }
}
