using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;

    /// <summary>
    /// Support target switching between the Player and its Time Echo mirror clones.
    /// </summary>
    private Transform lastTarget;
    private float lastTimeWasInBattle;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Enemy enters Battle State!");

        UpdateBattlePursuitTimer();

        if (player == null)
        {
            // Attempt to acquire Player reference from LOS raycast, or from TakeDamage.Transform
            // collision component reference.
            // Alternative special C# "if null, then assign" coalescing assignment syntax:
            // player ??= enemy.GetPlayerReference();
            // (Discouraged by Unity Linter)
            player = enemy.GetPlayerReference();
        }

        if (ShouldBackstep())
        {
            rb.linearVelocity = new Vector2(enemy.retreatVelocity.x * -DirectionToPlayer(), enemy.retreatVelocity.y);
            enemy.HandleFlip(DirectionToPlayer());
        }
    }

    public override void Update()
    {
        base.Update();

        // If the player is unreachable, we want the pursuit timer to eventually time-out,
        // otherwise the player will just be standing in an "aggressively idle" state, on
        // the ledge. We want the enemy to eventually go back into its Patrol state.
        if (enemy.PlayerDetected() && enemy.BelowLedgeDetected)
        {
            UpdateTargetIfNeeded();
            UpdateBattlePursuitTimer();
        }

        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(enemy.IdleState);
        }

        if (WithinAttackRange() && enemy.PlayerDetected())
        {
            // Note: This is tracking the Start of an Attack (rather than when the Attack
            // was finished), so the attack delay value should be longer than the full
            // attack animation.
            // The reason we perform this nested condition check, is to prevent the
            // enemy from hovering on the player and glitching left-and-right nonstop
            // while in a "pursuit" state.
            if (CanPerformAttack())
            {
                stateMachine.ChangeState(enemy.AttackState);
            }
        }
        else
        {
            if (enemy.BelowLedgeDetected
                // Prevent enemy from continuously running into a wall.
                && !enemy.WallDetected)
            {
                // Pursue the player (aggro).
                enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);
            }
        }
    }

    /// <summary>
    /// Handle target switching between Player and its Time Echo mirror clones.
    /// </summary>
    private void UpdateTargetIfNeeded()
    {
        if (!enemy.PlayerDetected()) return;

        Transform newTarget = enemy.PlayerDetected().transform;

        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    private bool CanPerformAttack() => Time.time > enemy.AttackState.LastTimeAttackPerformed + enemy.GetAttackDelay();

    private void UpdateBattlePursuitTimer() => lastTimeWasInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;

    // Summary:
    //     Checks if the player is within attack range of this enemy.
    private bool WithinAttackRange() => DistanceToPlayer() <= enemy.attackDistance;

    // Summary:
    //     Checks whether the enemy should perform a retreat backstep, to prevent
    //     the player from exploiting distance checks and standing within the enemy,
    //     triggering a constant attacking state.
    private bool ShouldBackstep() => DistanceToPlayer() < enemy.minRetreatDistance;

    // Summary:
    //     Calculates the distance between player and enemy, as a positive value.
    private float DistanceToPlayer()
    {
        if (player == null)
        {
            // Player not found - player is a million miles away.
            return float.MaxValue;
        }

        return Mathf.Abs(player.position.x - enemy.transform.position.x);
    }

    // Summary:
    //     Determine appropriate Facing Direction of the enemy, to Face the Player.
    //     1 = Right, -1 = Left.
    private int DirectionToPlayer()
    {
        if (player == null) return 0;

        // attackDir = player.MoveInput.x != 0 ? (int)player.MoveInput.x : player.FacingDir;
        return player.position.x > enemy.transform.position.x ? 1 : -1;
    }
}
