using UnityEngine;

public class Enemy_ArcherElf : Enemy
{
    public bool CanBeCountered { get => canBeStunned; }
    public Enemy_ArcherElfBattleState elfBattleState { get; set; }

    [Header("Archer Elf Specifics")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowStartPoint;
    [SerializeField] private float arrowSpeed = 8;

    private Entity_VisionCone visionCone;

    protected override void Awake()
    {
        base.Awake();

        shouldLogStateTransitions = true;

        Debug.Log($"{gameObject.name} calling AWAKE");

        visionCone = GetComponent<Entity_VisionCone>();

        IdleState = new Enemy_IdleState(this, stateMachine, "idle");
        MoveState = new Enemy_MoveState(this, stateMachine, "move");
        AttackState = new Enemy_AttackState(this, stateMachine, "attack");
        BattleState = new Enemy_BattleState(this, stateMachine, "battle");
        DeadState = new Enemy_DeadState(this, stateMachine, "idle");
        StunnedState = new Enemy_StunnedState(this, stateMachine, "stunned");

        elfBattleState = new Enemy_ArcherElfBattleState(this, stateMachine, "battle");
        BattleState = elfBattleState;

        stateMachine.Initialize(IdleState);
    }

    protected override void Start()
    {
        base.Start();

        Debug.Log($"{gameObject.name} calling START");
    }

    public override void SpecialAttack()
    {
        GameObject newArrow = Instantiate(
            arrowPrefab,
            arrowStartPoint.position,
            Quaternion.identity
        );
        // Make the arrow fly in the direction the enemy is facing.
        newArrow.GetComponent<Enemy_ArcherElfArrow>().SetupArrow(arrowSpeed * FacingDir, combat);
    }

    public void OnReceiveCounterattack()
    {
        if (!CanBeCountered) return;

        stateMachine.ChangeState(StunnedState);
    }
}
