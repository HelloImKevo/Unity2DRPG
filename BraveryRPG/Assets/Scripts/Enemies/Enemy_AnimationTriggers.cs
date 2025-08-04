using UnityEngine;

public class Enemy_AnimationTriggers : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemyVfx;

    protected override void Awake()
    {
        base.Awake();

        enemy = GetComponentInParent<Enemy>();
        enemyVfx = GetComponentInParent<Enemy_VFX>();
    }

    // Summary:
    //     Trigger a special attack, like the Elf Archer Arrow.
    public void SpecialAttackTrigger()
    {
        enemy.SpecialAttack();
    }

    // Summary:
    //     The enemy can be counterattacked within this window.
    public void EnableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(true);
        enemy.EnableCounterWindow(true);
    }

    // Summary:
    //     The enemy can no longer be counterattacked.
    public void DisableCounterWindow()
    {
        enemyVfx.EnableAttackAlert(false);
        enemy.EnableCounterWindow(false);
    }
}
