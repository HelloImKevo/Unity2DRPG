using UnityEngine;

public class EntityState
{
    protected StateMachine stateMachine;
    protected string stateName;

    public EntityState(StateMachine stateMachine, string stateName)
    {
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
