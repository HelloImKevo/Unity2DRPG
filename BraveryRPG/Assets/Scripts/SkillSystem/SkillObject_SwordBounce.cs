using System.Collections.Generic;
using UnityEngine;

public class SkillObject_SwordBounce : SkillObject_Sword
{
    private float bounceSpeed = 1f;
    private int bounceCount;

    private Collider2D[] enemyTargets;
    private Transform nextTarget;

    /// <summary>
    /// Keep track of which enemies have already been previously targeted and attacked
    /// by the Bouncing Sword. Once all nearby enemies have been attacked, this can be
    /// cleared, and those enemies can be retargeted.
    /// </summary>
    private List<Transform> selectedBefore = new();

    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        anim.SetTrigger("spin");
        base.SetupSword(swordManager, direction);

        bounceSpeed = swordManager.bounceSpeed;
        bounceCount = swordManager.bounceCount;
    }

    protected override void Update()
    {
        HandleComeback();
        HandleBounce();
    }

    private void HandleBounce()
    {
        if (nextTarget == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            nextTarget.position,
            bounceSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, nextTarget.position) < 0.75f)
        {
            DamageEnemiesInRadius(transform, damageRadius);
            BounceToNextTarget();

            if (bounceCount == 0 || nextTarget == null)
            {
                nextTarget = null;
                EnableSwordFlyBackToPlayer();
            }
        }
    }

    private void BounceToNextTarget()
    {
        nextTarget = GetNextTarget();
        bounceCount--;
        // Debug.Log("I did bounce at : " + nextTarget.gameObject.name + " : frame is - " + Time.frameCount);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemyTargets == null)
        {
            enemyTargets = EnemiesAround(transform, detectionRadius);
            // When you turn off simulations, it stops collision detection, gravity, etc.
            rb.simulated = false;
        }

        DamageEnemiesInRadius(transform, damageRadius);

        if (enemyTargets.Length <= 1 || bounceCount == 0)
        {
            EnableSwordFlyBackToPlayer();
        }
        else
        {
            nextTarget = GetNextTarget();
        }
    }

    private Transform GetNextTarget()
    {
        List<Transform> validTarget = GetValidTargets();

        int randomIndex = Random.Range(0, validTarget.Count);

        Transform nextTarget = validTarget[randomIndex];
        selectedBefore.Add(nextTarget);

        return nextTarget;
    }

    private List<Transform> GetValidTargets()
    {
        List<Transform> validTargets = new();
        List<Transform> aliveTargets = GetAliveTargets();

        foreach (var enemy in aliveTargets)
        {
            if (enemy != null && !selectedBefore.Contains(enemy.transform))
            {
                validTargets.Add(enemy.transform);
            }
        }

        if (validTargets.Count > 0)
        {
            return validTargets;
        }
        else
        {
            // We've cycled through all nearby enemies; clear the cache, and
            // allow the enemies to be retargeted.
            selectedBefore.Clear();
            return aliveTargets;
        }
    }

    private List<Transform> GetAliveTargets()
    {
        List<Transform> aliveTargets = new();

        // IMPORTANT: Enemies may die while they are being damaged and while this
        // list is traversed, which will make the enemy game object references null.
        foreach (var enemy in enemyTargets)
        {
            if (enemy != null)
            {
                aliveTargets.Add(enemy.transform);
            }
        }

        return aliveTargets;
    }
}
