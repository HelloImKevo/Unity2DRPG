using UnityEngine;

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
    Shard_TeleportHpRewind,

    // ------ Throw Sword Tree -------
    [Tooltip("You can throw sword to damage enemies from range.")]
    SwordThrow,

    [Tooltip("Your sword will spin at one point and damage enemies. Like a chainsaw.")]
    SwordThrow_Spin,

    [Tooltip("Sword will pierce N targets.")]
    SwordThrow_Pierce,

    [Tooltip("Sword will bounce between enemies.")]
    SwordThrow_Bounce,

    // ------ Time Echo -------
    [Tooltip("Create a clone of a player. It can take damage from enemies.")]
    TimeEcho,

    [Tooltip("Time Echo can perform a single attack.")]
    TimeEcho_SingleAttack,

    [Tooltip("Time Echo can perform N attacks.")]
    TimeEcho_MultiAttack,

    [Tooltip("Time Echo has a chance to create another time echo when attacks.")]
    TimeEcho_ChanceToDuplicate,

    [Tooltip("When time echo dies it creates a wips that flies towards the player to heal it. Heal is = to percantage of damage taken when died.")]
    TimeEcho_HealWisp,

    [Tooltip("Wisp will now remove negative effects from player.")]
    TimeEcho_CleanseWisp,

    [Tooltip("Wisp will reduce cooldown of all skills by N second. ")]
    TimeEcho_CooldownWisp,
}
