using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private Entity_VFX entityVfx;

    [SerializeField] private ElementType currentEffect = ElementType.None;

    [Header("Electrify Effect Details")]
    [SerializeField] private GameObject lightningStrikeVfx;
    // Fractional percentage value. Once it reaches 1.0 (100%) a lightning strike is performed.
    [SerializeField] private float currentCharge;
    [Tooltip("How much charge must be built up to trigger a Lightning Strike. 1.0 kind of means 100% charge.")]
    [SerializeField] private float maximumCharge = 1f;
    private Coroutine electrifyCo;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    #region Electrify Lightning Strike Effect

    /// <summary>
    /// Builds up charge of electricity. If there are enough charges,
    /// perform a lightning strike.
    /// </summary>
    public void ApplyElectrifyEffect(float duration, float damage, float charge)
    {
        // Dealer applies electrify effect to target (Charge build up: 25%, base damage: 43)
        // Target has 40% Lightning Resistance.
        // Charge build up: 25% * (1 - 0.4) = 15% Charge On Hit
        float lightningResistance = entityStats.GetElementalResistance(ElementType.Lightning);
        float finalCharge = charge * (1 - lightningResistance);
        currentCharge += finalCharge;

        if (currentCharge >= maximumCharge)
        {
            DoLightningStrike(damage);
            StopElectrifyEffect();
            return;
        }

        // Reset the window threshold and blinking, to allow more charges to build up.
        if (electrifyCo != null) StopCoroutine(electrifyCo);

        electrifyCo = StartCoroutine(ElectrifyBlinkEffectCo(duration));
    }

    private void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        entityHealth.ReduceHealth(damage);
    }

    private void StopElectrifyEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;

        entityVfx.StopAllVfx();
    }

    private IEnumerator ElectrifyBlinkEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        entityVfx.PlayOnStatusBlinkVfx(duration, ElementType.Lightning);

        yield return new WaitForSeconds(duration);

        StopElectrifyEffect();
    }

    #endregion

    #region Burn DoT Effect

    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistance = entityStats.GetElementalResistance(ElementType.Fire);
        // Fire resistance reduces damage dealt by burn effect.
        float finalDamage = fireDamage * (1 - fireResistance);

        StartCoroutine(BurnEffectCo(duration, finalDamage));
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        entityVfx.PlayOnStatusBlinkVfx(duration, ElementType.Fire);

        int ticksPerSecond = 2;
        int tickCount = Mathf.RoundToInt(ticksPerSecond * duration);

        float damagePerTick = totalDamage / tickCount;
        // IMPORTANT: We must perform the division using a decimal value,
        // otherwise the Int rounding will cause the DoT damage to be way off.
        float tickInterval = 1f / ticksPerSecond;

        for (int i = 0; i < tickCount; i++)
        {
            // Reduce health of entity.
            // NOTE: This currently does not trigger the white flash effect.
            entityHealth.ReduceHealth(damagePerTick);
            // Pause briefly before applying next DoT (Damage over Time) tick.
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    #endregion

    #region Chill Slow Effect

    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        // Ice resistance reduces the duration of chill slow effects.
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChillEffectCo(finalDuration, slowMultiplier));
        Debug.Log($"{gameObject.name} -> Chill effect applied! Duration = {finalDuration}, Multiplier = {slowMultiplier}");
    }

    private IEnumerator ChillEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusBlinkVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

    #endregion

    public bool CanBeApplied(ElementType element)
    {
        if (ElementType.Lightning == element && currentEffect == ElementType.Lightning)
        {
            // This will allow us to electrify an enemy even if it is
            // already electrified.
            return true;
        }

        // Status effects can be applied if the entity does not currently
        // have a status effect applied.
        return currentEffect == ElementType.None;
    }
}
