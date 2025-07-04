using UnityEngine;

/// <summary>
/// Dash skill that provides instant movement with optional special effects like clones
/// and shards at the start and end positions based on unlocked upgrades.
/// </summary>
public class Skill_Dash : Skill_Base
{
    /// <summary>
    /// Triggers effects when the dash begins, creating clones or shards based on unlocked
    /// upgrade types for dash start effects.
    /// </summary>
    public void OnStartEffect()
    {
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStart)
            || Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
        {
            CreateClone();
        }

        if (Unlocked(SkillUpgradeType.Dash_ShardOnShart)
            || Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
        {
            CreateShard();
        }
    }

    /// <summary>
    /// Triggers effects when the dash ends, creating additional clones or shards at the
    /// arrival position for upgrade types that affect both start and end locations.
    /// </summary>
    public void OnEndEffect()
    {
        if (Unlocked(SkillUpgradeType.Dash_CloneOnStartAndArrival))
        {
            CreateClone();
        }

        if (Unlocked(SkillUpgradeType.Dash_ShardOnStartAndArrival))
        {
            CreateShard();
        }
    }

    /// <summary>
    /// Creates a shard projectile at the current position via the skill manager.
    /// </summary>
    private void CreateShard()
    {
        SkillManager.Shard.CreateRawShard();
    }

    /// <summary>
    /// Creates a time echo clone at the current position. Currently logs debug message
    /// as placeholder for clone creation system.
    /// </summary>
    private void CreateClone()
    {
        SkillManager.TimeEcho.CreateTimeEcho();
    }
}
