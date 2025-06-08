using UnityEngine;

public class Player_AnimationTriggers : MonoBehaviour
{
    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerBasicAttack_1' animation, close
    //     to the end of the animation sequence, before the player starts to sheath
    //     their sword, indicating the attack combo sequence can be continued.
    public void OnNextActionInputReady()
    {
        player.CallOnNextActionInputReadyTrigger();
    }

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerBasicAttack' animation, on the last frame.
    public void OnAnimationEnded()
    {
        player.CallOnAnimationEndedTrigger();
    }
}
