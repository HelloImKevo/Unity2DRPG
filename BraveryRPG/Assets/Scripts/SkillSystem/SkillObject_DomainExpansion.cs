using UnityEngine;

public class SkillObject_DomainExpansion : SkillObject_Base
{
    private Skill_DomainExpansion domainManager;

    private float expandSpeed = 2f;
    private float duration;

    /// <summary>
    /// Percentage of slowing applied to all nearby enemies.
    /// Should be assigned a value between 0 - 1.
    /// </summary>
    private float slowDownPercent;

    private Vector3 targetScale;
    private bool isShrinking;

    public void SetupDomain(Skill_DomainExpansion domainManager)
    {
        this.domainManager = domainManager;

        duration = domainManager.GetDomainDuration();
        slowDownPercent = domainManager.GetSlowPercentage();
        expandSpeed = domainManager.expandSpeed;
        float maxSize = domainManager.maxDomainSize;

        // (1, 1, 1) will become (10, 10, 10)
        targetScale = Vector3.one * maxSize;
        Invoke(nameof(ShrinkDomain), duration);
    }

    private void Update()
    {
        HandleScaling();
    }

    private void HandleScaling()
    {
        // Difference between current size and target size.
        float sizeDiffrence = Mathf.Abs(transform.localScale.x - targetScale.x);
        bool shouldChangeScale = sizeDiffrence > 0.1f;

        if (shouldChangeScale)
        {
            // Continue growing in size, with a smooth Linear Interpolate transition,
            // until we reach the target size.
            transform.localScale = Vector3.Lerp(
                transform.localScale,
                targetScale,
                expandSpeed * Time.deltaTime
            );
        }

        if (isShrinking && sizeDiffrence < 0.1f)
        {
            TerminateDomain();
        }
    }

    private void TerminateDomain()
    {
        domainManager.ClearTargets();
        Destroy(gameObject);
    }

    private void ShrinkDomain()
    {
        // Shrink the Black Hole down to (0, 0, 0)
        targetScale = Vector3.zero;
        isShrinking = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Enemy>(out var enemy)) return;

        domainManager.AddTarget(enemy);
        enemy.SlowDownEntity(duration, slowDownPercent, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Enemy>(out var enemy)) return;

        enemy.StopSlowDown();
    }
}
