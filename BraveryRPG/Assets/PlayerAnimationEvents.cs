using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{

    private Player player;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
    }

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerAttack' animation, on the first frame.
    private void DisableMovementAndJump() => player.EnableMovementAndJump(false);

    //
    // Summary:
    //     Triggered by the Player.Animator 'playerAttack' animation, on the last frame.
    private void EnableMovementAndJump() => player.EnableMovementAndJump(true);
}
