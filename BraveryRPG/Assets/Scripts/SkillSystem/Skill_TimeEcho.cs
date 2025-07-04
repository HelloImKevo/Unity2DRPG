using UnityEngine;

public class Skill_TimeEcho : Skill_Base
{
    private SkillObject_TimeEcho lastEchoCreated;

    [SerializeField] private GameObject timeEchoPrefab;
    [SerializeField] private float timeEchoDuration;

    [Header("Time Echo Attack Upgrades")]
    [SerializeField] private int maxAttacks = 3;
    [Tooltip("Fractional percent chance that this Echo will create a duplicate Echo upon death and if an enemy was hit.")]
    [SerializeField] private float duplicateChance = 0.3f;

    // [Header("Heal Wisp Upgrades")]
    // [SerializeField] private float damagePercentHealed = 0.3f;
    // [SerializeField] private float cooldownReducedInSeconds;

    // public float GetPercentOfDamageHealed()
    // {
    //     if (ShouldBeWisp() == false)
    //     {
    //         return 0;
    //     }

    //     return damagePercentHealed;
    // }

    // public float GetCooldownReduceInSeconds()
    // {
    //     if (upgradeType != SkillUpgradeType.TimeEcho_CooldownWisp)
    //     {
    //         return 0;
    //     }

    //     return cooldownReducedInSeconds;
    // }

    // public bool CanRemoveNegativeEffects()
    // {
    //     return upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp;
    // }

    // public bool ShouldBeWisp()
    // {
    //     return upgradeType == SkillUpgradeType.TimeEcho_HealWisp
    //         || upgradeType == SkillUpgradeType.TimeEcho_CleanseWisp
    //         || upgradeType == SkillUpgradeType.TimeEcho_CooldownWisp;
    // }

    public float GetDuplicateChance()
    {
        if (upgradeType != SkillUpgradeType.TimeEcho_ChanceToDuplicate)
        {
            return 0f;
        }

        return duplicateChance;
    }

    public int GetMaxAttacks()
    {
        if (SkillUpgradeType.TimeEcho_SingleAttack == upgradeType
            || SkillUpgradeType.TimeEcho_ChanceToDuplicate == upgradeType)
        {
            return 1;
        }

        if (SkillUpgradeType.TimeEcho_MultiAttack == upgradeType)
        {
            return maxAttacks;
        }

        return 0;
    }

    public float GetEchoDuration()
    {
        return timeEchoDuration;
    }

    public override void TryUseSkill()
    {
        if (!CanUseSkill()) return;

        CreateTimeEcho();
    }

    public void CreateTimeEcho(Vector3? targetPosition = null)
    {
        Vector3 position = targetPosition ?? transform.position;

        GameObject timeEcho = Instantiate(timeEchoPrefab, position, Quaternion.identity);
        timeEcho.GetComponent<SkillObject_TimeEcho>().SetupEcho(this);
    }
}
