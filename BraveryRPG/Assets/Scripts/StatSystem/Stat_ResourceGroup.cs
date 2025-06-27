using System;
using UnityEngine;

/// <summary>
/// Contains character resource statistics such as health and regeneration rates.
/// Manages the vital resources that determine character survivability.
/// </summary>
[Serializable]
public class Stat_ResourceGroup
{
    public Stat maxHealth;

    /// <summary>
    /// How much health to regenerate every interval, typically every 1 second.
    /// </summary>
    [Tooltip("How much health to regenerate every interval, typically every 1 second.")]
    public Stat healthRegen;
}
