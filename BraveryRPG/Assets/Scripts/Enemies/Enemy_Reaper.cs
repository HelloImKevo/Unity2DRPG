using System.Collections;
using UnityEngine;

public class Enemy_Reaper : Enemy, ICounterable
{
    private Entity_VisionCone visionCone;

    public Enemy_ReaperAttackState reaperAttackState { get; private set; }
    public Enemy_ReaperBattleState reaperBattleState { get; private set; }
    public Enemy_ReaperTeleportState reaperTeleportState { get; private set; }
    public Enemy_ReaperSpellCastState reaperSpellCastState { get; private set; }

    [Header("Reaper Specifics")]
    public float maxBattleIdleTime = 5f;

    [Header("Reaper Spellcast")]
    [SerializeField] private DamageScaleData spellDamageScale;
    [SerializeField] private GameObject spellCastPrefab;
    [SerializeField] private int amountToCast = 6;
    [SerializeField] private float spellCastRate = 1.2f;
    [SerializeField] private float spellCastStateCooldown = 10f;
    [SerializeField] private Vector2 playerOffsetPrediction;
    private float lastTimeCastedSpells = float.NegativeInfinity;
    public bool spellCastPerformed { get; private set; }
    private Player playerScript;

    [Header("Reaper Teleport")]
    [SerializeField] private BoxCollider2D arenaBounds;
    [SerializeField] private float offsetCenterY = 1.725f;
    [SerializeField] private float chanceToTeleport = 0.25f;
    private float defaultTeleportChance;
    public bool teleportTrigger { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        shouldLogStateTransitions = true;

        visionCone = GetComponent<Entity_VisionCone>();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        DeadState = new Enemy_DeadState(this, stateMachine, "idle"); // We aren't applying an animation on death.
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        reaperBattleState = new Enemy_ReaperBattleState(this, stateMachine, "battle");
        reaperAttackState = new Enemy_ReaperAttackState(this, stateMachine, "attack");
        reaperTeleportState = new Enemy_ReaperTeleportState(this, stateMachine, "teleport");
        reaperSpellCastState = new Enemy_ReaperSpellCastState(this, stateMachine, "spellCast");

        BattleState = reaperBattleState;
        AttackState = reaperAttackState;
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{gameObject.name} calling START");

        arenaBounds.transform.parent = null;
        defaultTeleportChance = chanceToTeleport;

        stateMachine.Initialize(IdleState);
    }

    public override void SpecialAttack()
    {
        StartCoroutine(CastSpellCo());
    }

    private IEnumerator CastSpellCo()
    {
        var player = GetPlayerReference();

        if (playerScript == null)
        {
            playerScript = player.GetComponent<Player>();
        }

        for (int i = 0; i < amountToCast; i++)
        {
            bool playerMoving = playerScript.Rb.linearVelocity.magnitude > 0;

            float xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.FacingDir : 0f;
            Vector3 spellPosition = player.transform.position + new Vector3(xOffset, playerOffsetPrediction.y);

            Enemy_ReaperSpell spell = Instantiate(
                spellCastPrefab,
                spellPosition,
                Quaternion.identity
            ).GetComponent<Enemy_ReaperSpell>();

            spell.SetupSpell(combat, spellDamageScale);

            yield return new WaitForSeconds(spellCastRate);
        }

        SetSpellCastPreformed(true);
    }

    public void SetSpellCastPreformed(bool spellCastStatus) => spellCastPerformed = spellCastStatus;

    public bool CanDoSpellCast() => Time.time > lastTimeCastedSpells + spellCastStateCooldown;

    public void SetSpellCastOnCooldown() => lastTimeCastedSpells = Time.time;

    public bool ShouldTeleport()
    {
        if (Random.value < chanceToTeleport)
        {
            chanceToTeleport = defaultTeleportChance;
            return true;
        }

        chanceToTeleport += 0.05f;
        return false;
    }

    public void SetTeleportTrigger(bool triggerStatus) => teleportTrigger = triggerStatus;

    public Vector3 FindTeleportPoint()
    {
        int maxAttampts = 10;
        float bossWithColliderHalf = Col.bounds.size.x / 2;

        for (int i = 0; i < maxAttampts; i++)
        {
            float randomX = Random.Range(
                arenaBounds.bounds.min.x + bossWithColliderHalf,
                arenaBounds.bounds.max.x - bossWithColliderHalf
            );

            Vector2 raycastPoint = new(randomX, arenaBounds.bounds.max.y);

            RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, whatIsGround);

            if (hit.collider != null)
            {
                return hit.point + new Vector2(0, offsetCenterY);
            }
        }

        return transform.position;
    }

    #region ICounterable

    public bool CanBeCountered { get => canBeStunned; }

    public void OnReceiveCounterattack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }

    #endregion
}
