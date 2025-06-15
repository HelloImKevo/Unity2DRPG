using UnityEngine;

/// <summary>
/// Abstract base class for all player-specific states in the state machine.
/// 
/// This class extends EntityState to provide player-specific functionality,
/// including input handling, player component access, and common player
/// behaviors that apply across multiple states. It serves as the foundation
/// for all player states like movement, combat, and aerial states.
/// 
/// Key features:
/// - Direct access to player input system
/// - Automatic dash interrupt capability across all states
/// - Player-specific animation parameter updates
/// - Centralized player component references
/// 
/// All concrete player states should inherit from this class rather than
/// directly from EntityState to ensure consistent player behavior and
/// input handling across the state machine.
/// </summary>
public abstract class PlayerState : EntityState
{
    /// <summary>
    /// Reference to the Player entity that owns this state.
    /// 
    /// This provides direct access to all player-specific properties and
    /// methods, including movement parameters, state references, health,
    /// and player-specific behaviors. States use this reference to:
    /// - Access movement speeds and physics parameters
    /// - Trigger player-specific actions
    /// - Check player conditions and status
    /// - Initiate state transitions to other player states
    /// </summary>
    protected Player player;

    /// <summary>
    /// Reference to the player's input system for handling user controls.
    /// 
    /// This provides access to Unity's new Input System, allowing states to
    /// respond to player input like movement, jumping, attacking, and dashing.
    /// The input system handles multiple input devices (keyboard, gamepad)
    /// and provides convenient methods for checking input state like:
    /// - WasPressedThisFrame() for single-press actions
    /// - IsPressed() for continuous actions
    /// - ReadValue() for analog inputs like movement vectors
    /// 
    /// For more information about Unity's Input System:
    /// https://docs.unity3d.com/Packages/com.unity.inputsystem@1.0/manual/index.html
    /// </summary>
    protected PlayerInputSet input;

    /// <summary>
    /// Constructs a new PlayerState with required player-specific dependencies.
    /// 
    /// This constructor initializes all the component references needed for
    /// player state functionality, including the player entity, input system,
    /// animator, and rigidbody components. It ensures that all player states
    /// have consistent access to these essential systems.
    /// </summary>
    /// <param name="player">
    /// The Player entity that this state will control and manage.
    /// </param>
    /// <param name="stateMachine">
    /// The state machine that will manage this state's lifecycle.
    /// </param>
    /// <param name="animBoolName">
    /// The name of the boolean parameter in the Animator Controller for this state.
    /// </param>
    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.Anim;
        rb = player.Rb;
        input = player.Input;
    }

    /// <summary>
    /// Called when the player transitions into this state.
    /// 
    /// Inherits base functionality from EntityState.Enter() and can be
    /// overridden by subclasses to add player-specific initialization logic.
    /// </summary>
    public override void Enter()
    {
        base.Enter();
    }

    /// <summary>
    /// Called every frame while this player state is active.
    /// 
    /// This method includes core player functionality that applies to most
    /// states, such as the ability to interrupt actions with a dash. It
    /// processes common player input and maintains universal player behaviors
    /// while allowing subclasses to add state-specific logic.
    /// 
    /// The dash interrupt system allows players to cancel most actions by
    /// dashing, providing responsive controls and escape options in combat.
    /// </summary>
    public override void Update()
    {
        base.Update();

        // Run logic of the state here.

        // Enable the user to interrupt an Attack animation by Dashing.
        if (input.Player.Dash.WasPressedThisFrame() && CanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
    }

    /// <summary>
    /// Updates animation parameters specific to player movement and state.
    /// 
    /// This method sets animation parameters that are commonly used across
    /// multiple player states. Currently updates the vertical velocity for
    /// jump/fall animation blending, but can be extended with additional
    /// parameters as needed.
    /// 
    /// The yVelocity parameter is typically used in Animator blend trees
    /// to smoothly transition between jump and fall animations based on
    /// the player's vertical movement direction.
    /// </summary>
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    /// <summary>
    /// Called when the player transitions out of this state.
    /// 
    /// Inherits base functionality from EntityState.Exit() and can be
    /// overridden by subclasses to add player-specific cleanup logic.
    /// </summary>
    public override void Exit()
    {
        base.Exit();
    }

    /// <summary>
    /// Determines whether the player can currently perform a dash action.
    /// 
    /// This method checks various conditions that would prevent dashing:
    /// - Wall collision (prevents dashing into walls)
    /// - Already in dash state (prevents dash spam/chaining)
    /// 
    /// Additional conditions can be added here to further refine dash
    /// availability, such as stamina systems, cooldowns, or environmental
    /// restrictions.
    /// </summary>
    /// <returns>
    /// True if the player can dash from the current state, false otherwise.
    /// </returns>
    private bool CanDash()
    {
        if (player.WallDetected || stateMachine.CurrentState == player.DashState)
        {
            return false;
        }
        return true;
    }
}
