using UnityEngine;

public class Enemy_ReaperBattleState : Enemy_BattleState
{
    private Enemy_Reaper enemyReaper;

    public Enemy_ReaperBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering REAPER BATTLE state");
        }

        stateTimer = enemyReaper.maxBattleIdleTime;
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (stateTimer < 0)
        {
            stateMachine.ChangeState(enemyReaper.reaperTeleportState);
        }

        if (enemy.PlayerDetected())
        {
            UpdateTargetIfNeeded();
        }

        if (WithinAttackRange() && enemy.PlayerDetected() && CanPerformAttack())
        {
            stateMachine.ChangeState(enemyReaper.reaperAttackState);
        }
        else
        {
            float xVelocity = enemy.canChasePlayer ? enemy.GetBattleMoveSpeed() : 0.0001f;

            if (!enemy.GroundDetected)
            {
                xVelocity = 0.00001f;
            }

            enemy.SetVelocity(xVelocity * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }
}
