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
    public SkillType skillType;
    public UpgradeData upgradeData;
}

[Serializable]
public class UpgradeData
{
    public SkillUpgradeType upgradeType;
    public float cooldown;
    // public DamageScaleData damageScaleData;
}
