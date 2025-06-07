using UnityEngine;

/// <summary>
/// An Entity possessing an FSM (Finite State Machine).
/// </summary>
public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;
    protected PlayerInputSet input;

    public EntityState(Player player, StateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;

        anim = player.Anim;
        rb = player.Rb;
        input = player.Input;
    }

    public virtual void Enter()
    {
        // Every time state will be changed, enter will be called.
        anim.SetBool(animBoolName, true);
    }

    public virtual void Update()
    {
        // Run logic of the state here.
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public virtual void Exit()
    {
        // This will be called every time we exit state and change to a new one.
        anim.SetBool(animBoolName, false);
    }
}
