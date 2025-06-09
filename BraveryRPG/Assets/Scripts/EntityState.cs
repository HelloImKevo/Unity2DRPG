using UnityEngine;

/// <summary>
/// An abstract Entity possessing an FSM (Finite State Machine).
/// Concrete implementation examples: Player, Enemy.
/// </summary>
public abstract class EntityState
{
    protected StateMachine stateMachine;
    protected string animBoolName;

    protected Animator anim;
    protected Rigidbody2D rb;

    protected float stateTimer;
    protected bool onNextComboAttackReadyTrigger;
    protected bool onAnimationEndedTrigger;

    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
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
}
