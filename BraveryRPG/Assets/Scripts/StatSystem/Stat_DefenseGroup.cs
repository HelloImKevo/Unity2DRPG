using System;
using UnityEngine;

[Serializable]
public class Stat_DefenseGroup
{
    #region Physical Defense

    /// <summary>
    /// Armor points that mitigate incoming physical damage, with diminishing
    /// effectiveness at higher values. Caps at a maximum of 85% reduction.
    /// </summary>
    public Stat armor;

    /// <summary>
    /// Chance to evade as a whole percentage; 5 = 5% chance to evade attack.
    /// </summary>
    public Stat evasion;

    #endregion

    #region Elemental Resistance

    /// <summary>
    /// Fire resistance as a whole percentage; 10 = 10% resistance.
    /// </summary>
    public Stat fireRes;

    /// <summary>
    /// Ice resistance as a whole percentage; 15 = 15% resistance.
    /// </summary>
    public Stat iceRes;

    /// <summary>
    /// Lightning resistance as a whole percentage; 20 = 20% resistance.
    /// </summary>
    public Stat lightningRes;

    #endregion
}
