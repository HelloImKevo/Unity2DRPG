using System;
using UnityEngine;

/// <summary>
/// Unity Scriptable Object (SO) that defines a blueprint for Player skills
// that can be unlocked through a Skill Tree progression system.
/// 
/// To create a new instance in Unity, Right click, then navigate to:
/// Create > RPG Setup > Skill Data
/// </summary>
[CreateAssetMenu(menuName = "RPG Setup/Skill Data", fileName = "Skill Data - ")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill Description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock & Upgrade")]
    [Tooltip("Cost to purchase and unlock the skill.")]
    public int cost;
    public bool unlockedByDefault;
    public SkillType skillType;
    public UpgradeData upgradeData;
}

[Serializable]
public class UpgradeData
{
    [Tooltip("What benefit or perk does this upgrade provide?")]
    public SkillUpgradeType upgradeType;
    [Tooltip("Cooldown in seconds - How long before the skill can be used again. Defaults to zero, meaning 'No cooldown'.")]
    public float cooldown = 0f;
    public DamageScaleData damageScaleData;
}
