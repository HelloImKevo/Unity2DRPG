/// <summary>
/// Simple utility class for common RPG calculations.
/// This class contains pure functions with no Unity dependencies,
/// making it perfect for unit testing without Unity Editor.
/// </summary>
public static class RPGMath
{
    /// <summary>
    /// Calculates damage with a percentage bonus.
    /// </summary>
    /// <param name="baseDamage">The base damage amount</param>
    /// <param name="bonusPercent">Bonus percentage (e.g., 20 for +20%)</param>
    /// <returns>Total damage including bonus</returns>
    public static float CalculateDamageWithBonus(float baseDamage, float bonusPercent)
    {
        if (baseDamage < 0) return 0;
        return baseDamage * (1 + bonusPercent / 100f);
    }

    /// <summary>
    /// Calculates percentage between current and maximum values.
    /// </summary>
    /// <param name="current">Current value</param>
    /// <param name="maximum">Maximum value</param>
    /// <returns>Percentage as a value between 0 and 100</returns>
    public static float CalculatePercentage(float current, float maximum)
    {
        if (maximum <= 0) return 0;
        return (current / maximum) * 100f;
    }

    /// <summary>
    /// Applies damage reduction based on armor value.
    /// </summary>
    /// <param name="incomingDamage">The incoming damage amount</param>
    /// <param name="armorValue">The armor rating</param>
    /// <returns>Damage after armor mitigation</returns>
    public static float ApplyArmorMitigation(float incomingDamage, float armorValue)
    {
        if (incomingDamage <= 0 || armorValue <= 0) return incomingDamage;
        
        // Standard RPG armor formula: damage reduction = armor / (armor + 100)
        float damageReduction = armorValue / (armorValue + 100f);
        return incomingDamage * (1f - damageReduction);
    }

    /// <summary>
    /// Calculates critical hit damage multiplier.
    /// </summary>
    /// <param name="baseDamage">Base damage before critical</param>
    /// <param name="criticalMultiplier">Critical hit multiplier (e.g., 2.0 for double damage)</param>
    /// <returns>Critical hit damage</returns>
    public static float CalculateCriticalDamage(float baseDamage, float criticalMultiplier = 2.0f)
    {
        return baseDamage * criticalMultiplier;
    }

    /// <summary>
    /// Clamps a value between a minimum and maximum.
    /// </summary>
    /// <param name="value">Value to clamp</param>
    /// <param name="min">Minimum allowed value</param>
    /// <param name="max">Maximum allowed value</param>
    /// <returns>Clamped value</returns>
    public static float Clamp(float value, float min, float max)
    {
        if (value < min) return min;
        if (value > max) return max;
        return value;
    }
}
