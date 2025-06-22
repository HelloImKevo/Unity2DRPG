using Moq;
using NUnit.Framework;

namespace StandaloneTests
{
    /// <summary>
    /// Comprehensive unit tests for StateMachine class using Moq framework for mocking.
    /// Demonstrates testing state transitions, lifecycle management, and edge cases.
    /// </summary>
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine _stateMachine;
        private TestEntityState _initialState;
        private TestEntityState _secondState;

        [SetUp]
        public void SetUp()
        {
            _stateMachine = new StateMachine();
            _initialState = new TestEntityState();
            _secondState = new TestEntityState();
        }

        [Test]
        public void Initialize_WithValidState_SetsCurrentStateAndCallsEnter()
        {
            // Act
            _stateMachine.Initialize(_initialState);

            // Assert
            Assert.That(_stateMachine.CurrentState, Is.EqualTo(_initialState));
            Assert.That(_stateMachine.canChangeState, Is.True);
            Assert.That(_initialState.EnterCalled, Is.True, "Enter() should be called during initialization");
        }

        [Test]
        public void ChangeState_WhenCanChangeStateIsTrue_TransitionsToNewState()
        {
            // Arrange
            _stateMachine.Initialize(_initialState);

            // Act
            _stateMachine.ChangeState(_secondState);

            // Assert
            Assert.That(_stateMachine.CurrentState, Is.EqualTo(_secondState));
            Assert.That(_initialState.ExitCalled, Is.True, "Exit() should be called on previous state");
            Assert.That(_secondState.EnterCalled, Is.True, "Enter() should be called on new state");
        }

        [Test]
        public void ChangeState_WhenCanChangeStateIsFalse_IgnoresStateChange()
        {
            // Arrange
            _stateMachine.Initialize(_initialState);
            _stateMachine.canChangeState = false;

            // Act
            _stateMachine.ChangeState(_secondState);

            // Assert
            Assert.That(_stateMachine.CurrentState, Is.EqualTo(_initialState), "Current state should not change");
            Assert.That(_initialState.ExitCalled, Is.False, "Exit() should not be called when state changes are disabled");
            Assert.That(_secondState.EnterCalled, Is.False, "Enter() should not be called when state changes are disabled");
        }

        [Test]
        public void UpdateActiveState_CallsUpdateOnCurrentState()
        {
            // Arrange
            _stateMachine.Initialize(_initialState);

            // Act
            _stateMachine.UpdateActiveState();

            // Assert
            Assert.That(_initialState.UpdateCalled, Is.True, "Update() should be called on current state");
        }

        [Test]
        public void SwitchOffStateMachine_DisablesStateChanges()
        {
            // Arrange
            _stateMachine.Initialize(_initialState);

            // Act
            _stateMachine.SwitchOffStateMachine();
            _stateMachine.ChangeState(_secondState);

            // Assert
            Assert.That(_stateMachine.canChangeState, Is.False);
            Assert.That(_stateMachine.CurrentState, Is.EqualTo(_initialState), "State should not change after switching off");
        }

        [Test]
        public void MultipleStateTransitions_MaintainsProperLifecycle()
        {
            // Arrange
            var state3 = new TestEntityState();
            _stateMachine.Initialize(_initialState);

            // Act
            _stateMachine.ChangeState(_secondState);
            _stateMachine.ChangeState(state3);

            // Assert
            Assert.That(_stateMachine.CurrentState, Is.EqualTo(state3));
            Assert.That(_initialState.ExitCalled, Is.True, "State1 Exit() should be called");
            Assert.That(_secondState.EnterCalled, Is.True, "State2 Enter() should be called");
            Assert.That(_secondState.ExitCalled, Is.True, "State2 Exit() should be called");
            Assert.That(state3.EnterCalled, Is.True, "State3 Enter() should be called");
        }

        [Test]
        public void UpdateActiveState_MultipleFrames_CallsUpdateEachTime()
        {
            // Arrange
            _stateMachine.Initialize(_initialState);

            // Act
            _stateMachine.UpdateActiveState();
            _stateMachine.UpdateActiveState();
            _stateMachine.UpdateActiveState();

            // Assert
            Assert.That(_initialState.UpdateCallCount, Is.EqualTo(3), "Update() should be called for each frame");
        }

        [Test]
        public void StateMachine_WithMockFramework_DemonstratesMocking()
        {
            // Arrange - Using Moq to create a mock state
            var mockState = new Mock<IEntityState>();
            var stateMachineForMocking = new StateMachineWithInterface();

            // Act
            stateMachineForMocking.Initialize(mockState.Object);
            stateMachineForMocking.UpdateActiveState();
            stateMachineForMocking.ChangeState(mockState.Object);

            // Assert - Verify mock interactions
            mockState.Verify(x => x.Enter(), Times.Exactly(2), "Enter should be called twice");
            mockState.Verify(x => x.Update(), Times.Once, "Update should be called once");
            mockState.Verify(x => x.Exit(), Times.Once, "Exit should be called once");
        }
    }

    /// <summary>
    /// Interface for Entity State behavior - enables mocking with Moq
    /// </summary>
    public interface IEntityState
    {
        void Enter();
        void Update();
        void Exit();
    }

    /// <summary>
    /// Version of StateMachine that works with interfaces for better mocking support
    /// </summary>
    public class StateMachineWithInterface
    {
        public IEntityState CurrentState { get; private set; }
        public bool canChangeState = true;

        public void Initialize(IEntityState startState)
        {
            canChangeState = true;
            CurrentState = startState;
            CurrentState.Enter();
        }

        public void ChangeState(IEntityState newState)
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
}
