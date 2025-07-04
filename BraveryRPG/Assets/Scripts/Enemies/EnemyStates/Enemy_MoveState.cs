using UnityEngine;

public class Enemy_MoveState : Enemy_GroundedState
{
    public Enemy_MoveState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.WallDetected || !enemy.GroundDetected)
        {
            enemy.Flip();
        }
    }

    public override void Update()
    {
        base.Update();

        enemy.SetVelocity(enemy.GetMoveSpeed() * enemy.FacingDir, rb.linearVelocity.y);

        if (enemy.GroundDetected == false || enemy.WallDetected)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
    }
}
