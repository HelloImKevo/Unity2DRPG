using System;
using UnityEngine;

[Serializable]
public class DamageScaleData
{
    [Header("Damage")]
    [Tooltip("Scaling factor for physical damage. Default 1 = 100%")]
    public float physical = 1f;
    [Tooltip("Scaling factor for elemental damage. Default 1 = 100%")]
    public float elemental = 1f;

    [Header("Chill")]
    [Tooltip("Duration of status effects applied by this entity to its targets, in seconds.")]
    public float chillDuration = 3f;
    [Range(0, 1)]
    [Tooltip("Slow effect applied to targets of this entity as a fractional percentage; 0.2 = 20% slow effect.")]
    public float chillSlowMulitplier = 0.2f;

    [Header("Burn")]
    public float burnDuration = 3f;
    [Tooltip("Burn DoT (Damage over Time) damage scaling.")]
    public float burnDamageScale = 1f;

    [Header("Shock")]
    [Tooltip("Duration of status effects applied by this entity to its targets, in seconds.")]
    public float shockDuration = 3f;
    [Tooltip("Lightning Strike damage scaling when enough charges are built up.")]
    public float shockDamageScale = 1f;
    [Range(0, 1)]
    [Tooltip("Multiple attacks build up electrical charges, which then result in a Lightning Strike.")]
    public float shockChargeBuildup = 0.4f;
}
