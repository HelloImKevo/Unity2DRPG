using UnityEngine;

/// <summary>
/// Interface for game objects that can receive damage in the RPG system.
/// 
/// This interface defines a contract that all destructible entities must follow,
/// ensuring consistent damage handling across different game objects like players,
/// enemies, chests, and environmental objects.
/// 
/// For more information about Unity interfaces and game architecture patterns:
/// https://docs.unity3d.com/Manual/index.html
/// https://refactoring.guru/design-patterns/strategy
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Called when this entity should receive damage from an external source.
    /// 
    /// This method is typically invoked during combat interactions, environmental
    /// hazards, or any scenario where one game object inflicts damage on another.
    /// The implementation should handle health reduction, visual effects, audio
    /// feedback, and any death/destruction logic.
    /// </summary>
    /// <param name="damage">
    /// The amount of damage to inflict on this entity. Should be a positive value
    /// representing health points to subtract from the entity's current health.
    /// </param>
    /// <param name="damageDealer">
    /// The Transform component of the game object that is causing the damage.
    /// This reference allows the damaged entity to determine damage direction
    /// for knockback effects, identify the source for gameplay logic (e.g.,
    /// enemy aggro systems), and access components of the attacking entity.
    /// </param>
    public bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer);
}
