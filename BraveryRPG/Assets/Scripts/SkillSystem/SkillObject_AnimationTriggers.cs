using UnityEngine;

public class SkillObject_AnimationTriggers : MonoBehaviour
{
    private SkillObject_TimeEcho timeEcho;

    private void Awake()
    {
        timeEcho = GetComponentInParent<SkillObject_TimeEcho>();
    }

    // Summary:
    //     Triggered at the peak of an Attack Animation, when the Attack would "Make Contact"
    //     if there was a destructible object or enemy within collision range.
    void AttackTrigger()
    {
        timeEcho.PerformAttack();
    }

    // Summary:
    //     Triggered by the TimeEcho.Animator on the last frame of each attack animation.
    //     Once this is triggered 3 times (for a 3-hit combo), the mirror clone is destroyed.
    void TryTerminate(int currentAttackIndex)
    {
        if (currentAttackIndex == timeEcho.maxAttacks)
        {
            timeEcho.HandleDeath();
        }
    }
}
