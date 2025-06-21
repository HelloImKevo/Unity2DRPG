using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    // Physical Damage
    [Tooltip("Base physical damage as a whole number.")]
    public Stat damage;

    [Tooltip("Critical damage bonus as a whole number; 50 = +50% damage.")]
    public Stat critPower;

    [Tooltip("Percentage chance to inflict a critical attack, which is amplified by Crit Power.")]
    public Stat critChance;

    [Tooltip("Percentage to reduce the effective armor of the target; 0.2 = -20% reduction.")]
    public Stat armorPenetration;

    // Elemental Damage
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lightningDamage;
}
