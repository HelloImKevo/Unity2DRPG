public class ElementalEffectData
{
    public float chillDuration;
    public float chillSlowMultiplier;

    public float burnDuratoin;
    public float totalBurnDamage;

    public float shockDuration;
    public float shockDamage;
    public float shockChargeBuildup;

    public ElementalEffectData(Entity_Stats entityStats, DamageScaleData damageScale)
    {
        chillDuration = damageScale.chillDuration;
        chillSlowMultiplier = damageScale.chillSlowMulitplier;

        burnDuratoin = damageScale.burnDuration;
        totalBurnDamage = entityStats.offense.fireDamage.GetValue() * damageScale.burnDamageScale;

        shockDuration = damageScale.shockDuration;
        shockDamage = entityStats.offense.lightningDamage.GetValue() * damageScale.shockDamageScale;
        shockChargeBuildup = damageScale.shockChargeBuildup;
    }
}
