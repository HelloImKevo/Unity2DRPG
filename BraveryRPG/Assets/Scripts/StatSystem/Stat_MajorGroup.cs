using System;
using UnityEngine;

[Serializable]
public class Stat_MajorGroup
{
    public Stat strength;
    public Stat agility;
    public Stat intelligence;
    [Tooltip("Each point increases max HP +5")]
    public Stat vitality;
}
