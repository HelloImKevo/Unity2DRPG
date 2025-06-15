using UnityEngine;

/// <summary>
/// Abstract base class for all entity states in the Finite State Machine pattern.
/// 
/// This class defines the core structure and lifecycle methods that all concrete
/// state implementations must follow. It provides the foundation for managing
/// entity behavior through discrete states, each representing a specific behavior
/// pattern (like idle, moving, attacking, jumping, etc.).
/// 
/// The state pattern allows for clean separation of different behaviors, making
/// code more maintainable and enabling complex behavior trees through state
/// transitions. Each state encapsulates its own logic while maintaining access
/// to shared entity components and animation systems.
/// 
/// Key responsibilities:
/// - Managing state lifecycle (Enter, Update, Exit)
/// - Handling animation parameter updates
/// - Providing timer functionality for timed states
/// - Managing animation event triggers
/// 
/// For more information about the State pattern in game development:
/// https://gameprogrammingpatterns.com/state.html
/// https://docs.unity3d.com/Manual/StateMachineBasics.html
/// </summary>
public abstract class EntityState
{
    /// <summary>
    /// Reference to the state machine that manages this state.
    /// 
    /// This allows the state to trigger transitions to other states by calling
    /// stateMachine.ChangeState(). States should use this reference to initiate
    /// state changes based on input, game conditions, or animation events.
    /// </summary>
    protected StateMachine stateMachine;

    /// <summary>
    /// Name of the boolean parameter in the Animator controller for this state.
    /// 
    /// Unity's Animator system uses boolean parameters to control animation
    /// transitions. This string corresponds to a parameter name in the Animator
    /// Controller that will be set to true when entering this state and false
    /// when exiting. This ensures proper synchronization between game logic
    /// states and visual animation states.
    /// </summary>
    protected string animBoolName;

    /// <summary>
    /// Reference to the entity's Animator component for controlling animations.
    /// 
    /// This component is used to trigger animation transitions, set animation
    /// parameters, and synchronize visual feedback with game state changes.
    /// Subclasses can use this to play specific animations or update animation
    /// parameters based on entity behavior.
    /// </summary>
    protected Animator anim;

    /// <summary>
    /// Reference to the entity's Rigidbody2D component for physics interactions.
    /// 
    /// This component provides access to the entity's physics properties like
    /// velocity, position, and collision detection. States use this to control
    /// movement, apply forces, check physics conditions, and respond to
    /// collision events.
    /// </summary>
    protected Rigidbody2D rb;

    /// <summary>
    /// Timer for states that need to track elapsed time or duration limits.
    /// 
    /// This countdown timer automatically decreases each frame and is clamped
    /// to zero. States can use this for:
    /// - Timed state durations (e.g., dash duration, attack recovery)
    /// - Cooldown periods
    /// - Animation synchronization
    /// - Delayed state transitions
    /// </summary>
    protected float stateTimer;

    /// <summary>
    /// Flag indicating when the next combo attack can be queued.
    /// 
    /// This trigger is set by animation events during attack sequences to
    /// indicate when the player can input the next attack in a combo chain.
    /// It enables smooth combat flow by allowing input buffering during
    /// specific animation frames rather than requiring precise timing.
    /// </summary>
    protected bool onNextComboAttackReadyTrigger;

    /// <summary>
    /// Flag indicating when an animation sequence has completed.
    /// 
    /// This trigger is set by animation events on the final frame of animations
    /// to signal that the animation has finished playing. States use this to
    /// determine when to transition to new states, ensuring animations complete
    /// properly before state changes occur.
    /// </summary>
    protected bool onAnimationEndedTrigger;

    /// <summary>
    /// Constructs a new EntityState with required dependencies.
    /// 
    /// This constructor establishes the basic connections needed for state
    /// functionality. Subclasses typically call this constructor and then
    /// initialize their specific component references (anim, rb) by getting
    /// them from their associated entity.
    /// </summary>
    /// <param name="stateMachine">
    /// The state machine that will manage this state's lifecycle and transitions.
    /// </param>
    /// <param name="animBoolName">
    /// The name of the boolean parameter in the Animator Controller that
    /// corresponds to this state's animation condition.
    /// </param>
    public EntityState(StateMachine stateMachine, string animBoolName)
    {
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    /// <summary>
    /// Called when the entity transitions into this state.
    /// 
    /// This method performs initialization logic that should occur every time
    /// the state becomes active. It automatically:
    /// - Activates the corresponding animation boolean parameter
    /// - Resets animation event triggers
    /// - Prepares the state for its active phase
    /// 
    /// Subclasses should override this method to add state-specific
    /// initialization logic while calling base.Enter() to maintain core
    /// functionality.
    /// </summary>
    public virtual void Enter()
    {
        // Every time state will be changed, enter will be called.
        anim.SetBool(animBoolName, true);
        onNextComboAttackReadyTrigger = false;
        onAnimationEndedTrigger = false;
    }

    /// <summary>
    /// Called every frame while this state is active.
    /// 
    /// This method contains the core state logic that executes continuously
    /// while the state is active. It automatically:
    /// - Decrements the state timer
    /// - Updates animation parameters
    /// - Maintains frame-rate independent timing
    /// 
    /// Subclasses should override this method to implement state-specific
    /// behavior like input handling, condition checking, and state transition
    /// logic while calling base.Update() to maintain timing functionality.
    /// </summary>
    public virtual void Update()
    {
        stateTimer -= Time.deltaTime;
        stateTimer = Mathf.Max(0, stateTimer);

        UpdateAnimationParameters();
    }

    /// <summary>
    /// Called when the entity transitions out of this state.
    /// 
    /// This method performs cleanup logic that should occur every time the
    /// state becomes inactive. It automatically deactivates the corresponding
    /// animation boolean parameter to ensure proper animation transitions.
    /// 
    /// Subclasses should override this method to add state-specific cleanup
    /// logic while calling base.Exit() to maintain core functionality.
    /// </summary>
    public virtual void Exit()
    {
        // This will be called every time we exit state and change to a new one.
        anim.SetBool(animBoolName, false);
    }

    /// <summary>
    /// Triggers the combo attack ready flag for attack chaining.
    /// 
    /// This method is called by animation events to signal when the next attack
    /// in a combo sequence can be queued. It enables smooth combat flow by
    /// allowing input buffering during specific animation frames, creating
    /// responsive and fluid combat mechanics.
    /// 
    /// The system could be extended to support other interruptible actions
    /// like jumping or dashing during attack animations.
    /// </summary>
    public void CallOnNextActionInputReadyTrigger()
    {
        onNextComboAttackReadyTrigger = true;
    }

    /// <summary>
    /// Triggers the animation ended flag for state transition logic.
    /// 
    /// This method is called by animation events on the final frame of
    /// animations to signal completion. States use this trigger to determine
    /// when animations have finished playing, ensuring proper timing for
    /// state transitions and preventing premature state changes.
    /// </summary>
    public void CallOnAnimationEndedTrigger()
    {
        onAnimationEndedTrigger = true;
    }

    /// <summary>
    /// Updates animation parameters based on current state conditions.
    /// 
    /// This virtual method is called every frame during Update() to synchronize
    /// animation parameters with current game state. The base implementation
    /// is empty, allowing subclasses to override with specific parameter
    /// updates like velocity values, state flags, or condition variables.
    /// 
    /// Common parameters include:
    /// - Movement velocity for blend trees
    /// - Boolean flags for animation conditions
    /// - Float values for animation speeds or multipliers
    /// </summary>
    public virtual void UpdateAnimationParameters()
    {
        // Override in subclasses as needed.
    }
}
