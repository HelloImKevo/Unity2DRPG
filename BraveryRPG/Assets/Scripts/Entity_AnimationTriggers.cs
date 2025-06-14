using UnityEngine;

// Summary:
//     This script should be attached to an [Animator] component.
public class Entity_AnimationTriggers : MonoBehaviour
{
    private Entity entity;

    private void Awake()
    {
        entity = GetComponentInParent<Entity>();
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
}
