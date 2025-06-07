using UnityEngine;

public class Player_AiredState : EntityState
{
    public Player_AiredState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        if (player.MoveInput.x != 0)
        {
            player.SetVelocity(player.MoveInput.x * (player.moveSpeed * player.inAirMoveMultiplier), rb.linearVelocity.y);
        }
    }
}
