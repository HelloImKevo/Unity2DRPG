using System;

/// <summary>
/// Represents a temporary stat modification that can be applied to an entity.
/// </summary>
[Serializable]
public class BuffEffectData
{
    // TODO: Introduce a Name property and possibly a description.
    public StatType type;
    public float value;
}
