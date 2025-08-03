using System;
using UnityEngine;

/// <summary>
/// Manages health, damage reception, and death mechanics for game entities.
/// 
/// This component provides a comprehensive health system that handles damage
/// reception, knockback effects, visual feedback, and death processing. It
/// implements the IDamageable interface to integrate with the combat system
/// and provides a foundation for entity survivability mechanics.
/// 
/// Key features:
/// - Health point management with damage reduction
/// - Directional knockback based on damage source
/// - Heavy damage detection with enhanced effects
/// - Visual effect integration for damage feedback
/// - Death state management and entity cleanup
/// - Configurable knockback parameters for game feel tuning
/// 
/// The system supports both light and heavy damage types, with heavy damage
/// triggering enhanced knockback effects when damage exceeds a configurable
/// threshold percentage of current health.
/// 
/// For more information about game health systems and damage mechanics:
/// https://docs.unity3d.com/Manual/index.html
/// https://gamedev.stackexchange.com/questions/tagged/health-system
/// </summary>
public class Entity_Health : MonoBehaviour, IDamageable
{
    public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    [SerializeField] private UI_MiniHealthBar healthBar;

    /// <summary>
    /// The entity's current health points.
    /// 
    /// This value represents the entity's remaining life force. When it reaches
    /// zero or below, the entity will die. The value is serialized to allow
    /// configuration in the Unity Inspector and can be modified at runtime
    /// for dynamic health changes like healing or poison effects.
    /// </summary>
    [SerializeField] protected float currentHealth;

    /// <summary>
    /// Health regeneration rate frequency, in seconds. Typically every 1 second.
    /// </summary>
    [Header("Health Regen")]
    [Tooltip("Health regeneration rate frequency, in seconds. Typically every 1 second.")]
    [SerializeField] private float regenInterval = 1f;
    [SerializeField] private bool canRegenerateHealth = true;

    public float lastDamageTaken { get; private set; }

    /// <summary>
    /// Flag indicating whether this entity has died.
    /// 
    /// Once set to true, this prevents the entity from taking additional damage
    /// and can be used by other systems to check death state. The flag is
    /// automatically set when health reaches zero and should not be manually
    /// modified unless implementing resurrection mechanics.
    /// </summary>
    public bool isDead { get; private set; }
    protected bool canTakeDamage = true;

    /// <summary>
    /// The knockback force applied when the entity receives normal damage.
    /// 
    /// This Vector2 defines the knockback velocity applied to the entity's
    /// Rigidbody2D when damaged. The X component determines horizontal force
    /// (automatically adjusted based on damage direction), and the Y component
    /// provides vertical lift. Values should be tuned for satisfying game feel.
    /// 
    /// Typical values:
    /// - X: 1-5 for subtle to strong horizontal knockback
    /// - Y: 1-3 for slight to significant vertical lift
    /// </summary>
    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 onDamageKnockback = new(2.5f, 2f);

    /// <summary>
    /// Duration in seconds that normal knockback effects last.
    /// 
    /// This controls how long the entity remains in knockback state, during
    /// which it may be unable to act or have reduced control. Shorter durations
    /// maintain responsive gameplay, while longer durations emphasize the
    /// impact of damage reception.
    /// </summary>
    [SerializeField] private float knockbackDuration = 0.2f;

    [Header("Heavy Damage Knockback")]

    /// <summary>
    /// The percentage threshold that determines when damage is considered "heavy".
    /// 
    /// When a single damage instance reduces current health by this percentage
    /// or more, it triggers enhanced knockback effects. This creates dynamic
    /// feedback where large attacks feel more impactful than small ones.
    /// 
    /// Example: With threshold 0.3 (30%) and 100 HP, damage ≥30 triggers heavy effects.
    /// </summary>
    [Range(0, 1)]
    [Tooltip("Percentage of HP you should lose to get a heavy knockback effect")]
    [SerializeField] private float heavyDamageThreshold = 0.3f;

    /// <summary>
    /// Duration in seconds that heavy knockback effects last.
    /// 
    /// Heavy damage knockback typically lasts longer than normal knockback to
    /// emphasize the significance of large damage amounts. This creates a
    /// risk/reward dynamic where powerful attacks have greater impact.
    /// </summary>
    [SerializeField] private float heavyKnockbackDuration = 0.5f;

    /// <summary>
    /// The enhanced knockback force applied when receiving heavy damage.
    /// 
    /// This Vector2 defines stronger knockback effects for significant damage.
    /// Values should be noticeably larger than normal knockback to provide
    /// clear visual and mechanical feedback for heavy hits.
    /// </summary>
    [SerializeField] private Vector2 onHeavyDamageKnockback = new(6f, 4f);

    /// <summary>
    /// Reference to the Entity component for accessing entity-specific functionality.
    /// 
    /// This reference allows the health system to trigger entity-specific
    /// behaviors like death animations, state changes, and cleanup procedures.
    /// It's automatically acquired during Awake() and cached for performance.
    /// </summary>
    private Entity entity;

