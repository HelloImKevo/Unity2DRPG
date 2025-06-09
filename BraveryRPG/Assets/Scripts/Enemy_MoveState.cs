using UnityEngine;

public class Enemy_MoveState : EnemyState
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

        enemy.SetVelocity(enemy.moveSpeed * enemy.FacingDir, rb.linearVelocity.y);

        if (enemy.GroundDetected == false || enemy.WallDetected)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }
    }
}
