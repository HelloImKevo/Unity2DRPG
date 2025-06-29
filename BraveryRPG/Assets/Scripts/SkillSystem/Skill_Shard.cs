using System.Collections;
using UnityEngine;

public class Skill_Shard : Skill_Base
{
    private SkillObject_Shard currentShard;
    private Entity_Health playerHealth;

    [Tooltip("The SkillObject_Shard prefab with animator.")]
    [SerializeField] private GameObject shardPrefab;
    [SerializeField] private float detonateTime = 2f;

    [Header("Moving Shard Upgrade")]
    [SerializeField] private float shardSpeed = 6f;

    [Header("Multicast Shard Upgrade")]
    [SerializeField] private int maxCharges = 3;
    [SerializeField] private int currentCharges = 3;
    [SerializeField] private bool isRecharging;

    [Header("Teleport Shard Upgrade")]
    [SerializeField] private float shardExistDuration = 10f;

    [Header("Health Rewind Shard Upgrade")]
    [SerializeField] private float savedHealthPercent;

    protected override void Awake()
    {
        base.Awake();
        currentCharges = maxCharges;
        playerHealth = GetComponentInParent<Entity_Health>();
    }

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
    public void CreateRawShard()
    {
        bool canMove = Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_Multicast);

        GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        // Use overload which allows for behavior customization of shard instances.
        shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed);
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
