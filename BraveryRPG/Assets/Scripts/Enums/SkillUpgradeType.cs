public enum SkillUpgradeType
{
    None,

    // ------ Dash Tree -------
    /// <summary>
    /// Dash to avoid damage
    /// </summary>
    Dash,

    /// <summary>
    /// Create a clone when dash starts
    /// </summary>
    Dash_CloneOnStart,

    /// <summary>
    /// Create a clone when dash starts and ends
    /// </summary>
    Dash_CloneOnStartAndArrival,

    /// <summary>
    /// Create a shard when dash starts
    /// </summary>
    Dash_ShardOnShart,

    /// <summary>
    /// Create a shard when dash starts and ends
    /// </summary>
    Dash_ShardOnStartAndArrival,

    // ------ Shard Tree -------
    /// <summary>
    /// The shard explodes when touched by an enemy or time goes up
    /// </summary>
    Shard,

    /// <summary>
    /// Shard will move towards nearest enemy
    /// </summary>
    Shard_MoveToEnemy,

    /// <summary>
    /// Shard ability can have up to N charges. You can cast them all in a row.
    /// </summary>
    Shard_Multicast,

    /// <summary>
    /// You can swap places with the last shard you created
    /// </summary>
    Shard_Teleport,

    /// <summary>
    /// When you swap places with shard, your HP % is same as it was when you created shard.
    /// </summary>
    Shard_TeleportHpRewind
}
