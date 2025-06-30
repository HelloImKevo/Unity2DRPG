using UnityEngine;

public class SkillObject_SwordPierce : SkillObject_Sword
{
    private int amountToPierce;

    public override void SetupSword(Skill_ThrowSword swordManager, Vector2 direction)
    {
        base.SetupSword(swordManager, direction);
        amountToPierce = swordManager.amountToPierce;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        // If two enemies are close to each other, the OnTriggerEnter could
        // be invoked twice, and our splash damage could get invoked twice
        // to deal damage twice. As an easy workaround, we just use a very
        // small radius. This can be fine-tuned later.
        float splashDamageRadius = 0.3f;
        bool groundHit = collision.gameObject.layer == LayerMask.NameToLayer("Ground");

        if (amountToPierce <= 0 || groundHit)
        {
            DamageEnemiesInRadius(transform, splashDamageRadius);
            StickSwordToCollider(collision);
            return;
        }

        amountToPierce--;
        Debug.Log($"{GetType().Name} -> OnTriggerEnter2D() -> Decrement amountToPierce: {amountToPierce}");

        DamageEnemiesInRadius(transform, splashDamageRadius);
    }
}
