using UnityEngine;

// Summary:
//     This script should be attached to an [Animator] component.
public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;
    private Entity_Combat entityCombat;

    protected virtual void Awake()
    {
        entity = GetComponentInParent<Entity>();
        entityCombat = GetComponentInParent<Entity_Combat>();
    }

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerBasicAttack_1' animation, close
    //     to the end of the animation sequence, before the player starts to sheath
    //     their sword, indicating the attack combo sequence can be continued.
    public void OnNextActionInputReady()
    {
        entity.CallOnNextActionInputReadyTrigger();
    }

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerBasicAttack' animation, on the last frame.
    public void OnAnimationEnded()
    {
        entity.CallOnAnimationEndedTrigger();
    }

    // Summary:
    //     Triggered at the peak of an Attack Animation, when the Attack would "Make Contact"
    //     if there was a destructible object or enemy within collision range.
    public void OnAttackPeak()
    {
        entityCombat.PerformAttack();
    }
}
