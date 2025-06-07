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
    //     Triggered by the Player.Animator 'playerBasicAttack' animation, on the last frame.
    public void OnAnimationEnded()
    {
        player.CallAnimationTrigger();
    }
}
