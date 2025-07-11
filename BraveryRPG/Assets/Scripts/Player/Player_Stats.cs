using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Stats : Entity_Stats
{
    private List<string> activeBuff = new();
    private Inventory_Player inventory;

    protected override void Awake()
    {
        base.Awake();
        inventory = GetComponent<Inventory_Player>();
    }

    public bool CanApplyBuffOf(string source)
    {
        return !activeBuff.Contains(source);
    }

    /// <summary>
    /// Applies buff effects to the target entity's stats based on the apply parameter.
    /// </summary>
    public void ApplyBuff(BuffEffectData[] buffsToApply, float duration, string source)
    {
        StartCoroutine(BuffCo(buffsToApply, duration, source));
    }

    /// <summary>
    /// Coroutine that manages the buff lifecycle, including application, duration timing,
    /// and cleanup. See also: <see cref="Object_Buff"/>.
    /// </summary>
    /// <param name="duration">The duration in seconds for how long the buff should last.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator BuffCo(BuffEffectData[] buffsToApply, float duration, string source)
    {
        activeBuff.Add(source);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).AddModifier(buff.value, source);
        }

        Debug.Log($"Buff '{source}' is applied for {duration} seconds");

        yield return new WaitForSeconds(duration);

        foreach (var buff in buffsToApply)
        {
            GetStatByType(buff.type).RemoveModifier(source);
        }

        inventory.TriggerUpdateUI();
        activeBuff.Remove(source);
    }
}
