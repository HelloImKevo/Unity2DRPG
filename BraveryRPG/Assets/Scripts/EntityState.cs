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

    protected float stateTimer;
    protected bool onNextComboAttackReadyTrigger;
    protected bool onAnimationEndedTrigger;

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
        onNextComboAttackReadyTrigger = false;
        onAnimationEndedTrigger = false;
    }

    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        stateTimer = Mathf.Max(0, stateTimer);

        // Run logic of the state here.
        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        // Enable the user to interrupt an Attack animation by Dashing.
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    public virtual void Exit()
    {
        // This will be called every time we exit state and change to a new one.
        anim.SetBool(animBoolName, false);
    }

    // Currently enables the smooth chaining of attacks in a combo, but could
    // be extended to support other kinds of interruptible Actions, like being
    // able to Jump or Dash, before the 3rd Heavy Slash animation is finished.
    public void CallOnNextActionInputReadyTrigger()
    {
        onNextComboAttackReadyTrigger = true;
    }

    public void CallOnAnimationEndedTrigger()
    {
        onAnimationEndedTrigger = true;
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
