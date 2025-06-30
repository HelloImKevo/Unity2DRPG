using UnityEngine;

public class Player_ThrowSwordState : PlayerState
{
    private Camera mainCamera;

    public Player_ThrowSwordState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        skillManager.ThrowSword.EnableDots(true);

        if (mainCamera != Camera.main)
        {
            mainCamera = Camera.main;
        }
    }

    public override void Update()
    {
        base.Update();

        Vector2 dirToMouse = DirectionToMouse();

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(dirToMouse.x);
        skillManager.ThrowSword.PredictTrajectory(dirToMouse);


        if (input.Player.Attack.WasPressedThisFrame())
        {
            anim.SetBool("throwSwordPerformed", true);

            skillManager.ThrowSword.EnableDots(false);
            skillManager.ThrowSword.ConfirmTrajectory(dirToMouse);

            // skill manager create sword
        }

        // If the player releases the "Range Attack" button, return to the Idle State.
        if (input.Player.RangeAttack.WasReleasedThisFrame() || onAnimationEndTriggered)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("throwSwordPerformed", false);
        skillManager.ThrowSword.EnableDots(false);
    }

    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.MousePosition);

        // Direction from player to the mouse.
        Vector2 directionToMouse = worldMousePosition - playerPosition;

        return directionToMouse.normalized;
    }
}
