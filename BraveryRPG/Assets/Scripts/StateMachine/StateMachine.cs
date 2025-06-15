/// <summary>
/// Provides the structure for our FSM (Finite State Machine).
/// </summary>
public class StateMachine
{
    public EntityState CurrentState { get; private set; }
    public bool canChangeState = true;

    // Summary:
    //     Should be called in the Start() lifecycle function of a MonoBehaviour component.
    public void Initialize(EntityState startState)
    {
        canChangeState = true;
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(EntityState newState)
    {
        if (!canChangeState) return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void UpdateActiveState()
    {
        CurrentState.Update();
    }

    public void SwitchOffStateMachine() => canChangeState = false;
}
