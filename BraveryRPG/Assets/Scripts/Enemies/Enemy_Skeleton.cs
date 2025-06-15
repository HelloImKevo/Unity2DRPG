using UnityEngine;

public class Enemy_Skeleton : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        Debug.Log("Enemy_Skeleton Is Awake");

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "idle"); // We aren't applying an animation on death.
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }

    protected override void Update()
    {
        base.Update();
    }
}
