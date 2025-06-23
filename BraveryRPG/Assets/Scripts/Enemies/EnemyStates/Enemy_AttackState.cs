using UnityEngine;

public class Enemy_AttackState : EnemyState
{
    public Enemy_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
    }

    public override void Update()
    {
        base.Update();

        if (onAnimationEndedTrigger)
        {
            // Switch to Battle state (instead of Idle state), to prevent
            // player from running through the enemy to reset aggro.
            stateMachine.ChangeState(enemy.BattleState);
        }
    }

    public override void Exit()
    {
        base.Exit();

        LastTimeAttackPerformed = Time.time;
    }
}
