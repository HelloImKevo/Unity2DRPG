using UnityEngine;

public class Enemy_BattleState : EnemyState
{
    private Transform player;

    public Enemy_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        Debug.Log("Enemy enters Battle State!");

        if (player == null)
        {
            player = enemy.PlayerDetection().transform;
        }
    }

    public override void Update()
    {
        base.Update();

        if (WithinAttackRange())
        {
            stateMachine.ChangeState(enemy.AttackState);
        }
        else
        {
            enemy.SetVelocity(enemy.battleMoveSpeed * DirectionToPlayer(), rb.linearVelocity.y);
        }
    }

    // Summary:
    //     Checks if the player is within attack range of this enemy.
    private bool WithinAttackRange() => DistanceToPlayer() <= enemy.attackDistance;

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
