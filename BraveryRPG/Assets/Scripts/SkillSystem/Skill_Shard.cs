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

    // [Header("Teleport Shard Upgrade")]
    // [SerializeField] private float shardExistDuration = 10;

    // [Header("Health Rewind Shard Upgrade")]
    // [SerializeField] private float savedHealthPercent;

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

        // if (Unlocked(SkillUpgradeType.Shard_Teleport))
        // {
        //     HandleShardTeleport();
        // }

        // if (Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        // {
        //     HandleShardHealthRewind();
        // }
    }

    // private void HandleShardHealthRewind()
    // {
    //     if (currentShard == null)
    //     {
    //         CreateShard();
    //         savedHealthPercent = playerHealth.GetHealthPercent();
    //     }
    //     else
    //     {
    //         SwapPlayerAndShard();
    //         playerHealth.SetHealthToPercent(savedHealthPercent);
    //         SetSkillOnCooldown();
    //     }
    // }

    // private void HandleShardTeleport()
    // {
    //     if (currentShard == null)
    //     {
    //         CreateShard();
    //     }
    //     else
    //     {
    //         SwapPlayerAndShard();
    //         SetSkillOnCooldown();
    //     }
    // }

    // private void SwapPlayerAndShard()
    // {
    //     Vector3 shardPosition = currentShard.transform.position;
    //     Vector3 playerPosition = player.transform.position;

    //     currentShard.transform.position = playerPosition;
    //     currentShard.Explode();

    //     player.TeleportPlayer(shardPosition);
    // }

    private void HandleShardMulticast()
    {
        if (currentCharges <= 0) return;

        CreateShard();
        currentShard.MoveTowardsClosestTarget(shardSpeed);
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
        currentShard.MoveTowardsClosestTarget(shardSpeed);

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

        // if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        // {
        //     currentShard.OnExplode += ForceCooldown;
        // }
    }

    public void CreateRawShard()
    {
        bool canMove = Unlocked(SkillUpgradeType.Shard_MoveToEnemy) || Unlocked(SkillUpgradeType.Shard_Multicast);

        // GameObject shard = Instantiate(shardPrefab, transform.position, Quaternion.identity);
        // shard.GetComponent<SkillObject_Shard>().SetupShard(this, detonateTime, canMove, shardSpeed);
    }

    public float GetDetonateTime()
    {
        if (Unlocked(SkillUpgradeType.Shard_Teleport) || Unlocked(SkillUpgradeType.Shard_TeleportHpRewind))
        {
            // return shardExistDuration;
        }

        return detonateTime;
    }

    // private void ForceCooldown()
    // {
    //     if (!OnCooldown())
    //     {
    //         SetSkillOnCooldown();
    //         currentShard.OnExplode -= ForceCooldown;
    //     }
    // }
}
