using System;
using UnityEngine;

/// <summary>
/// Unity Scriptable Object (SO) that defines a blueprint for Player skills
/// that can be unlocked through a Skill Tree progression system.
/// 
/// To create a new instance in Unity, Right click, then navigate to:
/// Create > RPG Setup > Skill Data
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill Description")]
    /// <summary>User-friendly name displayed in the skill tree UI.</summary>
    public string displayName;

    /// <summary>Detailed description of the skill's effects and mechanics.</summary>
    [TextArea]
    public string description;

    /// <summary>Icon sprite displayed in the skill tree node.</summary>
    public Sprite icon;

    /// <summary>Skill points required to unlock this skill.</summary>
    [Header("Unlock & Upgrade")]
    [Tooltip("Cost to purchase and unlock the skill.")]
    public int cost;

    /// <summary>Whether this skill is available from the start without unlocking.</summary>
    public bool unlockedByDefault;

    /// <summary>Category type of this skill for filtering and organization.</summary>
    public SkillType skillType;

    /// <summary>Configuration data for skill upgrades and effects.</summary>
    public UpgradeData upgradeData;
}

/// <summary>
/// Configuration data for skill upgrades including cooldown timing, upgrade benefits,
/// and damage scaling parameters.
/// </summary>
[Serializable]
public class UpgradeData
{
    /// <summary>Type of benefit or enhancement this upgrade provides.</summary>
    [Tooltip("What benefit or perk does this upgrade provide?")]
    public SkillUpgradeType upgradeType;

    /// <summary>Cooldown duration in seconds between skill uses. Zero means no cooldown.</summary>
    [Tooltip("Cooldown in seconds - How long before the skill can be used again. Defaults to zero, meaning 'No cooldown'.")]
    public float cooldown = 0f;

    /// <summary>Damage scaling configuration for offensive skills.</summary>
    public DamageScaleData damageScaleData;
}
