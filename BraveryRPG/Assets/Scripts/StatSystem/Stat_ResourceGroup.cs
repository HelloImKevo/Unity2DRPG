using System;
using UnityEngine;

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
