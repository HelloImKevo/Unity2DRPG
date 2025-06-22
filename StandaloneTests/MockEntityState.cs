namespace StandaloneTests.Mocks
{
    /// <summary>
    /// Mock-friendly version of EntityState for testing StateMachine without Unity dependencies.
    /// This removes the Unity-specific components while preserving the core state lifecycle.
    /// </summary>
    public abstract class MockEntityState
    {
        protected StateMachine stateMachine;
        protected string animBoolName;
        protected float stateTimer;

        public MockEntityState(StateMachine stateMachine, string animBoolName)
        {
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        /// <summary>
        /// Called when entering this state. Override to implement state entry logic.
        /// </summary>
        public virtual void Enter()
        {
            stateTimer = 0f;
        }

        /// <summary>
        /// Called every frame while in this state. Override to implement state behavior.
        /// </summary>
        public virtual void Update()
        {
            stateTimer += 0.016f; // Simulate ~60 FPS
        }

        /// <summary>
        /// Called when exiting this state. Override to implement cleanup logic.
        /// </summary>
        public virtual void Exit()
        {
            // Base implementation - can be overridden
        }

        /// <summary>
        /// Animation trigger method for testing
        /// </summary>
        public virtual void CallOnNextActionInputReadyTrigger()
        {
            // Base implementation - can be overridden
        }

        /// <summary>
        /// Animation trigger method for testing
        /// </summary>
        public virtual void CallOnAnimationEndedTrigger()
        {
            // Base implementation - can be overridden
        }

        // Public accessor for testing
        public float StateTimer => stateTimer;
        public string AnimBoolName => animBoolName;
    }
}
