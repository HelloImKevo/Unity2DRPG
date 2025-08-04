using UnityEngine;

public class Enemy_ArcherElfBattleState : Enemy_BattleState
{
    private bool canFlip;
    private bool reachedDeadEnd;

    public Enemy_ArcherElfBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    protected override bool ShouldAttackIfPossible()
    {
        return WithinAttackRange() && !ShouldWalkAwayFromPlayer();
    }

    protected override bool ShouldWalkAwayFromPlayer()
    {
        float distanceToPlayer = DistanceToPlayer();
        float attackRange = enemy.attackDistance;

        bool isPlayerWithinNarrowBand =
            distanceToPlayer > (attackRange * 0.7f)
            && distanceToPlayer < (attackRange * 0.8f);

        if (!enemy.CanAggressivelyPursuePlayer()) return false;

        // If the player is in the outer reaches of the enemy's max attack distance,
        // have the enemy strafe backwards from the player.
        return isPlayerWithinNarrowBand;
    }

    // public override void Update()
    // {
    //     base.Update();

    //     stateTimer -= Time.deltaTime;
    //     UpdateAnimationParameters();

    //     if (enemy.groundDetected == false || enemy.wallDetected)
    //     {
    //         reachedDeadEnd = true;
    //     }

    //     if (enemy.PlayerDetected())
    //     {
    //         UpdateTargetIfNeeded();
    //         UpdateBattleTimer();
    //     }

    //     if (BattleTimeIsOver())
    //     {
    //         stateMachine.ChangeState(enemy.idleState);
    //     }

    //     if (CanPerformAttack())
    //     {
    //         if (enemy.PlayerDetected() == false && canFlip)
    //         {
    //             enemy.HandleFlip(DirectionToPlayer());
    //             canFlip = false;
    //         }

    //         enemy.SetVelocity(0, rb.linearVelocity.y);

    //         if (WithinAttackRange() && enemy.PlayerDetected())
    //         {
    //             canFlip = true;
    //             lastTimeAttacked = Time.time;
    //             stateMachine.ChangeState(enemy.attackState);
    //         }
    //     }
    //     else
    //     {
    //         bool shouldWalkAway = reachedDeadEnd == false && DistanceToPlayer() < (enemy.attackDistance * 0.85f);

    //         if (shouldWalkAway)
    //         {
    //             enemy.SetVelocity((enemy.GetBattleMoveSpeed() * -1) * DirectionToPlayer(), rb.linearVelocity.y);
    //         }
    //         else
    //         {
    //             enemy.SetVelocity(0, rb.linearVelocity.y);

    //             if (enemy.PlayerDetected() == false)
    //             {
    //                 enemy.HandleFlip(DirectionToPlayer());
    //             }
    //         }
    //     }
    // }
}
