using System;
using UnityEngine;

[Serializable]
public class Stat_OffenseGroup
{
    // Physical Damage
    [Tooltip("Base physical damage as a whole number.")]
    public Stat damage;

    [Tooltip("Critical damage bonus as a whole percentage; 50.7 = +50.7% damage.")]
    public Stat critPower;

    [Tooltip("Fractional percentage chance to inflict a critical attack, which is amplified by Crit Power.")]
    public Stat critChance;

    [Tooltip("Fractional percentage to reduce the effective armor of the target; 0.2 = -20% reduction.")]
    public Stat armorPenetration;

    // Elemental Damage
    [Tooltip("Fire resistance as a whole percentage; 20.5 = 20.5% resist.")]
    public Stat fireDamage;

    [Tooltip("Ice resistance as a whole percentage; 30.5 = 30.5% resist.")]
    public Stat iceDamage;

    [Tooltip("Lightning resistance as a whole percentage; 40.5 = 40.5% resist.")]
    public Stat lightningDamage;
}
