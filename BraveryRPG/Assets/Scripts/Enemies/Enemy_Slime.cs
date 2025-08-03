using UnityEngine;

public class Enemy_Slime : Enemy, ICounterable
{
    public bool CanBeCountered { get => canBeStunned; }
    // public Enemy_SlimeDeadState slimeDeadState { get; set; }

    [Header("Slime Specifics")]
    [SerializeField] private GameObject slimeToCreatePrefab;
    [SerializeField] private int amountOfSlimesToCreate = 2;

    [SerializeField] private bool hasRecoveryAnimation = true;

    protected override void Awake()
    {
        base.Awake();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");
        // slimeDeadState = new Enemy_SlimeDeadState(this, stateMachine, "idle");

        Anim.SetBool("hasStunRecovery", hasRecoveryAnimation);
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(IdleState);
    }

    public override void EntityDeath()
    {
        // stateMachine.ChangeState(slimeDeadState);
    }

    public void OnReceiveCounterattack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }

    public void CreateSlimeOnDeath()
    {
        if (slimeToCreatePrefab == null) return;

        for (int i = 0; i < amountOfSlimesToCreate; i++)
        {
            GameObject newSlime = Instantiate(slimeToCreatePrefab, transform.position, Quaternion.identity);
            Enemy_Slime slimeScript = newSlime.GetComponent<Enemy_Slime>();

            // slimeScript.Stats.AdjustStatSetup(Stats.resources, Stats.offense, Stats.defense, 0.6f, 1.2f);
            // slimeScript.ApplyRespawnVelocity();
            // slimeScript.StartBattleStateCheck(PlayerRef);
        }
    }

    public void ApplyRespawnVelocity()
    {
        Vector2 velocity = new(stunnedVelocity.x * Random.Range(-1f, 1f), stunnedVelocity.y * Random.Range(1f, 2f));
        SetVelocity(velocity.x, velocity.y);
    }

    // public void StartBattleStateCheck(Transform player)
    // {
    //     TryEnterBattleState(player);
    //     InvokeRepeating(nameof(ReEnterBattleState), 0, 0.3f);
    // }

    // private void ReEnterBattleState()
    // {
    //     if (stateMachine.CurrentState == BattleState || stateMachine.CurrentState == AttackState)
    //     {
    //         CancelInvoke(nameof(ReEnterBattleState));
    //         return;
    //     }

    //     stateMachine.ChangeState(BattleState);
    // }
}
