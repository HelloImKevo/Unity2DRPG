using UnityEngine;

public class Player_JumpState : EntityState
{
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Make object go up, increase Y velocity.
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
    }

    public override void Update()
    {
        base.Update();

        // If Y velocity goes down, character is falling, transfer to Fall state.
        if (rb.linearVelocity.y < 0)
        {
            stateMachine.ChangeState(player.FallState);
        }
    }
}
