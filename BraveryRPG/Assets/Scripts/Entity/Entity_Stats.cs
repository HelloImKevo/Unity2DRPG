using UnityEngine;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetupSO defaultStatSetup;

    /// <summary>
    /// Resources like Health, Mana, Rage, Energy, etc.
    /// </summary>
    public Stat_ResourceGroup resources;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;
    public Stat_MajorGroup major;

    protected virtual void Awake()
    {
        // Override where needed.
    }

    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }

    /// <param name="scaleFactor">Fractional percentage value between 0.0 - 2.0 (200%).</param>
    public float GetElementalDamage(out ElementType element, float scaleFactor = 1f)
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

        return finalDamage * scaleFactor;
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

    /// <param name="scaleFactor">Fractional percentage value between 0.0 - 2.0 (200%).</param>
    public float GetPhysicalDamage(out bool isCrit, float scaleFactor = 1f)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();

        // Total crit power as multiplier (ex: 150 / 100 = 1.5f)
        float critPower = GetCritPower() / 100f;

        isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalDamage * scaleFactor;
    }

    /// <summary>
    /// Bonus damage from Strength: +1 per STR.
    /// </summary>
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue();

    /// <summary>
    ///  Bonus crit chance from Agility: +0.3% per AGI.
    /// </summary>
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * 0.3f);

    /// <summary>
    /// Bonus crit chance from Strength: +0.5% per STR.
    /// </summary>
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * 0.5f);

    /// <summary>
    /// Calculates armor mitigation percentage for this entity.
    /// </summary>
    /// <param name="armorReduction">Opional debuff percentage to reduce the total armor of this entity.</param>
    /// <returns>Armor mitigation as a fractional percentage. 0.85 is 85%</returns>
    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

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

    /// <summary>
    /// Bonus armor from Vitality: +1 per VIT 
    /// </summary>
    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue();

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
        float baseMaxHealth = resources.maxHealth.GetValue();
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

    // TODO: Explore refactoring this implementation to embed the StatType
    // in each Stat instance, and then iterate over the entity stat collection?
    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            // Resource
            case StatType.MaxHealth: return resources.maxHealth;
            case StatType.HealthRegen: return resources.healthRegen;

            // Major
            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;

            // Offense
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorPenetration: return offense.armorPenetration;

            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;

            // Defense
            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;

            case StatType.IceResistance: return defense.iceRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }

    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultStatSetup == null)
        {
            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.maxHealth.SetBaseValue(defaultStatSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultStatSetup.healthRegen);

        major.strength.SetBaseValue(defaultStatSetup.strength);
        major.agility.SetBaseValue(defaultStatSetup.agility);
        major.intelligence.SetBaseValue(defaultStatSetup.intelligence);
        major.vitality.SetBaseValue(defaultStatSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaultStatSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultStatSetup.damage);
        offense.critChance.SetBaseValue(defaultStatSetup.critChance);
        offense.critPower.SetBaseValue(defaultStatSetup.critPower);
        offense.armorPenetration.SetBaseValue(defaultStatSetup.armorPenetration);

        offense.iceDamage.SetBaseValue(defaultStatSetup.iceDamage);
        offense.fireDamage.SetBaseValue(defaultStatSetup.fireDamage);
        offense.lightningDamage.SetBaseValue(defaultStatSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultStatSetup.armor);
        defense.evasion.SetBaseValue(defaultStatSetup.evasion);

        defense.iceRes.SetBaseValue(defaultStatSetup.iceResistance);
        defense.fireRes.SetBaseValue(defaultStatSetup.fireResistance);
        defense.lightningRes.SetBaseValue(defaultStatSetup.lightningResistance);
    }
}
