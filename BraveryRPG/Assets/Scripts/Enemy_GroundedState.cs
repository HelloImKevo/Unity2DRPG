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
        if (enemy.PlayerDetection())
        {
            stateMachine.ChangeState(enemy.BattleState);
        }
    }
}
