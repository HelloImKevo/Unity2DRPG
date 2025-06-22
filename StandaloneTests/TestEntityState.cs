/// <summary>
/// Minimal EntityState base class for testing purposes, removing Unity dependencies.
/// This allows us to test StateMachine behavior without requiring Unity Engine.
/// </summary>
public abstract class EntityState
{
    /// <summary>
    /// Called when entering this state
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// Called every frame while in this state
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Called when exiting this state
    /// </summary>
    public abstract void Exit();
}

/// <summary>
/// Test implementation of EntityState for testing without Unity dependencies.
/// This allows us to test StateMachine behavior by providing a minimal EntityState implementation.
/// Tracks method calls for verification in unit tests.
/// </summary>
public class TestEntityState : EntityState
{
    public bool EnterCalled { get; private set; }
    public bool UpdateCalled { get; private set; }
    public bool ExitCalled { get; private set; }
    public int UpdateCallCount { get; private set; }

    public override void Enter()
    {
        EnterCalled = true;
    }

    public override void Update()
    {
        UpdateCalled = true;
        UpdateCallCount++;
    }

    public override void Exit()
    {
        ExitCalled = true;
    }
}
