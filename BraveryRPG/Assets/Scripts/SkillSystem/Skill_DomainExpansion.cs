using System.Collections.Generic;
using UnityEngine;

public class Skill_DomainExpansion : Skill_Base
{
    [SerializeField] private GameObject domainPrefab;

    [Header("Slowing Down Upgrade")]
    [Range(0f, 1f)]
    [Tooltip("Percentage of slowing applied to all nearby enemies.")]
    [SerializeField] private float slowDownPercent = 0.8f;
    [Tooltip("How many seconds this ultimate spell lasts.")]
    [SerializeField] private float slowDownDomainDuration = 5f;

    [Header("Shard Cast Upgrade")]
    [SerializeField] private int shardsToCast = 10;
    [SerializeField] private float shardCastDomainSlow = 0.9f;
    [Tooltip("How many seconds the Shard spellcasting effect will last, before the player falls back to the Ground.")]
    [SerializeField] private float shardCastDomainDuration = 8f;
    private float spellCastTimer;
    private float spellsPerSecond;

    [Header("Time Echo Cast Upgrade")]
    [SerializeField] private int echoToCast = 8;
    [SerializeField] private float echoCastDomainSlow = 1f;
    [SerializeField] private float echoCastDomainDuration = 6f;
    [SerializeField] private float healthToRestoreWithEcho = 0.05f;

    [Header("Domain Details")]
    public float maxDomainSize = 10;
    public float expandSpeed = 3;

    private List<Enemy> trappedTargets = new();
    private Transform currentTarget;

    public void CreateDomain()
    {
        spellsPerSecond = GetSpellsToCast() / GetDomainDuration();

        GameObject domain = Instantiate(domainPrefab, transform.position, Quaternion.identity);
        domain.GetComponent<SkillObject_DomainExpansion>().SetupDomain(this);
    }

    public void DoSpellCasting()
    {
        spellCastTimer -= Time.deltaTime;

        if (currentTarget == null)
        {
            currentTarget = FindTargetInDomain();
        }

        if (currentTarget != null && spellCastTimer < 0)
        {
            CastSpell(currentTarget);
            spellCastTimer = 1 / spellsPerSecond;
            currentTarget = null;
        }
    }

    private void CastSpell(Transform target)
    {
        if (SkillUpgradeType.Domain_EchoSpam == upgradeType)
        {
            // 50% chance to spawn the Echo on the left-side or right-side of enemy.
            Vector3 offset = Random.value < 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);
            SkillManager.TimeEcho.CreateTimeEcho(target.position + offset);
        }

        if (SkillUpgradeType.Domain_ShardSpam == upgradeType)
        {
            SkillManager.Shard.CreateRawShard(target, true);
        }
    }

    /// <summary>
    /// Pick a random target from the enemies trapped in the Black Hole.
    /// </summary>
    /// <returns></returns>
    private Transform FindTargetInDomain()
    {
        // trappedTargets.RemoveAll(target => target == null || target.Health.isDead);

        if (trappedTargets.Count == 0) return null;

        int randomIndex = Random.Range(0, trappedTargets.Count);
        return trappedTargets[randomIndex].transform;
    }

    public float GetDomainDuration()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
        {
            return slowDownDomainDuration;
        }
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            return shardCastDomainDuration;
        }
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            return echoCastDomainDuration;
        }

        return 0;
    }

    public float GetSlowPercentage()
    {
        if (upgradeType == SkillUpgradeType.Domain_SlowingDown)
        {
            return slowDownPercent;
        }
        else if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            return shardCastDomainSlow;
        }
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            return echoCastDomainSlow;
        }

        return 0;
    }

    private int GetSpellsToCast()
    {
        if (upgradeType == SkillUpgradeType.Domain_ShardSpam)
        {
            return shardsToCast;
        }
        else if (upgradeType == SkillUpgradeType.Domain_EchoSpam)
        {
            return echoToCast;
        }

        return 0;
    }

    public bool IsInstantDomain()
    {
        return upgradeType != SkillUpgradeType.Domain_EchoSpam
            && upgradeType != SkillUpgradeType.Domain_ShardSpam;
    }

    public void AddTarget(Enemy targetToAdd)
    {
        trappedTargets.Add(targetToAdd);
    }

    public void ClearTargets()
    {
        foreach (var enemy in trappedTargets)
        {
            enemy.StopSlowDown();
        }

        trappedTargets = new List<Enemy>();
    }
}
