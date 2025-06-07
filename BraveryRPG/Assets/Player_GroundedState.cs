public class Player_GroundedState : EntityState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // Prevents flicker of Animation from WallSlide -> Idle -> Fall -> Idle.
        if (rb.linearVelocity.y < 0 && !player.GroundDetected)
        {
            stateMachine.ChangeState(player.FallState);
        }

        if (input.Player.Jump.WasPerformedThisFrame())
        {
            stateMachine.ChangeState(player.JumpState);
        }
    }
}
