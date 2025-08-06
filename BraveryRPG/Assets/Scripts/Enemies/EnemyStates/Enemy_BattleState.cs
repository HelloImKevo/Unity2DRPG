using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;

    private float verticalTolerance = 1.5f;

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

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering BATTLE state");
        }

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

        if (ShouldRetreat())
        {
            ShortRetreat();
        }
    }

    public override void Update()
    {
        base.Update();

        // If the player is unreachable, we want the pursuit timer to eventually time-out,
        // otherwise the player will just be standing in an "aggressively idle" state, on
        // the ledge. We want the enemy to eventually go back into its Patrol state.
        if (enemy.PlayerDetected() && enemy.CanAggressivelyPursuePlayer())
        {
            UpdateTargetIfNeeded();
            UpdateBattlePursuitTimer();
        }

        if (BattleTimeIsOver() || IsPlayerBeyondReach())
        {
            stateMachine.ChangeState(enemy.IdleState);
            return;
        }

        if (ShouldAttackIfPossible())
        {
            if (!ShouldWalkAwayFromPlayer())
            {
                FlipToFacePlayerIfNeeded();
            }
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
            // Prevent enemy from continuously running into a wall.
            if (enemy.CanAggressivelyPursuePlayer())
            {
                if (ShouldWalkAwayFromPlayer())
                {
                    // Debug.Log("Elf should walk away from player ...");
                    // Strafe / retreat backwards a short distance.
                    enemy.SetVelocity(enemy.GetBattleMoveSpeed() * -1 * DirectionToPlayer(), rb.linearVelocity.y);
                }
                else
                {
                    FlipToFacePlayerIfNeeded();
                    // Pursue the player (aggro).
                    enemy.SetVelocity(enemy.GetBattleMoveSpeed() * DirectionToPlayer(), rb.linearVelocity.y);
                }
            }
        }
    }

    protected void ShortRetreat()
    {
        float x = (enemy.retreatVelocity.x * enemy.activeSlowMultiplier) * -DirectionToPlayer();
        float y = enemy.retreatVelocity.y;

        rb.linearVelocity = new Vector2(x, y);
        enemy.HandleFlip(DirectionToPlayer());
    }

    protected virtual bool ShouldAttackIfPossible()
    {
        return WithinAttackRange() && enemy.PlayerDetected();
    }

    protected virtual bool ShouldWalkAwayFromPlayer() => false;

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

    private void FlipToFacePlayerIfNeeded()
    {
        if (DirectionToPlayer() != enemy.FacingDir)
        {
            enemy.HandleFlip(DirectionToPlayer());
        }
    }

    protected bool CanPerformAttack() => Time.time > enemy.AttackState.LastTimeAttackPerformed + enemy.GetAttackDelay();

    private void UpdateBattlePursuitTimer() => lastTimeWasInBattle = Time.time;

    private bool BattleTimeIsOver() => Time.time > lastTimeWasInBattle + enemy.battleTimeDuration;

    private bool IsPlayerBeyondReach() => DistanceToPlayer() > enemy.attackDistance * 5f;

    // Summary:
    //     Checks if the player is within attack range of this enemy.
    protected bool WithinAttackRange() => DistanceToPlayer() <= enemy.attackDistance;

    // Summary:
    //     Checks whether the enemy should perform a retreat backstep, to prevent
    //     the player from exploiting distance checks and standing within the enemy,
    //     triggering a constant attacking state.
    protected bool ShouldRetreat()
    {
        return DistanceToPlayer() < enemy.minRetreatDistance
            && enemy.CanAggressivelyPursuePlayer();
    }

    // Summary:
    //     Calculates the distance between player and enemy, as a positive value.
    protected float DistanceToPlayer()
    {
        if (player == null)
        {
            // Player not found - player is a million miles away.
            return float.MaxValue;
        }

        // return Mathf.Abs(player.position.x - enemy.transform.position.x);

        Vector2 playerPos = player.position;
        Vector2 enemyPos = enemy.transform.position;

        float verticalDiff = Mathf.Abs(playerPos.y - enemyPos.y);

        // Player is too high or too low — treat as "out of range"
        if (verticalDiff > verticalTolerance)
        {
            return float.MaxValue;
        }

        float horizontalDistance = Mathf.Abs(playerPos.x - enemyPos.x);
        return horizontalDistance;
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
