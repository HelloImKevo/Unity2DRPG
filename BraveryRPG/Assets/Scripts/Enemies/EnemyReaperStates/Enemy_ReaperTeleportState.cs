using UnityEngine;

/// <summary>
/// We require an "Arena Collider" to specify the teleportation bounds.
/// Then we grab a random X coordinate from within the arena, and then
/// perform a raycast from that position, down, and teleport the boss
/// to the ground by calculating half of the boss' height.
/// </summary>
public class Enemy_ReaperTeleportState : EnemyState
{
    private Enemy_Reaper enemyReaper;
    public Enemy_ReaperTeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering REAPER TELEPORT state");
        }

        enemyReaper.MakeUntargetable(true);
    }

    public override void Update()
    {
        base.Update();

        if (enemyReaper.teleportTrigger)
        {
            enemyReaper.transform.position = enemyReaper.FindTeleportPoint();
            enemyReaper.SetTeleportTrigger(false);
        }

        if (onAnimationEndTriggered)
        {
            if (enemyReaper.CanDoSpellCast())
            {
                stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
            }
            else
            {
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemyReaper.MakeUntargetable(false);
    }
}
