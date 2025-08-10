using UnityEngine;

public class Enemy_Reaper : Enemy, ICounterable
{
    private Entity_VisionCone visionCone;

    protected override void Awake()
    {
        base.Awake();

        shouldLogStateTransitions = true;

        visionCone = GetComponent<Entity_VisionCone>();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "idle"); // We aren't applying an animation on death.
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{gameObject.name} calling START");

        stateMachine.Initialize(IdleState);
    }

    #region ICounterable

    public bool CanBeCountered { get => canBeStunned; }

    public void OnReceiveCounterattack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }

    #endregion
}
