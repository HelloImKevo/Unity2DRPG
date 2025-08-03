using UnityEngine;

public class Enemy_DeadState : EnemyState
{
    private readonly Collider2D collider;

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        collider = enemy.GetComponent<Collider2D>();
    }

    public override void Enter()
    {
        // Stop animations - freeze enemy on the current frame.
        anim.enabled = false;

        // Disable Physics2D rigidbody collider.
        collider.enabled = false;

        // Make the enemy fall through the bottom of the screen.
        rb.gravityScale = 12;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 15);

        stateMachine.SwitchOffStateMachine();
        enemy.DestroyGameObjectWithDelay();
    }
}
