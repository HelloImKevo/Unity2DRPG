using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    public Player_SkillManager SkillManager { get; private set; }
    public Player PlayerRef { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }

    [Header("General Details")]
    [Tooltip("Which core skill does this represent?")]
    [SerializeField] protected SkillType skillType;
    [Tooltip("Which tier of Skill capability has been unlocked? Choose 'None' if the core Skill hasn't been unlocked.")]
    [SerializeField] protected SkillUpgradeType upgradeType;
    [SerializeField] protected float cooldown;
    private float lastTimeUsed;

    protected virtual void Awake()
    {
        SkillManager = GetComponentInParent<Player_SkillManager>();
        PlayerRef = GetComponentInParent<Player>();
        lastTimeUsed -= cooldown;

        // If a SkillObject is created without the base Skill being unlocked,
        // then the Damage Scale Data will likely be null.
        if (damageScaleData == null)
        {
            Debug.Log($"{GetType().Name} -> Awake() -> damageScaleData is null - Has the corresponding Skill been unlocked?");
            damageScaleData = new DamageScaleData();
        }
    }

    public virtual void TryUseSkill()
    {
        // Override when needed.
    }

    public void SetSkillUpgrade(Skill_DataSO skillData)
    {
        UpgradeData upgrade = skillData.upgradeData;

        upgradeType = upgrade.upgradeType;
        cooldown = upgrade.cooldown;
        damageScaleData = upgrade.damageScaleData;

        GetAssociatedUISkillSlot().SetupSkillSlot(skillData);

        // Allow skills to be immediately used when they are first unlocked.
        ResetCooldown();
    }

    public virtual bool CanUseSkill()
    {
        // If the skill is not unlocked, you cannot use it.
        if (SkillUpgradeType.None == upgradeType) return false;

        Debug.Log($"CanUseSkill() -> upgradeType = {upgradeType}");

        if (OnCooldown())
        {
            Debug.Log($"{GetType().Name} On Cooldown");
            return false;
        }

        return true;
    }

    private UI_SkillSlot GetAssociatedUISkillSlot()
    {
        return PlayerRef.UserInterface.inGameUI.GetSkillSlot(skillType);
    }

    protected bool Unlocked(SkillUpgradeType upgradeToCheck) => upgradeType == upgradeToCheck;

    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;

    /// <summary>
    /// Call when the skill was just used, and is now on cooldown.
    /// Some skills might only go on cooldown once the animation is finished,
    /// so we want our system to be flexible.
    /// </summary>
    public void SetSkillOnCooldown()
    {
        GetAssociatedUISkillSlot().StartCooldownVisual(cooldown);
        lastTimeUsed = Time.time;
    }

    /// <summary>
    /// Reduce cooldown by a specific amount.
    /// </summary>
    /// <param name="cooldownReduction"></param>
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed += cooldownReduction;

    public void ResetCooldown()
    {
        GetAssociatedUISkillSlot().ResetCooldownVisual();
        lastTimeUsed = Time.time - cooldown;
    }

    // TODO: Need to enforce that 'Unlocked By Default' skills cannot be refunded!
    public void RefundSkill()
    {
        upgradeType = SkillUpgradeType.None;
        ResetCooldown();
    }
}
