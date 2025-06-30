public class Player_GroundedState : PlayerState
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

        if (input.Player.Jump.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.JumpState);
        }

        if (input.Player.Attack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.BasicAttackState);
        }

        if (input.Player.Counterattack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.CounterattackState);
        }

        if (input.Player.RangeAttack.WasPressedThisFrame())
        {
            stateMachine.ChangeState(player.ThrowSwordState);
        }
    }
}
