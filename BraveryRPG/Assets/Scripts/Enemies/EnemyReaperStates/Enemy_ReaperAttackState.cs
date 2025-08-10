using UnityEngine;

public class Enemy_ReaperAttackState : Enemy_AttackState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering REAPER ATTACK state");
        }
    }

    public override void Update()
    {
        if (onAnimationEndTriggered)
        {
            if (enemyReaper.ShouldTeleport())
            {
                stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            }
            else
            {
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
            }
        }
    }
}
