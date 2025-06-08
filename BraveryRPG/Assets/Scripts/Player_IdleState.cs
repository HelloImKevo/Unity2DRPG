using UnityEngine;

public class Player_IdleState : Player_GroundedState
{
    public Player_IdleState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Reset velocity when entering Idle (to prevent player from Sliding along
        // the Ground when landing from Aired state).
        player.SetVelocity(0, rb.linearVelocity.y);
    }

    public override void Update()
    {
        base.Update();

        // Stop player from being able to 'Continue Running' into a Wall.
        if (player.MoveInput.x == player.FacingDir && player.WallDetected)
        {
            return;
        }

        if (player.MoveInput.x != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
