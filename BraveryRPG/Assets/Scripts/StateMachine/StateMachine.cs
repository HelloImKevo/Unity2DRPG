/// <summary>
/// Provides the structure for a Finite State Machine (FSM) implementation.
/// 
/// A Finite State Machine is a computational model used to manage an entity's
/// behavior by organizing it into distinct states. Each state represents a
/// specific behavior pattern (like idle, moving, attacking), and the FSM
/// controls when to transition between these states based on game conditions.
/// 
/// This implementation is commonly used in game development for:
/// - Character AI behavior management
/// - Player controller state handling
/// - UI state management
/// - Game flow control
/// 
/// For more information about state machines in game development:
/// https://gameprogrammingpatterns.com/state.html
/// https://docs.unity3d.com/Manual/StateMachineBasics.html
/// </summary>
public class StateMachine
{
    /// <summary>
    /// The currently active state in the state machine.
    /// 
    /// This property provides read-only access to the current state, allowing
    /// other systems to query which state is active without being able to
    /// directly modify it. State changes should only occur through the
    /// ChangeState() method to ensure proper Enter/Exit lifecycle management.
    /// </summary>
    public EntityState CurrentState { get; private set; }

    /// <summary>
    /// Flag indicating whether the state machine can process state changes.
    /// 
    /// When set to false, all calls to ChangeState() will be ignored. This is
    /// useful for temporarily freezing the state machine during specific
    /// scenarios like death sequences, cutscenes, or pause states where the
    /// entity should not respond to normal state transition triggers.
    /// </summary>
    public bool canChangeState = true;

    /// <summary>
    /// Initializes the state machine with a starting state.
    /// 
    /// This method should be called during the Start() lifecycle function of a
    /// MonoBehaviour component, after all states have been properly instantiated
    /// and configured. It sets up the initial state and triggers its Enter()
    /// method to begin the state machine's operation.
    /// </summary>
    /// <param name="startState">
    /// The initial state that the entity should begin in. This is typically
    /// an idle or default state that represents the entity's resting behavior.
    /// </param>
    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        CurrentState = startState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Transitions the state machine from the current state to a new state.
    /// 
    /// This method handles the complete state transition process:
    /// 1. Calls Exit() on the current state for cleanup
    /// 2. Updates CurrentState to the new state
    /// 3. Calls Enter() on the new state for initialization
    /// 
    /// The transition is ignored if canChangeState is false, allowing for
    /// controlled state machine freezing when necessary.
    /// </summary>
    /// <param name="newState">
    /// The state to transition to. This should be a properly instantiated
    /// EntityState subclass that has been configured with appropriate
    /// references to the entity and state machine.
    /// </param>
    public void ChangeState(EntityState newState)
    {
        if (!canChangeState) return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>
    /// Updates the currently active state.
    /// 
    /// This method should be called every frame (typically from Update() or
    /// FixedUpdate() in a MonoBehaviour) to allow the current state to process
    /// its frame-by-frame logic. It delegates to the current state's Update()
    /// method, which contains the state-specific behavior and transition logic.
    /// </summary>
    public void UpdateActiveState()
    {
        CurrentState.Update();
    }

    /// <summary>
    /// Disables the state machine's ability to change states.
    /// 
    /// This method permanently sets canChangeState to false, effectively
    /// freezing the state machine in its current state. This is typically
    /// used in scenarios like entity death where no further state transitions
    /// should occur. Once called, the state machine cannot be re-enabled.
    /// </summary>
    public void SwitchOffStateMachine() => canChangeState = false;
}
