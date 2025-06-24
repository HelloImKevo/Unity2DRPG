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
    [Tooltip("Cost to purchase and unlock the skill.")]
    public int cost;

    // TODO: Need to figure out localization for translations in the Unity infrastructure.

    [Header("Skill Description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;
}
