using UnityEngine;

public class Enemy_MageSpellCastState : EnemyState
{
    private Enemy_Mage enemyMage;

    public Enemy_MageSpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyMage = enemy as Enemy_Mage;
    }

    public override void Enter()
    {
        base.Enter();

        if (enemy.shouldLogStateTransitions)
        {
            Debug.Log($"{enemy.gameObject.name} Entering MAGE SPELL CAST state");
        }

        enemyMage.SetVelocity(0, 0);
        enemyMage.SetSpellCastPerformed(false);
    }

    public override void Update()
    {
        base.Update();

        if (enemyMage.spellCastPerformed)
        {
            // anim.SetBool("spellCast_performed", true);
        }

        if (onAnimationEndTriggered)
        {
            stateMachine.ChangeState(enemy.BattleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        // anim.SetBool("spellCast_performed", false);
    }
}
