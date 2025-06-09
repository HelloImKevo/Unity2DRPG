using UnityEngine;

public class Player_JumpState : Player_AiredState
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
        // We need to make sure we are not in Jump Attack state when we transfer
        // to the Falling state, otherwise we can enter JumpAttack -> Falling
        // within the same Frame; we can debug this with Time.frameCount
        if (rb.linearVelocity.y < 0 && stateMachine.CurrentState != player.JumpAttackState)
        {
            stateMachine.ChangeState(player.FallState);
        }
    }
}
