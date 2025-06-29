using UnityEngine;

/// <summary>
/// Handles combat mechanics and damage dealing for game entities.
/// 
/// This component manages the combat system for entities that can deal damage
/// to other objects in the game world. It provides a centralized system for
/// attack detection, damage calculation, and target validation using Unity's
/// 2D physics system.
/// 
/// The component uses circular collision detection to identify targets within
/// attack range and applies damage to any objects that implement the IDamageable
/// interface. This design allows for flexible combat interactions between
/// players, enemies, destructible objects, and environmental elements.
/// 
/// Key features:
/// - Circular area-of-effect attack detection
/// - Configurable damage values per entity
/// - Layer mask filtering for target selection
/// - Visual gizmo debugging for attack ranges
/// - Integration with Unity's 2D physics system
/// 
/// For more information about Unity's 2D physics and collision detection:
/// https://docs.unity3d.com/Manual/Physics2DReference.html
/// https://docs.unity3d.com/ScriptReference/Physics2D.OverlapCircleAll.html
/// </summary>
public class Entity_Combat : MonoBehaviour
{
    private Entity_Stats stats;
    private Entity_VFX vfx;

    [Tooltip("Physical & elemental attack damage scaling and behavior parameters.")]
    public DamageScaleData basicAttackScale;

    [Header("Target Detection")]

    /// <summary>
    /// The central point from which collision detection is performed for attacks.
    /// 
    /// This Transform defines the origin point for the circular attack area.
    /// It's typically positioned:
    /// - At the tip of a weapon for melee attacks
    /// - At the center of the entity for area attacks
    /// - Slightly in front of the entity for forward-facing attacks
    /// 
    /// The position can be animated or moved dynamically to create complex
    /// attack patterns or follow weapon movements during animations.
    /// </summary>
    [Tooltip("Central point from which collision detection will be performed for melee attacks")]
    [SerializeField] private Transform targetCheckPoint;

    /// <summary>
    /// The radius of the circular area used for attack collision detection.
    /// 
    /// This value determines how large the attack's hitbox is around the
    /// targetCheckPoint. Larger values create wider area attacks, while
    /// smaller values require more precise positioning. The radius should
    /// be balanced to feel responsive without being overpowered.
    /// 
    /// Common values:
    /// - 0.5f for precise melee weapons (swords, daggers)
    /// - 1.0f for medium range attacks (clubs, axes)
    /// - 2.0f for large area attacks (hammers, magic spells)
    /// </summary>
    [Tooltip("Circular radius from the Target Check Point to detect collisions when a melee attack is performed")]
    [SerializeField] private float targetCheckRadius = 1f;

    /// <summary>
    /// Layer mask that defines which objects can be targeted by attacks.
    /// 
    /// This mask filters collision detection to only affect specific layers,
    /// allowing for precise control over what can be damaged. Common layer
    /// setups include:
    /// - "Enemy" layer for player attacks
    /// - "Player" layer for enemy attacks  
    /// - "Destructible" layer for environmental objects
    /// - Combined layers for area-of-effect attacks
    /// 
    /// For more information about Unity layer masks:
    /// https://docs.unity3d.com/Manual/Layers.html
    /// </summary>
    [SerializeField] private LayerMask whatIsTarget;

    private void Awake()
    {
        stats = GetComponent<Entity_Stats>();
        vfx = GetComponent<Entity_VFX>();
    }

    /// <summary>
    /// Executes an attack by detecting and damaging targets within range.
    /// 
    /// This method is typically called by animation events during attack
    /// animations to ensure damage is applied at the correct moment (e.g.,
    /// when a sword swing reaches its peak). The process:
    /// 
    /// 1. Detects all colliders within the attack radius
    /// 2. Filters colliders based on the layer mask
    /// 3. Attempts to get IDamageable component from each target
    /// 4. Applies damage to valid targets
    /// 
    /// The null-conditional operator (?.) ensures that objects without
    /// IDamageable components are safely ignored rather than causing errors.
    /// </summary>
    public void PerformAttack()
    {
        GetDetectedColliders();

        foreach (var target in GetDetectedColliders())
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                AttackData attackData = stats.GetAttackData(basicAttackScale);

                float physicalDamage = attackData.physicalDamage;
                float elementalDamage = attackData.elementalDamage;
                ElementType element = attackData.element;

                bool targetDamaged = damageable.TakeDamage(physicalDamage, elementalDamage, element, transform);

                if (element != ElementType.None)
                {
                    target.GetComponent<Entity_StatusHandler>().ApplyStatusEffect(element, attackData.effectData);
                }

                if (targetDamaged)
                {
                    vfx.CreateOnHitVfx(target.transform, attackData.isCrit, element);
                }
            }
        }
    }

    /// <summary>
    /// Detects all colliders within the attack area using circular overlap detection.
    /// 
    /// This method uses Unity's Physics2D.OverlapCircleAll to find all colliders
    /// that intersect with a circle centered at the targetCheckPoint. The layer
    /// mask ensures only relevant objects are detected, improving performance
    /// and preventing unintended interactions.
    /// 
    /// The method is called twice in PerformAttack() - this could be optimized
    /// by caching the result or restructuring the method calls.
    /// </summary>
    /// <returns>
    /// An array of all Collider2D components found within the attack radius
    /// that match the specified layer mask.
    /// </returns>
    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCircleAll(targetCheckPoint.position, targetCheckRadius, whatIsTarget);
    }

    /// <summary>
    /// Draws visual gizmos in the Scene view for debugging attack ranges.
    /// 
    /// This method renders a wireframe sphere at the target check point with
    /// the configured radius, allowing developers to visualize and fine-tune
    /// attack ranges directly in the Unity editor. The gizmo is only visible
    /// in the Scene view and does not appear in the built game.
    /// 
    /// This is particularly useful for:
    /// - Balancing attack ranges
    /// - Debugging collision detection issues
    /// - Visualizing attack patterns during development
    /// 
    /// For more information about Unity gizmos:
    /// https://docs.unity3d.com/ScriptReference/Gizmos.html
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetCheckPoint.position, targetCheckRadius);
    }
}
