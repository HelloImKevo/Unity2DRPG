using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat maxHealth;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;

    public float GetElementalDamage(out ElementType element)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue();

        float highestDamage = Mathf.Max(fireDamage, iceDamage, lightningDamage);

        // Mathf.Approximately(highestDamage, 0f) - shouldn't be necessary for Unity.
        if (highestDamage <= 0)
        {
            // Intelligence only provides BONUS damage. If there is no
            // base elemental damage, then return zero.
            element = ElementType.None;
            return 0;
        }

        element = GetHighestElementType();

        float bonusFire = (fireDamage == highestDamage) ? 0 : fireDamage * 0.5f;
        float bonusIce = (iceDamage == highestDamage) ? 0 : iceDamage * 0.5f;
        float bonusLightning = (lightningDamage == highestDamage) ? 0 : lightningDamage * 0.5f;

        // Minor bonus damage from the two weaker elements.
        float minorBonusFromWeakerElements = bonusFire + bonusIce + bonusLightning;

        float finalDamage = highestDamage + minorBonusFromWeakerElements + bonusElementalDamage;

        return finalDamage;
    }

    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * 0.5f;

        switch (element)
        {
            case ElementType.None:
                baseResistance = 0;
                break;

            case ElementType.Fire:
                baseResistance = defense.fireRes.GetValue();
                break;

            case ElementType.Ice:
                baseResistance = defense.iceRes.GetValue();
                break;

            case ElementType.Lightning:
                baseResistance = defense.lightningRes.GetValue();
                break;
        }

        Debug.Log($"{gameObject} -> Resist (base) = {baseResistance} Resist (bonus) = {bonusResistance}");

        float resistance = baseResistance + bonusResistance;
        // Max resistance cap as a whole percentage.
        float resistanceCap = 75f; // Traditional 75% max elemental resistance cap.
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100f;

        return finalResistance;
    }

    private ElementType GetHighestElementType()
    {
        ElementType primaryElement = ElementType.None;

        float highestDamage = 0;

        float fireDamage = offense.fireDamage.GetValue();
        if (fireDamage > 0)
        {
            highestDamage = fireDamage;
            primaryElement = ElementType.Fire;
        }

        float iceDamage = offense.iceDamage.GetValue();
        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            primaryElement = ElementType.Ice;
        }

        float lightningDamage = offense.lightningDamage.GetValue();
        if (lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            primaryElement = ElementType.Lightning;
        }

        return primaryElement;
    }

    public float GetPhysicalDamage(out bool isCrit)
    {
        float baseDamage = offense.damage.GetValue();
        float bonusDamage = major.strength.GetValue();
        float totalBaseDamage = baseDamage + bonusDamage;

        float baseCritChance = offense.critChance.GetValue();
        float bonusCritChance = major.agility.GetValue() * 0.3f;
        float critChance = baseCritChance + bonusCritChance;

        float baseCritPower = offense.critPower.GetValue();
        // Bonus crit power from strength: +0.5% per STR
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
        // Physical damage reduction as a fractional percentage.
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
