using UnityEngine;

/// <summary>
/// Unity Scriptable Object (SO) that facilitates more efficient stat configuration
/// for entities (Players and Enemies).
/// This is like a blueprint that can be instantiated many times.
/// 
/// To create a new instance in Unity, Right click, then navigate to:
/// Create > RPG Setup > Default Stat Setup
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Default Stat Setup", fileName = "Default Stat Setup")]
public class Stat_SetupSO : ScriptableObject
{
    [Header("Resources")]
    public float maxHealth = 100;
    [Tooltip("How much health to regenerate every interval, typically every 1 second.")]
    public float healthRegen;

    [Header("Offense - Phyiscal Damage")]
    [Tooltip("Corresponds to Animator property 'attackSpeedMultiplier' and alters entity attack speed. Should default to 1.")]
    public float attackSpeed = 1;
    [Tooltip("Base physical damage as a whole number.")]
    public float damage = 10;
    [Tooltip("Fractional percentage chance to inflict a critical attack, which is amplified by Crit Power.")]
    public float critChance;
    [Tooltip("Critical damage as a whole percentage; 150.7 = +150.7% damage. Should be greater than 100.")]
    public float critPower = 150;
    [Tooltip("Fractional percentage to reduce the effective armor of the target; 0.2 = -20% reduction.")]
    public float armorPenetration;

    [Header("Offense - Elemental Damage")]
    public float fireDamage;
    public float iceDamage;
    public float lightningDamage;

    [Header("Defense - Phyiscal Damage")]
    [Tooltip("Mitigates incoming physical damage. Caps at 85% max reduction.")]
    public float armor;
    [Tooltip("Chance to evade as a whole percentage; 5 = 5% chance to evade attack.")]
    public float evasion;

    [Header("Defense - Elemental Damage")]
    [Tooltip("Fire resistance as a whole percentage; 10 = 10% resistance.")]
    public float fireResistance;
    [Tooltip("Ice resistance as a whole percentage; 15 = 15% resistance.")]
    public float iceResistance;
    [Tooltip("Lightning resistance as a whole percentage; 20 = 20% resistance.")]
    public float lightningResistance;

    [Header("Major Stats")]
    [Tooltip("Each STR point gives +0.5% critical power damage bonus")]
    public float strength;
    [Tooltip("Each AGI point gives 0.5% chance to dodge a physical attack")]
    public float agility;
    [Tooltip("Each INT point gives +1 bonus elemental damage and +0.5% elemental resistance")]
    public float intelligence;
    [Tooltip("Each point increases max HP +5")]
    public float vitality;
}
