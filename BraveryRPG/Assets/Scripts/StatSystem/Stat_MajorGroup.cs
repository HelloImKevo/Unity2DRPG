using System;
using UnityEngine;

[Serializable]
public class Stat_MajorGroup
{
    public Stat strength;
    [Tooltip("Each point gives 0.5% chance to dodge a physical attack")]
    public Stat agility;
    public Stat intelligence;
    [Tooltip("Each point increases max HP +5")]
    public Stat vitality;
}
