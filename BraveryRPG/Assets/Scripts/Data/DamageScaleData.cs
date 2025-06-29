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
    public float chillDuration = 3f;
    public float chillSlowMulitplier = 0.2f;

    [Header("Burn")]
    public float burnDuration = 3f;
    public float burnDamageScale = 1f;


    [Header("Shock")]
    public float shockDuration = 3f;
    public float shockDamageScale = 1f;
    public float shockCharge = 0.4f;
}
