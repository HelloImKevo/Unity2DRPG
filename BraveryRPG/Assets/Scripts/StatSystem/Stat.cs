using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<StatModifier> modifiers = new();

    // Always perform initial calculation once, to establish initial value.
    private bool needsCalculation = true;
    private float finalValue;

    public float GetValue()
    {
        if (needsCalculation)
        {
            // Recalculate (which performs List traversal).
            finalValue = CalculateFinalValue();
            needsCalculation = false;
        }

        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        StatModifier modToAdd = new(value, source);
        modifiers.Add(modToAdd);
        needsCalculation = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        needsCalculation = true;
    }

    private float CalculateFinalValue()
    {
        float finalValue = baseValue;

        foreach (StatModifier modifier in modifiers)
        {
            finalValue += modifier.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;
}

[Serializable]
public class StatModifier
{
    public float value;
    public string source;

    public StatModifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
