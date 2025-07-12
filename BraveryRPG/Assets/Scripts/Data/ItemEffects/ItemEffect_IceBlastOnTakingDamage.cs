using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Item Data/Item Effect/Ice Blast", fileName = "Item Effect Data - Ice Blast on Taking Damage")]
public class ItemEffect_IceBlastOnTakingDamage : ItemEffect_DataSO
{
    [SerializeField] private ElementalEffectData effectData;
    [SerializeField] private float iceDamage;

    /// <summary>Radius around the player where enemies take damage.</summary>
    [Tooltip("Radius around the player where enemies take damage.")]
    [SerializeField] protected float damageRadius = 1.5f;

    /// <summary>Layer mask defining what counts as an enemy for collision detection.</summary>
    [Tooltip("The 'Enemy' Layer")]
    [SerializeField] private LayerMask whatIsEnemy;

    [Space]
    [SerializeField] private float healthPercentTrigger = 0.25f;
    [SerializeField] private float cooldown;

    private float lastTimeUsed = -999;

    [Header("VFX Objects")]
    [SerializeField] private GameObject iceBlastVfx;
    [SerializeField] private GameObject onHitVfx;

    public override void ExecuteEffect()
    {
        bool noCooldown = Time.time >= lastTimeUsed + cooldown;
        bool reachedThreshold = player.Health.GetHealthPercent() <= healthPercentTrigger;

        if (noCooldown && reachedThreshold)
        {
            player.Vfx.CreateEffectOf(iceBlastVfx, player.transform);
            lastTimeUsed = Time.time;
            DamageEnemiesWithIce();
        }
    }

    private void DamageEnemiesWithIce()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            player.transform.position, damageRadius, whatIsEnemy
        );

        foreach (var target in enemies)
        {
            IDamageable damagable = target.GetComponent<IDamageable>();

            if (damagable == null) continue;

            bool targetGotHit = damagable.TakeDamage(
                0, iceDamage, ElementType.Ice, player.transform
            );

            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();
            statusHandler?.ApplyStatusEffect(ElementType.Ice, effectData);

            if (targetGotHit)
            {
                player.Vfx.CreateEffectOf(onHitVfx, target.transform);
            }
        }
    }

    public override void Subscribe(Player player)
    {
        base.Subscribe(player);
        // When we observe an event where the player Takes Damage, try to execute
        // the unique item effect (check whether it is on cooldown, and whether
        // other conditions are satisfied).
        player.Health.OnTakingDamage += ExecuteEffect;
    }

    public override void Unsubscribe()
    {
        base.Unsubscribe();
        player.Health.OnTakingDamage -= ExecuteEffect;
        player = null;
    }

    private void OnEnable()
    {
        lastTimeUsed = -999;
    }
}
