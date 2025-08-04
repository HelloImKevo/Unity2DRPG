/// <summary>
/// Abstract base class for all enemy-specific states in the state machine.
/// 
/// This class extends EntityState to provide enemy-specific functionality,
/// including enemy component access and enemy-specific animation parameter
/// updates. It serves as the foundation for all enemy states like patrolling,
/// combat, and idle behaviors.
/// 
/// Key features:
/// - Direct access to enemy entity and its properties
/// - Enemy-specific animation parameter management
/// - Battle animation speed synchronization
/// - Foundation for enemy AI behavior states
/// 
/// All concrete enemy states should inherit from this class rather than
/// directly from EntityState to ensure consistent enemy behavior and
/// animation handling across the state machine.
/// </summary>
public class EnemyState : EntityState
{
    /// <summary>
    /// Reference to the Enemy entity that owns this state.
    /// 
    /// This provides direct access to all enemy-specific properties and
    /// methods, including AI parameters, detection systems, combat stats,
    /// and enemy-specific behaviors. States use this reference to:
    /// - Access movement speeds and patrol parameters
    /// - Check player detection and line-of-sight
    /// - Manage combat distances and aggro systems
    /// - Control enemy-specific animations and effects
    /// </summary>
    protected Enemy enemy;

    // When was the last attack performed (using Time.time).
    public float LastTimeAttackPerformed { get; protected set; } = float.NegativeInfinity;

    /// <summary>
    /// Constructs a new EnemyState with required enemy-specific dependencies.
    /// 
    /// This constructor initializes all the component references needed for
    /// enemy state functionality, including the enemy entity, animator, and
    /// rigidbody components. It ensures that all enemy states have consistent
    /// access to these essential systems.
    /// </summary>
    /// <param name="enemy">
    /// The Enemy entity that this state will control and manage.
    /// </param>
    /// <param name="stateMachine">
    /// The state machine that will manage this state's lifecycle.
    /// </param>
    /// <param name="animBoolName">
    /// The name of the boolean parameter in the Animator Controller for this state.
    /// </param>
    public EnemyState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.enemy = enemy;

        anim = enemy.Anim;
        rb = enemy.Rb;
        stats = enemy.Stats;
    }

    /// <summary>
    /// Called every frame while this enemy state is active.
    /// 
    /// Inherits base functionality from EntityState.Update() and can be
    /// overridden by subclasses to add enemy-specific frame logic while
    /// maintaining the core state timing and animation update systems.
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Updates animation parameters specific to enemy behavior and combat.
    /// 
    /// This method manages enemy-specific animation parameters that are used
    /// across multiple enemy states:
    /// 
    /// - moveAnimSpeedMultiplier: Controls the speed of walking/movement animations
    /// - battleAnimSpeedMultiplier: Increases animation speed during combat to
    ///   match the faster movement speed, preventing the "sliding" effect where
    ///   the enemy moves faster than their walking animation plays
    /// - xVelocity: Provides velocity information for animation blend trees
    /// 
    /// The battle animation speed system ensures that when enemies enter an
    /// aggressive state with increased movement speed, their animations speed
    /// up proportionally to maintain visual consistency.
    /// </summary>
    public override void UpdateAnimationParameters()
    {
        base.UpdateAnimationParameters();

        // When the enemy enters its Battle state (aggro), increase its move speed
        // to match the aggressive animation (so it looks like the enemy is walking
        // faster, rather than sliding across the floor).
        float battleAnimSpeedMultiplier = enemy.GetBattleMoveSpeed() / enemy.GetMoveSpeed();

        anim.SetFloat("moveAnimSpeedMultiplier", enemy.moveAnimSpeedMultiplier);
        anim.SetFloat("battleAnimSpeedMultiplier", battleAnimSpeedMultiplier);
        anim.SetFloat("xVelocity", rb.linearVelocity.x);
    }
}
