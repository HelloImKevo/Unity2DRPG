using UnityEngine;

// TODO: This class can be deleted - no longer used.
//  Use Entity_AnimationTriggers instead.
public class PlayerAnimationEvents : MonoBehaviour
{

    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    // public void DamageEnemies() => player.DamageEnemies();

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerAttack' animation, on the first frame.
    // private void DisableMovementAndJump() => player.EnableMovementAndJump(false);

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerAttack' animation, on the last frame.
    // private void EnableMovementAndJump() => player.EnableMovementAndJump(true);
}
