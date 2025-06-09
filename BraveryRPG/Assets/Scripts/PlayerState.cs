using UnityEngine;

/// <summary>
/// An Entity possessing an FSM (Finite State Machine).
/// </summary>
public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;

    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.Anim;
        rb = player.Rb;
        input = player.Input;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        // Run logic of the state here.
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // Enable the user to interrupt an Attack animation by Dashing.
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanDash()
    {
        if (player.WallDetected || stateMachine.CurrentState == player.DashState)
        {
            return false;
        }
        return true;
    }
}
