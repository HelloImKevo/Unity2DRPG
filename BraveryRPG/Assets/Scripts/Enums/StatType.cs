public enum StatType
{
    // Resource Groups
    MaxHealth,
    HealthRegen,

    // Major Stats
    Strength,
    Agility,
    Intelligence,
    Vitality,

    // Offense Stats
    AttackSpeed,
    Damage,
    CritChance,
    CritPower,
    ArmorPenetration,
    FireDamage,
    IceDamage,
    LightningDamage,

    // Defense Stats
    Armor,
    Evasion,
    IceResistance,
    FireResistance,
    LightningResistance,

    // If you don't add this to the end, it will offset ScriptableObject
    // data and you'll need to do a lot of game object refactoring.
    ElementalDamage
}
