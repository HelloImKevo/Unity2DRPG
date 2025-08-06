using System.Collections;
using UnityEngine;

public class Enemy_Mage : Enemy, ICounterable
{
    public Enemy_MageRetreatState mageRetreatState { get; private set; }
    public Enemy_MageBattleState mageBattleState { get; private set; }
    public Enemy_MageSpellCastState mageSpellCastState { get; private set; }

    // MAGE SPECIFICS -------------------------------------------------------------------

    [Header("Mage Specifics")]
    [SerializeField] private GameObject spellPrefab;
    [SerializeField] private Transform spellStartPosition;
    [SerializeField] private int amountToCast = 3;
    [SerializeField] private float spellCastCooldown = 0.3f;
    public bool spellCastPerformed { get; private set; }

    [Space]
    public float retreatCooldown = 5f;
    public float retreatMaxDistance = 6f;
    public float retreatSpeed = 15f;

    [SerializeField] private Transform behindCollsionCheck;
    [SerializeField] private bool hasRecoveryAnimation = true;

    protected override void Awake()
    {
        base.Awake();

        shouldLogStateTransitions = true;

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        DeadState = new Enemy_DeadState(this, stateMachine, "idle");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        mageSpellCastState = new Enemy_MageSpellCastState(this, stateMachine, "spellCast");
        mageRetreatState = new Enemy_MageRetreatState(this, stateMachine, "battle");
        mageBattleState = new Enemy_MageBattleState(this, stateMachine, "battle");
        BattleState = mageBattleState;

        Anim.SetBool("hasStunRecovery", hasRecoveryAnimation);
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{gameObject.name} calling START");

        stateMachine.Initialize(IdleState);
    }

    public void SetSpellCastPerformed(bool performed) => spellCastPerformed = performed;

    public override void SpecialAttack()
    {
        // StartCoroutine(CastSpellCo());
    }

    // private IEnumerator CastSpellCo()
    // {
    //     for (int i = 0; i < amountToCast; i++)
    //     {
    //         Enemy_MageProjectile projectile
    //             = Instantiate(spellPrefab, spellStartPosition.position, Quaternion.identity).GetComponent<Enemy_MageProjectile>();

    //         projectile.SetupProjectile(GetPlayerReference().transform, combat);
    //         yield return new WaitForSeconds(spellCastCooldown);
    //     }

    //     SetSpellCastPerformed(true);
    // }

    // public bool CantMoveBackwards()
    // {
    //     bool detectedWall = Physics2D.Raycast(
    //         behindCollsionCheck.position,
    //         Vector2.right * -FacingDir,
    //         1.5f,
    //         whatIsGround
    //     );
    //     bool noGround = !Physics2D.Raycast(
    //         behindCollsionCheck.position,
    //         Vector2.down,
    //         1.5f,
    //         whatIsGround
    //     );

    //     return noGround || detectedWall;
    // }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawLine(
            behindCollsionCheck.position,
            new Vector3(
                behindCollsionCheck.position.x + (1.5f * -FacingDir),
                behindCollsionCheck.position.y
            )
        );

        Gizmos.DrawLine(
            behindCollsionCheck.position,
            new Vector3(
                behindCollsionCheck.position.x,
                behindCollsionCheck.position.y - 1.5f
            )
        );
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
