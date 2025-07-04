using System.Collections;
using UnityEngine;

/// <summary>
/// Shard skill system managing explosive projectiles with various upgrade behaviors
/// including movement, teleportation, multicast charges, and health restoration.
/// </summary>
public class Skill_Shard : Skill_Base
{
    /// <summary>Reference to the currently active shard object.</summary>
    private SkillObject_Shard currentShard;

    /// <summary>Player's health component for health rewind functionality.</summary>
    private Entity_Health playerHealth;

    /// <summary>Prefab used to create shard instances.</summary>
    [Tooltip("The SkillObject_Shard prefab with animator.")]
    [SerializeField] private GameObject shardPrefab;

    /// <summary>Base detonation time in seconds for regular shards.</summary>
    [SerializeField] private float detonateTime = 2f;

    /// <summary>Movement speed for shards that track enemies.</summary>
    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 6f;

    /// <summary>Maximum number of shard charges available for multicast.</summary>
    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;

    /// <summary>Current number of available shard charges.</summary>
    [SerializeField] private int currentCharges = 3;

    /// <summary>Flag indicating if charges are currently recharging.</summary>
    [SerializeField] private bool isRecharging;

    /// <summary>Duration shards exist before auto-detonation for teleport variants.</summary>
    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f;

    /// <summary>Saved health percentage for restoration during teleport.</summary>
    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent;

    /// <summary>
    /// Initializes shard charges and gets player health component reference.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
    }

    /// <summary>
    /// Main skill activation method that determines which shard behavior to execute
    /// based on unlocked upgrade types.
    /// </summary>
    public override void TryUseSkill()
    {
        if (!CanUseSkill()) return;

        if (Unlocked(SkillUpgradeType.Shard))
        {
            HandleShardRegular();
        }

        if (Unlocked(SkillUpgradeType.Shard_MoveToEnemy))
        {
            HandleShardMoving();
        }

        if (Unlocked(SkillUpgradeType.Shard_Multicast))
        {
            HandleShardMulticast();
        }

        if (Unlocked(SkillUpgradeType.Shard_Teleport))
        {
            HandleShardTeleport();
        }

        if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            HandleShardHealthRewind();
        }
    }

    /// <summary>
    /// Handles health rewind teleport behavior. First cast saves health and creates
    /// shard, second cast teleports to shard and restores saved health percentage.
    /// </summary>
    private void HandleShardHealthRewind()
    {
        if (currentShard == null)
        {
            CreateShard();
            savedHealthPercent = playerHealth.GetHealthPercent();
        }
        else
        {
            SwapPlayerAndShard();
            playerHealth.SetHealthToPercent(savedHealthPercent);
            SetSkillOnCooldown();
        }
    }

    /// <summary>
    /// Handles basic teleport behavior. First cast creates shard, second cast
    /// teleports player to shard location and explodes the shard.
    /// </summary>
    private void HandleShardTeleport()
    {
        if (currentShard == null)
        {
            CreateShard();
        }
        else
        {
            SwapPlayerAndShard();
            SetSkillOnCooldown();
        }
    }

    private void SwapPlayerAndShard()
    {
        Vector3 shardPosition = currentShard.transform.position;
        Vector3 playerPosition = Player.transform.position;

        currentShard.transform.position = playerPosition;
        // Immediately explode shard to prevent subsequent teleports.
        currentShard.Explode();

        Player.TeleportPlayer(shardPosition);
    }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0) return;

        CreateShard();
        currentShard.StartMovingTowardsClosestTarget(shardSpeed);
        currentCharges--;

        if (!isRecharging)
        {
            StartCoroutine(ShardRechargeCo());
        }
    }

    private IEnumerator ShardRechargeCo()
    {
        isRecharging = true;

        while (currentCharges < maxCharges)
        {
            yield return new WaitForSeconds(cooldown);
            // Replenish charge stacks to player.
            currentCharges++;
        }

        isRecharging = false;
    }

    private void HandleShardMoving()
    {
        CreateShard();
        currentShard.StartMovingTowardsClosestTarget(shardSpeed);

        SetSkillOnCooldown();
    }

    private void HandleShardRegular()
    {
        CreateShard();
        SetSkillOnCooldown();
    }

    private void CreateShard()
    {
        float detonateTime = GetDetonateTime();

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        currentShard = shard.GetComponent<SkillObject_Shard>();
        currentShard.SetupShard(this);

        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            // Subscribe to event Action - whenever we observe the OnExplode event, force cooldown.
            currentShard.OnExplode += ForceCooldown;
        }
    }

    /// <summary>
    /// Used to create Shard instances by synergy skills, like Dash.
    /// </summary>
    public void CreateRawShard(Transform target = null, bool shardsCanMove = false)
    {
        bool canMove;
        if (shardsCanMove)
        {
            canMove = true;
        }
        else
        {
            canMove = Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_Multicast);
        }

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        // Use overload which allows for behavior customization of shard instances.
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed, target);
    }

    public float GetDetonateTime()
    {
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            return shardExistDuration;
        }

        return detonateTime;
    }

    private void ForceCooldown()
    {
        if (!OnCooldown())
        {
            SetSkillOnCooldown();
            // Unsubscribe this function from the action event.
            currentShard.OnExplode -= ForceCooldown;
        }
    }
}
