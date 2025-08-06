using UnityEngine;

public class Enemy_MageBattleState : Enemy_BattleState
{
    private Enemy_Mage enemyMage;
    private float lastTimeUsedRetreat = float.NegativeInfinity;

    public Enemy_MageBattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering MAGE BATTLE state");
        }

        if (ShouldRetreat())
        {
            if (CanUseEchoRetreatAbility())
            {
                EchoRetreatAbility();
            }
            else
            {
                ShortRetreat();
            }
        }
    }

    private void EchoRetreatAbility()
    {
        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Changing to MAGE ECHO RETREAT state ...");
        }

        lastTimeUsedRetreat = Time.time;
        stateMachine.ChangeState(enemyMage.mageRetreatState);
    }

    private bool CanUseEchoRetreatAbility() => Time.time > lastTimeUsedRetreat + enemyMage.retreatCooldown;
}
