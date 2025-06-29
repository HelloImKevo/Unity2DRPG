using System;
using UnityEngine;

/// <summary>
/// Contains offensive combat statistics including physical damage, critical hit mechanics,
/// and elemental damage types. Manages all aspects of damage output and attack modifiers.
/// </summary>
[Serializable]
public class Stat_OffenseGroup
{
    // Physical Damage
    /// <summary>
    /// 1 = Default Attack Speed, 1.5 = +50% Increased Attack Speed.
    /// This value should not be much higher than 3 (300%), because
    /// it will start to break animations become very overpowered.
    /// </summary>
    [Tooltip("Corresponds to Animator property 'attackSpeedMultiplier' and alters entity attack speed. Should default to 1.")]
    public Stat attackSpeed;

    [Tooltip("Base physical damage as a whole number.")]
    public Stat damage;

    [Tooltip("Fractional percentage chance to inflict a critical attack, which is amplified by Crit Power.")]
    public Stat critChance;

    [Tooltip("Critical damage as a whole percentage; 150.7 = +150.7% damage. Should be greater than 100.")]
    public Stat critPower;

    [Tooltip("Fractional percentage to reduce the effective armor of the target; 0.2 = -20% reduction.")]
    public Stat armorPenetration;

    // Elemental Damage
    [Tooltip("Base Fire elemental damage.")]
    public Stat fireDamage;

    [Tooltip("Base Ice elemental damage.")]
    public Stat iceDamage;

    [Tooltip("Base Lightning elemental damage.")]
    public Stat lightningDamage;
}
