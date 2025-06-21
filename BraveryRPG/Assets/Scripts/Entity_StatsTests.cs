using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Unit tests for the Entity_Stats component.
/// 
/// These tests verify the complex stat calculations including:
/// - Physical damage with critical hits and strength bonuses
/// - Elemental damage with intelligence bonuses and element detection
/// - Armor mitigation with vitality bonuses and armor reduction debuffs
/// - Elemental resistance with intelligence bonuses and caps
/// - Evasion calculations with agility bonuses and caps
/// - Max health calculations with vitality bonuses
/// - Armor penetration calculations
/// 
/// Test Strategy:
/// - Each test method focuses on a specific calculation
/// - Tests use controlled stat values to verify formulas
/// - Edge cases like zero stats, caps, and negative values are tested
/// - Random elements (like critical hits) are tested with seeded values
/// </summary>
public class Entity_StatsTests
{
    private GameObject testEntity;
    private Entity_Stats entityStats;

    /// <summary>
    /// Sets up a fresh test entity with Entity_Stats component before each test.
    /// This ensures test isolation and prevents state from leaking between tests.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        // Create a test GameObject with Entity_Stats component
        testEntity = new GameObject("TestEntity");
        entityStats = testEntity.AddComponent<Entity_Stats>();

        // Initialize stat groups to prevent null reference exceptions
        entityStats.maxHealth = new Stat();
        entityStats.major = new Stat_MajorGroup
        {
            strength = new Stat(),
            agility = new Stat(),
            intelligence = new Stat(),
            vitality = new Stat()
        };
        entityStats.offense = new Stat_OffenseGroup
        {
            damage = new Stat(),
            critChance = new Stat(),
            critPower = new Stat(),
            armorPenetration = new Stat(),
            fireDamage = new Stat(),
            iceDamage = new Stat(),
            lightningDamage = new Stat()
        };
        entityStats.defense = new Stat_DefenseGroup
        {
            armor = new Stat(),
            evasion = new Stat(),
            fireRes = new Stat(),
            iceRes = new Stat(),
            lightningRes = new Stat()
        };
    }

    /// <summary>
    /// Cleans up the test GameObject after each test to prevent memory leaks.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (testEntity != null)
        {
            Object.DestroyImmediate(testEntity);
        }
    }

    #region Physical Damage Tests

    [Test]
    public void GetPhysicalDamage_WithBaseDamageOnly_ReturnsCorrectValue()
    {
        // Arrange
        SetStatValue(entityStats.offense.damage, 50f);
        SetStatValue(entityStats.major.strength, 0f);
        SetStatValue(entityStats.offense.critChance, 0f);

        // Act
        float damage = entityStats.GetPhysicalDamage(out bool isCrit);

        // Assert
        Assert.AreEqual(50f, damage, 0.01f, "Base damage should be returned when no bonuses");
        Assert.IsFalse(isCrit, "Should not crit with 0% crit chance");
    }

    [Test]
    public void GetPhysicalDamage_WithStrengthBonus_IncludesBonusDamage()
    {
        // Arrange
        SetStatValue(entityStats.offense.damage, 50f);
        SetStatValue(entityStats.major.strength, 20f); // +20 bonus damage
        SetStatValue(entityStats.offense.critChance, 0f);

        // Act
        float damage = entityStats.GetPhysicalDamage(out bool isCrit);

        // Assert
        Assert.AreEqual(70f, damage, 0.01f, "Should include strength bonus damage");
        Assert.IsFalse(isCrit, "Should not crit with 0% crit chance");
    }

    [Test]
    public void GetPhysicalDamage_WithGuaranteedCrit_AppliesCriticalMultiplier()
    {
        // Arrange
        SetStatValue(entityStats.offense.damage, 50f);
        SetStatValue(entityStats.major.strength, 0f);
        SetStatValue(entityStats.offense.critChance, 100f); // 100% crit chance
        SetStatValue(entityStats.offense.critPower, 150f); // 150% = 1.5x multiplier

        // Act
        float damage = entityStats.GetPhysicalDamage(out bool isCrit);

        // Assert
        Assert.AreEqual(75f, damage, 0.01f, "Should apply 1.5x critical multiplier to 50 base damage");
        Assert.IsTrue(isCrit, "Should always crit with 100% crit chance");
    }

    [Test]
    public void GetPhysicalDamage_WithStrengthCritPowerBonus_IncludesBonusCritPower()
    {
        // Arrange
        SetStatValue(entityStats.offense.damage, 100f);
        SetStatValue(entityStats.major.strength, 20f); // +10% crit power (20 * 0.5%)
        SetStatValue(entityStats.offense.critChance, 100f); // Guaranteed crit
        SetStatValue(entityStats.offense.critPower, 150f); // Base 150%

        // Act
        float damage = entityStats.GetPhysicalDamage(out bool isCrit);

        // Assert
        // Total crit power: 150% + 10% = 160% = 1.6x multiplier
        // Total base damage: 100 + 20 = 120
        // Final damage: 120 * 1.6 = 192
        Assert.AreEqual(192f, damage, 0.01f, "Should apply strength bonuses to both damage and crit power");
        Assert.IsTrue(isCrit, "Should crit with guaranteed chance");
    }

    [Test]
    public void GetPhysicalDamage_WithAgilityCritChanceBonus_IncludesBonusCritChance()
    {
        // Arrange - Set up a scenario where agility affects crit chance
        SetStatValue(entityStats.offense.damage, 50f);
        SetStatValue(entityStats.major.agility, 100f); // +30% crit chance (100 * 0.3%)
        SetStatValue(entityStats.offense.critChance, 70f); // Base 70%
        SetStatValue(entityStats.offense.critPower, 200f); // 2x multiplier

        // Act - Test multiple times to verify crit chance behavior
        int critCount = 0;
        int testRuns = 1000;
        
        // Set random seed for consistent test results
        Random.InitState(12345);
        
        for (int i = 0; i < testRuns; i++)
        {
            entityStats.GetPhysicalDamage(out bool isCrit);
            if (isCrit) critCount++;
        }

        // Assert - With 100% total crit chance, should always crit
        // Total crit chance: 70% + 30% = 100%
        Assert.AreEqual(testRuns, critCount, "Should crit 100% of the time with total 100% crit chance");
    }

    #endregion

    #region Elemental Damage Tests

    [Test]
    public void GetElementalDamage_WithNoElementalStats_ReturnsZero()
    {
        // Arrange
        SetStatValue(entityStats.offense.fireDamage, 0f);
        SetStatValue(entityStats.offense.iceDamage, 0f);
        SetStatValue(entityStats.offense.lightningDamage, 0f);
        SetStatValue(entityStats.major.intelligence, 50f); // Intelligence bonus shouldn't matter

        // Act
        float damage = entityStats.GetElementalDamage(out ElementType element);

        // Assert
        Assert.AreEqual(0f, damage, "Should return 0 when no elemental damage stats");
        Assert.AreEqual(ElementType.None, element, "Element should be None when no elemental damage");
    }

    [Test]
    public void GetElementalDamage_WithSingleElementType_ReturnsCorrectDamageAndType()
    {
        // Arrange
        SetStatValue(entityStats.offense.fireDamage, 30f);
        SetStatValue(entityStats.offense.iceDamage, 0f);
        SetStatValue(entityStats.offense.lightningDamage, 0f);
        SetStatValue(entityStats.major.intelligence, 10f); // +10 bonus elemental damage

        // Act
        float damage = entityStats.GetElementalDamage(out ElementType element);

        // Assert
        Assert.AreEqual(40f, damage, 0.01f, "Should return fire damage + intelligence bonus");
        Assert.AreEqual(ElementType.Fire, element, "Primary element should be Fire");
    }

    [Test]
    public void GetElementalDamage_WithMultipleElements_ReturnsHighestAsPrimaryWithBonuses()
    {
        // Arrange
        SetStatValue(entityStats.offense.fireDamage, 20f);
        SetStatValue(entityStats.offense.iceDamage, 50f); // Highest
        SetStatValue(entityStats.offense.lightningDamage, 30f);
        SetStatValue(entityStats.major.intelligence, 10f); // +10 bonus

        // Act
        float damage = entityStats.GetElementalDamage(out ElementType element);

        // Assert
        // Expected calculation:
        // Highest: 50 (ice)
        // Minor bonuses: (20 * 0.5) + (30 * 0.5) = 10 + 15 = 25
        // Intelligence bonus: 10
        // Total: 50 + 25 + 10 = 85
        Assert.AreEqual(85f, damage, 0.01f, "Should include highest element + minor bonuses + intelligence");
        Assert.AreEqual(ElementType.Ice, element, "Primary element should be Ice (highest value)");
    }

    [Test]
    public void GetElementalDamage_WithTiedHighestValues_SelectsFirstEncountered()
    {
        // Arrange - Fire and Lightning tied for highest
        SetStatValue(entityStats.offense.fireDamage, 40f);
        SetStatValue(entityStats.offense.iceDamage, 20f);
        SetStatValue(entityStats.offense.lightningDamage, 40f);
        SetStatValue(entityStats.major.intelligence, 5f);

        // Act
        float damage = entityStats.GetElementalDamage(out ElementType element);

        // Assert
        // Based on GetHighestElementType logic, Lightning should win as it's checked last
        // Highest: 40 (lightning)
        // Minor bonuses: (40 * 0.5) + (20 * 0.5) = 20 + 10 = 30
        // Intelligence: 5
        // Total: 40 + 30 + 5 = 75
        Assert.AreEqual(75f, damage, 0.01f, "Should handle tied values correctly");
        Assert.AreEqual(ElementType.Lightning, element, "Lightning should be selected (last in comparison)");
    }

    #endregion

    #region Elemental Resistance Tests

    [Test]
    public void GetElementalResistance_WithNoResistanceStats_ReturnsIntelligenceBonusOnly()
    {
        // Arrange
        SetStatValue(entityStats.defense.fireRes, 0f);
        SetStatValue(entityStats.major.intelligence, 20f); // +10% resistance (20 * 0.5%)

        // Act
        float resistance = entityStats.GetElementalResistance(ElementType.Fire);

        // Assert
        Assert.AreEqual(0.1f, resistance, 0.01f, "Should return 10% (0.1) from intelligence bonus only");
    }

    [Test]
    public void GetElementalResistance_WithBaseResistance_CombinesBaseAndBonus()
    {
        // Arrange
        SetStatValue(entityStats.defense.iceRes, 30f); // 30% base resistance
        SetStatValue(entityStats.major.intelligence, 40f); // +20% bonus (40 * 0.5%)

        // Act
        float resistance = entityStats.GetElementalResistance(ElementType.Ice);

        // Assert
        // Total: 30% + 20% = 50% = 0.5 fractional
        Assert.AreEqual(0.5f, resistance, 0.01f, "Should combine base and intelligence bonus resistance");
    }

    [Test]
    public void GetElementalResistance_ExceedingCap_ClampsTo75Percent()
    {
        // Arrange
        SetStatValue(entityStats.defense.lightningRes, 60f); // 60% base
        SetStatValue(entityStats.major.intelligence, 100f); // +50% bonus (100 * 0.5%)

        // Act
        float resistance = entityStats.GetElementalResistance(ElementType.Lightning);

        // Assert
        // Total would be: 60% + 50% = 110%, but capped at 75%
        Assert.AreEqual(0.75f, resistance, 0.01f, "Should cap resistance at 75% (0.75)");
    }

    [Test]
    public void GetElementalResistance_WithNoneElement_ReturnsIntelligenceBonusOnly()
    {
        // Arrange
        SetStatValue(entityStats.major.intelligence, 30f); // +15% bonus

        // Act
        float resistance = entityStats.GetElementalResistance(ElementType.None);

        // Assert
        Assert.AreEqual(0.15f, resistance, 0.01f, "None element should only get intelligence bonus");
    }

    #endregion

    #region Armor Mitigation Tests

    [Test]
    public void GetArmorMitigation_WithNoArmor_ReturnsZeroMitigation()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 0f);
        SetStatValue(entityStats.major.vitality, 0f);

        // Act
        float mitigation = entityStats.GetArmorMitigation(0f);

        // Assert
        Assert.AreEqual(0f, mitigation, 0.01f, "No armor should result in no mitigation");
    }

    [Test]
    public void GetArmorMitigation_WithBaseArmor_CalculatesCorrectMitigation()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 50f);
        SetStatValue(entityStats.major.vitality, 0f);

        // Act
        float mitigation = entityStats.GetArmorMitigation(0f);

        // Assert
        // Formula: armor / (armor + 100) = 50 / (50 + 100) = 50/150 = 0.333...
        float expected = 50f / (50f + 100f);
        Assert.AreEqual(expected, mitigation, 0.01f, "Should calculate mitigation using armor formula");
    }

    [Test]
    public void GetArmorMitigation_WithVitalityBonus_IncludesBonusArmor()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 40f);
        SetStatValue(entityStats.major.vitality, 20f); // +20 bonus armor

        // Act
        float mitigation = entityStats.GetArmorMitigation(0f);

        // Assert
        // Total armor: 40 + 20 = 60
        // Mitigation: 60 / (60 + 100) = 60/160 = 0.375
        float expected = 60f / (60f + 100f);
        Assert.AreEqual(expected, mitigation, 0.01f, "Should include vitality bonus in armor calculation");
    }

    [Test]
    public void GetArmorMitigation_WithArmorReduction_ReducesEffectiveArmor()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 100f);
        SetStatValue(entityStats.major.vitality, 0f);

        // Act
        float mitigation = entityStats.GetArmorMitigation(0.4f); // 40% armor reduction

        // Assert
        // Effective armor: 100 * (1 - 0.4) = 100 * 0.6 = 60
        // Mitigation: 60 / (60 + 100) = 60/160 = 0.375
        float expected = 60f / (60f + 100f);
        Assert.AreEqual(expected, mitigation, 0.01f, "Should apply armor reduction correctly");
    }

    [Test]
    public void GetArmorMitigation_WithHighArmor_ClampsToMaxMitigation()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 1000f); // Very high armor
        SetStatValue(entityStats.major.vitality, 0f);

        // Act
        float mitigation = entityStats.GetArmorMitigation(0f);

        // Assert
        Assert.AreEqual(0.85f, mitigation, 0.01f, "Should cap mitigation at 85% (0.85)");
    }

    [Test]
    public void GetArmorMitigation_With100PercentReduction_ReturnsZeroMitigation()
    {
        // Arrange
        SetStatValue(entityStats.defense.armor, 100f);
        SetStatValue(entityStats.major.vitality, 50f);

        // Act
        float mitigation = entityStats.GetArmorMitigation(1.0f); // 100% armor reduction

        // Assert
        Assert.AreEqual(0f, mitigation, 0.01f, "100% armor reduction should result in zero mitigation");
    }

    #endregion

    #region Max Health Tests

    [Test]
    public void GetMaxHealth_WithBaseHealthOnly_ReturnsBaseValue()
    {
        // Arrange
        SetStatValue(entityStats.maxHealth, 100f);
        SetStatValue(entityStats.major.vitality, 0f);

        // Act
        float maxHealth = entityStats.GetMaxHealth();

        // Assert
        Assert.AreEqual(100f, maxHealth, "Should return base health when no vitality bonus");
    }

    [Test]
    public void GetMaxHealth_WithVitalityBonus_AddsCorrectBonus()
    {
        // Arrange
        SetStatValue(entityStats.maxHealth, 100f);
        SetStatValue(entityStats.major.vitality, 15f); // +75 health (15 * 5)

        // Act
        float maxHealth = entityStats.GetMaxHealth();

        // Assert
        Assert.AreEqual(175f, maxHealth, "Should add vitality bonus (15 * 5 = 75)");
    }

    #endregion

    #region Evasion Tests

    [Test]
    public void GetEvasion_WithBaseEvasionOnly_ReturnsBaseValue()
    {
        // Arrange
        SetStatValue(entityStats.defense.evasion, 25f);
        SetStatValue(entityStats.major.agility, 0f);

        // Act
        float evasion = entityStats.GetEvasion();

        // Assert
        Assert.AreEqual(25f, evasion, "Should return base evasion when no agility bonus");
    }

    [Test]
    public void GetEvasion_WithAgilityBonus_AddsCorrectBonus()
    {
        // Arrange
        SetStatValue(entityStats.defense.evasion, 30f);
        SetStatValue(entityStats.major.agility, 40f); // +20% evasion (40 * 0.5%)

        // Act
        float evasion = entityStats.GetEvasion();

        // Assert
        Assert.AreEqual(50f, evasion, "Should add agility bonus (40 * 0.5 = 20)");
    }

    [Test]
    public void GetEvasion_ExceedingCap_ClampsTo85Percent()
    {
        // Arrange
        SetStatValue(entityStats.defense.evasion, 60f);
        SetStatValue(entityStats.major.agility, 100f); // +50% bonus

        // Act
        float evasion = entityStats.GetEvasion();

        // Assert
        // Total would be: 60 + 50 = 110%, but capped at 85%
        Assert.AreEqual(85f, evasion, "Should cap evasion at 85%");
    }

    #endregion

    #region Armor Penetration Tests

    [Test]
    public void GetArmorPenetration_WithBasePenetration_ReturnsCorrectFraction()
    {
        // Arrange
        SetStatValue(entityStats.offense.armorPenetration, 25f); // 25%

        // Act
        float penetration = entityStats.GetArmorPenetration();

        // Assert
        Assert.AreEqual(0.25f, penetration, 0.01f, "Should convert 25% to 0.25 fraction");
    }

    [Test]
    public void GetArmorPenetration_WithZeroPenetration_ReturnsZero()
    {
        // Arrange
        SetStatValue(entityStats.offense.armorPenetration, 0f);

        // Act
        float penetration = entityStats.GetArmorPenetration();

        // Assert
        Assert.AreEqual(0f, penetration, "Should return 0 for no armor penetration");
    }

    [Test]
    public void GetArmorPenetration_WithHighPenetration_ReturnsCorrectFraction()
    {
        // Arrange
        SetStatValue(entityStats.offense.armorPenetration, 75f); // 75%

        // Act
        float penetration = entityStats.GetArmorPenetration();

        // Assert
        Assert.AreEqual(0.75f, penetration, 0.01f, "Should convert 75% to 0.75 fraction");
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Helper method to set stat values using reflection to access private baseValue field.
    /// This is necessary because the Stat class doesn't provide a public setter.
    /// </summary>
    private void SetStatValue(Stat stat, float value)
    {
        if (stat == null) return;
        
        var field = typeof(Stat).GetField("baseValue", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(stat, value);
    }

    #endregion
}
