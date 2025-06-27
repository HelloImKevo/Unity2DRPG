using System;
using UnityEngine;

/// <summary>
/// Contains the four primary character attributes that form the foundation of character progression.
/// These stats influence various aspects of combat, survivability, and magical abilities.
/// </summary>
[Serializable]
public class Stat_MajorGroup
{
    [Tooltip("Each STR point gives +0.5% critical power damage bonus")]
    public Stat strength;

    [Tooltip("Each AGI point gives 0.5% chance to dodge a physical attack")]
    public Stat agility;

    [Tooltip("Each INT point gives +1 bonus elemental damage and +0.5% elemental resistance")]
    public Stat intelligence;

    /// <summary>
    /// Each point increases max HP +5
    /// Bonus armor from Vitality: +1 per VIT
    /// </summary>
    [Tooltip("Each point increases max HP +5")]
    public Stat vitality;
}
