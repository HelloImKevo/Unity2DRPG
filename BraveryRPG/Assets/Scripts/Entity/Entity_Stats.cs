using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;

    public float GetPhysicalDamage(out bool isCrit)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f;
        float critChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        // Bonus crit chance from strength: +0.5% per STR
        float bonusCritPower = major.strength.GetValue() * 0.5f;
        // Total crit power as multiplier (ex: 150 / 100 = 1.5f)
        float critPower = (baseCritPower + bonusCritPower) / 100f;

        isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? totalBaseDamage * critPower : totalBaseDamage;

        return finalDamage;
    }

    /// <summary>
    /// Calculates armor mitigation percentage for this entity.
    /// </summary>
    /// <param name="armorReduction">Opional debuff percentage to reduce the total armor of this entity.</param>
    /// <returns>Armor mitigation as a fractional percentage. 0.85 is 85%</returns>
    public float GetArmorMitigation(float armorReduction)
    {
        float baseArmor = defense.armor.GetValue();
        float bonusArmor = major.vitality.GetValue() * 1.0f;
        float totalArmor = baseArmor + bonusArmor;

        // Cap armor reduction to 100% (handle situations where you receive a 100% armor reduction debuff)
        float reductionMultiplier = Mathf.Clamp01(1 - armorReduction); // 1 - 0.4 = 0.6
        float effectiveArmor = totalArmor * reductionMultiplier;

        // 50 = Easy mode, less armor required to reach mitigation cap
        // 150 = Hard mode, more armor required to reach mitigtaion cap
        const float scalingConstant = 100f;
        float mitigation = effectiveArmor / (effectiveArmor + scalingConstant);
        float mitigationCap = 0.85f; // 85% Max Mitigation

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    /// <returns>
    /// Fractional percentage of armor penetration, which reduces the effective armor
    /// of the target of an attack.
    /// </returns>
    public float GetArmorPenetration()
    {
        // Total armor reduction as multiplier (ex: 30 / 100 = 0.3f multiplier)
        float finalReduction = offense.armorPenetration.GetValue() / 100f;

        return finalReduction;
    }

    public float GetMaxHealth()
    {
        float baseMaxHealth = maxHealth.GetValue();
        float bonusMaxHealth = major.vitality.GetValue() * 5;
        float finalMaxHealth = baseMaxHealth + bonusMaxHealth;

        return finalMaxHealth;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * 0.5f;

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f; // Evasion capped at 85%

        // Evasion must be between 0 - 85.
        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }
}
