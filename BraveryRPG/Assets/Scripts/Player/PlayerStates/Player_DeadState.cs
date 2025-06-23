using UnityEngine;

public class Player_DeadState : PlayerState
{
    public Player_DeadState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // Stop receiving inputs from the user.
        input.Disable();

        // Disable physics simulation, otherwise enemy will immediately detect player.
        rb.simulated = false;
    }
}
