using UnityEngine;

/// <summary>
/// An Entity possessing an FSM (Finite State Machine).
/// </summary>
public abstract class EntityState
{
    protected Player player;
    protected StateMachine stateMachine;
    protected string stateName;

    public EntityState(Player player, StateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        // Every time state will be changed, enter will be called.
        Debug.Log("I enter " + stateName);
    }

    public virtual void Update()
    {
        // Run logic of the state here.
        Debug.Log("I run update of " + stateName);
    }

    public virtual void Exit()
    {
        // This will be called every time we exit state and change to a new one.
        Debug.Log("I exit " + stateName);
    }
}