    /// <summary>
    /// Reference to the Entity_VFX component for visual damage feedback.
    /// 
    /// This optional component handles visual effects like damage flashing,
    /// screen shake, or particle effects when the entity takes damage. The
    /// null-conditional operator ensures the system works even if VFX
    /// components are not present.
    /// </summary>
    private Entity_VFX entityVfx;

    private Entity_Stats entityStats;

    /// <summary>Handles item drops upon death in <see cref="Die"/>.</summary>
    private Entity_DropManager dropManager;

    /// <summary>
    /// Initializes component references during the Awake lifecycle phase.
    /// 
    /// This method automatically locates and caches references to related
    /// components on the same GameObject. It's marked virtual to allow
    /// subclasses to override and add additional initialization logic while
    /// maintaining the base functionality.
    /// </summary>
    protected virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityVfx = GetComponent<Entity_VFX>();
        entityStats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();
    }

    protected virtual void Start()
    {
        SetupHealth();
    }

    private void SetupHealth()
    {
        // Time Echo mirror clones currently do not have "Stats".
        if (entityStats == null) return;

        currentHealth = entityStats.GetMaxHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();

        // Note: This approach will only work if the entity is created with a starting
        // health regeneration value. If health regeneration was applied as a buff, this
        // solution would need to be revised.
        if (regenInterval > 0)
        {
            InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
        }
    }

    /// <summary>
    /// Processes damage reception with knockback, visual effects, and health reduction.
    /// 
    /// This method implements the IDamageable interface and serves as the main
    /// entry point for damage processing. It coordinates multiple systems:
    /// 
    /// 1. Validates that the entity is not already dead
    /// 2. Calculates knockback force and duration based on damage amount
    /// 3. Triggers visual effects if available
    /// 4. Applies knockback to the entity
    /// 5. Reduces health and potentially triggers death
    /// 
    /// The method is virtual to allow subclasses (like Enemy_Health) to add
    /// additional logic while maintaining core functionality.
    /// </summary>
    /// <param name="damage">
    /// The amount of damage to apply to this entity's health.
    /// </param>
    /// <param name="damageDealer">
    /// The Transform of the entity dealing damage, used for calculating
    /// knockback direction and enabling subclass-specific logic like
    /// enemy aggro systems or player interaction tracking.
    /// </param>
    public virtual bool TakeDamage(float damage, float elementalDamage, ElementType element, Transform damageDealer)
    {
        if (isDead) return false;

        if (!canTakeDamage) return false;

        if (AttackEvaded())
        {
            Debug.Log($"{gameObject.name} evaded the attack!");
            return false;
        }

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorPenetration() : 0;

        float mitigation = entityStats != null ? entityStats.GetArmorMitigation(armorReduction) : 0f;
        // 85% mitigation -> Receive 15% of damage
        float physicalDamageTaken = damage * (1 - mitigation);

        float resistance = entityStats != null ? entityStats.GetElementalResistance(element) : 0f;
        float elementalDamageTaken = elementalDamage * (1 - resistance);

        // Play "Damaged" visual effect whenever HP is reduced.
        // This effect is being played too often when working with DoT
        // status effects.
        if (entityVfx != null) entityVfx.PlayOnDamageFlashWhiteVfx();

        HandleKnockback(physicalDamageTaken, damageDealer);
        ReduceHealth(physicalDamageTaken + elementalDamageTaken);

        lastDamageTaken = physicalDamageTaken + elementalDamageTaken;

        Debug.LogFormat(
            "{0} -> Physical damage taken: {1} (Mitigation: {2}) - Elemental damage taken: {3} (Element: {4}, Resist: {5})",
            gameObject.name,
            physicalDamageTaken,
            mitigation,
            elementalDamageTaken,
            element,
            resistance
        );

        OnTakingDamage?.Invoke();
        return true;
    }

    /// <summary>
    /// Sets the entity invulnerable status, which affects whether the TakeDamage()
    /// function will apply damage and status effects to the entity.
    /// </summary>
    public void SetCanTakeDamage(bool canTakeDamage) => this.canTakeDamage = canTakeDamage;

    private bool AttackEvaded()
    {
        if (entityStats == null) return false;

        return UnityEngine.Random.Range(0, 100) < entityStats.GetEvasion();
    }

    private void RegenerateHealth()
    {
        if (!canRegenerateHealth || entityStats == null) return;

        float regenAmount = entityStats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);
    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead || entityStats == null) return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = entityStats.GetMaxHealth();

        // Disallow "overheal" (regenerating higher health than the maximum).
        currentHealth = Mathf.Min(newHealth, maxHealth);

        OnHealthUpdate?.Invoke();
    }

    /// <summary>
    /// Reduces the entity's health by the specified damage amount.
    /// 
    /// This method handles the core health reduction logic and automatically
    /// triggers death when health reaches zero or below. It's marked protected
    /// to allow subclasses to access it for custom damage processing while
    /// preventing external systems from bypassing the damage pipeline.
    /// </summary>
    /// <param name="damage">
    /// The amount of health points to subtract from current health.
    /// </param>
    public void ReduceHealth(float damage)
    {
        currentHealth -= damage;

        OnHealthUpdate?.Invoke();

        if (currentHealth <= 0) Die();
    }

    /// <summary>
    /// Processes entity death by setting the death flag and triggering cleanup.
    /// 
    /// This method handles the transition from alive to dead state, ensuring
    /// that the entity stops taking damage and initiates its death sequence.
    /// The Entity.EntityDeath() call allows for entity-specific death behaviors
    /// like animation triggers, loot drops, or state machine changes.
    /// </summary>
    protected virtual void Die()
    {
        isDead = true;
        entity.EntityDeath();
        dropManager?.DropItems();
    }

    public float GetHealthPercent()
    {
        if (entityStats == null) return 0f;

        return currentHealth / entityStats.GetMaxHealth();
    }

    public void SetHealthToPercent(float percent)
    {
        if (entityStats == null) return;

        currentHealth = entityStats.GetMaxHealth() * Mathf.Clamp01(percent);
        OnHealthUpdate?.Invoke();
    }

    public float GetCurrentHealth() => currentHealth;

    private void UpdateHealthBar()
    {
        if (healthBar == null || !healthBar.gameObject.activeSelf) return;

        if (healthBar.slider == null)
        {
            Debug.LogWarning($"{gameObject.name}.Entity_Health -> healthBar.slider is null due to Component Lifecycle Initialization Sequence!");
            return;
        }

        healthBar.slider.value = currentHealth / entityStats.GetMaxHealth();
    }

    public void EnableHealthBar(bool enable) => healthBar.gameObject.SetActive(enable);

    #region Knockback

    private void HandleKnockback(float physicalDamageTaken, Transform damageDealer)
    {
        float duration = CalculateDuration(physicalDamageTaken);
        Vector2 knockback = CalculateKnockback(physicalDamageTaken, damageDealer);

        if (entity != null) entity.ReceiveKnockback(knockback, duration);
    }

    /// <summary>
    /// Calculates directional knockback based on damage source position.
    /// 
    /// This method determines the knockback direction by comparing the positions
    /// of the damaged entity and the damage dealer. The horizontal component
    /// is automatically adjusted to push the entity away from the damage source,
    /// creating intuitive knockback behavior.
    /// 
    /// The calculation also considers whether the damage qualifies as "heavy"
    /// and applies the appropriate knockback magnitude accordingly.
    /// </summary>
    /// <param name="damage">
    /// The amount of damage dealt, used for heavy damage detection.
    /// </param>
    /// <param name="damageDealer">
    /// The Transform of the damage source, used for direction calculation.
    /// </param>
    /// <returns>
    /// A Vector2 representing the knockback force to apply to the entity.
    /// </returns>
    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        // See also: Enemy_BattleState.DirectionToPlayer()
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? onHeavyDamageKnockback : onDamageKnockback;
        knockback.x *= direction;

        return knockback;
    }

    /// <summary>
    /// Determines the appropriate knockback duration based on damage severity.
    /// 
    /// This method selects between normal and heavy knockback durations
    /// depending on whether the damage amount exceeds the heavy damage
    /// threshold. This creates escalating feedback for more significant hits.
    /// </summary>
    /// <param name="damage">
    /// The amount of damage dealt, used for heavy damage detection.
    /// </param>
    /// <returns>
    /// The duration in seconds that knockback effects should last.
    /// </returns>
    private float CalculateDuration(float damage)
    {
        return IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
    }

    #endregion

    /// <summary>
    /// Determines whether the damage amount qualifies as "heavy damage".
    /// 
    /// This method calculates the percentage of current health that the damage
    /// represents and compares it to the configured threshold. This percentage-
    /// based approach ensures that heavy damage detection scales appropriately
    /// with the entity's current health state.
    /// 
    /// For example, with a 30% threshold:
    /// - 100 HP entity: 30+ damage triggers heavy effects
    /// - 50 HP entity: 15+ damage triggers heavy effects
    /// - 10 HP entity: 3+ damage triggers heavy effects
    /// </summary>
    /// <param name="damage">
    /// The amount of damage to evaluate against the heavy damage threshold.
    /// </param>
    /// <returns>
    /// True if the damage exceeds the heavy damage threshold percentage,
    /// false otherwise.
    /// </returns>
    private bool IsHeavyDamage(float damage)
    {
        if (entityStats == null) return false;

        return damage / entityStats.GetMaxHealth() > heavyDamageThreshold;
    }
}
