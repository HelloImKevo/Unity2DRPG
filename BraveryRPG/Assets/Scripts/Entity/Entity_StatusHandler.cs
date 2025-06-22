using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats entityStats;
    private Entity_Health entityHealth;
    private Entity_VFX entityVfx;

    [SerializeField] private ElementType currentEffect = ElementType.None;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        entityStats = GetComponent<Entity_Stats>();
        entityHealth = GetComponent<Entity_Health>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    #region Burn DoT

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
            entityHealth.ReduceHp(damagePerTick);
            // Pause briefly before applying next DoT (Damage over Time) tick.
            yield return new WaitForSeconds(tickInterval);
        }

        currentEffect = ElementType.None;
    }

    #endregion

    #region Chill DoT

    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        float iceResistance = entityStats.GetElementalResistance(ElementType.Ice);
        // Ice resistance reduces the duration of chill slow effects.
        float finalDuration = duration * (1 - iceResistance);

        StartCoroutine(ChilledEffectCo(finalDuration, slowMultiplier));
        Debug.Log($"{gameObject.name} -> Chill effect applied! Duration = {finalDuration}, Multiplier = {slowMultiplier}");
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
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
        // Status effects can be applied if the entity does not currently
        // have a status effect applied.
        return currentEffect == ElementType.None;
    }
}
