using System.Collections;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_Stats stats;
    private Entity_VFX entityVfx;
    private ElementType currentEffect = ElementType.None;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        stats = GetComponent<Entity_Stats>();
        entityVfx = GetComponent<Entity_VFX>();
    }

    public void ApplyChilledEffect(float duration, float slowMultiplier)
    {
        float iceResistance = stats.GetElementalResistance(ElementType.Ice);
        // Ice resistance reduces the duration of chill slow effects.
        float reducedDuration = duration * (1 - iceResistance);

        StartCoroutine(ChilledEffectCo(reducedDuration, slowMultiplier));
        Debug.Log($"{gameObject.name} -> Chill effect applied! Duration = {reducedDuration}, Multiplier = {slowMultiplier}");
    }

    private IEnumerator ChilledEffectCo(float duration, float slowMultiplier)
    {
        entity.SlowDownEntity(duration, slowMultiplier);
        currentEffect = ElementType.Ice;
        entityVfx.PlayOnStatusBlinkVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

    public bool CanBeApplied(ElementType element)
    {
        // Status effects can be applied if the entity does not currently
        // have a status effect applied.
        return currentEffect == ElementType.None;
    }
}
