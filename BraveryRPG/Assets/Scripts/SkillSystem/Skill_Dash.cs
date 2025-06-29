using UnityEngine;

public class Skill_Dash : Skill_Base
{

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

    private void CreateShard()
    {
        SkillManager.Shard.CreateRawShard();
    }

    private void CreateClone()
    {
        Debug.Log("Create time echo!");
        // skill manager clone create clone
    }
}
