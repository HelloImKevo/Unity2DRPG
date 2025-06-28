using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash Dash { get; private set; }
    public Skill_Shard Shard { get; private set; }

    private void Awake()
    {
        Dash = GetComponentInChildren<Skill_Dash>();
        Shard = GetComponentInChildren<Skill_Shard>();
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash: return Dash;
            case SkillType.TimeShard: return Shard;

            default:
                Debug.Log($"Skill type {type} is not implemented yet.");
                return null;
        }
    }
}
