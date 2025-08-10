using UnityEngine;

public class Enemy_ReaperAnimationTriggers : Enemy_AnimationTriggers
{
    private Enemy_Reaper enemyReaper;

    protected override void Awake()
    {
        base.Awake();

        enemyReaper = GetComponentInParent<Enemy_Reaper>();
    }

    // Summary: Trigger enemy teleportation.
    public void TeleportTrigger()
    {
        enemyReaper.SetTeleportTrigger(true);
    }
}
