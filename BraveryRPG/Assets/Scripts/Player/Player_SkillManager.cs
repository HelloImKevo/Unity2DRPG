using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{
    public Skill_Dash Dash { get; private set; }
    public Skill_Shard Shard { get; private set; }
    public Skill_ThrowSword ThrowSword { get; private set; }
    public Skill_TimeEcho TimeEcho { get; private set; }

    private void Awake()
    {
        Dash = GetComponentInChildren<Skill_Dash>();
        Shard = GetComponentInChildren<Skill_Shard>();
        ThrowSword = GetComponentInChildren<Skill_ThrowSword>();
        TimeEcho = GetComponentInChildren<Skill_TimeEcho>();
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash: return Dash;
            case SkillType.TimeShard: return Shard;
            case SkillType.ThrowSword: return ThrowSword;
            case SkillType.TimeEcho: return TimeEcho;

            default:
                Debug.Log($"Skill type {type} is not implemented yet.");
                return null;
        }
    }
}
