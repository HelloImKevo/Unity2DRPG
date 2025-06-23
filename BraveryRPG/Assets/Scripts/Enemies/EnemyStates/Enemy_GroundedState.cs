using UnityEngine;

public class Enemy_GroundedState : EnemyState
{
    public Enemy_GroundedState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        base.Update();

        // If enemy detects player, state machine changes to Battle state.
        // Note: This strategy would not work for Flying enemies.
        if (enemy.PlayerDetected() && enemy.BelowLedgeDetected)
        {
            stateMachine.ChangeState(enemy.BattleState);
        }
    }
}
