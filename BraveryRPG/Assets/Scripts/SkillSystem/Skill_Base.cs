using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    // public Player_SkillManager skillManager { get; private set; }
    // public Player player { get; private set; }
    // public DamageScaleData damageScaleData { get; private set; }


    [Header("General details")]
    // [SerializeField] protected SkillType skillType;
    // [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        // skillManager = GetComponentInParent<Player_SkillManager>();
        // player = GetComponentInParent<Player>();
        lastTimeUsed -= cooldown;
    }

    public virtual void TryUseSkill()
    {
        // Override when needed.
    }

    // public void SetSkillUpgrade(UpgradeData upgrade)
    // {
    //     upgradeType = upgrade.upgradeType;
    //     cooldown = upgrade.cooldown;
    //     damageScaleData = upgrade.damageScaleData;
    // }

    public bool CanUseSkill()
    {
        // if (SkillUpgradeType.None == upgradeType) return false;

        if (OnCooldown())
        {
            Debug.Log($"{gameObject.name} On Cooldown");
            return false;
        }

        return true;
    }

    // protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    /// <summary>
    /// Call when the skill was just used, and is now on cooldown.
    /// Some skills might only go on cooldown once the animation is finished,
    /// so we want our system to be flexible.
    /// </summary>
    public void SetSkillOnCooldown() => lastTimeUsed = Time.time;

    // TODO: Rename this function to 'ReduceCooldownBy' for clarity.
    /// <summary>
    /// Reduce cooldown by a specific amount.
    /// </summary>
    /// <param name="cooldownReduction"></param>
    public void ResetCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;

    public void ResetCooldown() => lastTimeUsed = Time.time;
}
