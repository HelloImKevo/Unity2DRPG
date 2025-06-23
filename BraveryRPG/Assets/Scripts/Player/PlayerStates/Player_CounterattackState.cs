using UnityEngine;

public class Player_CounterattackState : PlayerState
{
    private readonly Player_Combat combat;
    private bool counteredSomebody;

    public Player_CounterattackState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        combat = player.GetComponent<Player_Combat>();
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = combat.GetCounterattackRecoveryDuration();
        counteredSomebody = combat.CounterattackPerformed();

        anim.SetBool("counterattackPerformed", counteredSomebody);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(0, rb.linearVelocity.y);

        if (onAnimationEndedTrigger)
        {
            stateMachine.ChangeState(player.IdleState);
        }

        if (stateTimer <= 0 && !counteredSomebody)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }
}
