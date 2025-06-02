using UnityEngine;

public class Player_MoveState : EntityState
{
    public Player_MoveState(Player player, StateMachine stateMachine, string stateName) : base(player, stateMachine, stateName)
    {
    }

    public override void Update()
    {
        base.Update();

        // If there is no horizontal movement input, transition to Idle State.
        if (player.MoveInput.x == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
