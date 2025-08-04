using UnityEngine;

public class Enemy_IdleState : Enemy_GroundedState
{
    public Enemy_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering IDLE state");
        }

        stateTimer = enemy.idleTime;
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer <= 0)
        {
            stateMachine.ChangeState(enemy.MoveState);
        }
    }
}
