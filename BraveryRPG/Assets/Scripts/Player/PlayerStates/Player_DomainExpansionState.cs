using UnityEngine;

public class Player_DomainExpansionState : PlayerState
{
    /// <summary>
    /// Original position of the player, used to calculate how far the player will rise from starting point.
    /// </summary>
    private Vector2 originalPosition;
    private float originalGravity;
    private float maxDistanceToGoUp;

    private bool isLevitating;
    private bool createdDomain;

    public Player_DomainExpansionState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        originalPosition = player.transform.position;
        originalGravity = rb.gravityScale;
        maxDistanceToGoUp = GetAvailableRiseDistance();

        // Apply upward velocity movement.
        player.SetVelocity(0, player.riseSpeed);

        // Make the player invulnerable while the Ultimate Spell sequence is playing out.
        player.Health.SetCanTakeDamage(false);
    }

    public override void Update()
    {
        base.Update();

        bool shouldLevitate = Vector2.Distance(originalPosition, player.transform.position) >= maxDistanceToGoUp
            && !isLevitating;

        if (shouldLevitate)
        {
            Levitate();
        }

        if (isLevitating)
        {
            skillManager.DomainExpansion.DoSpellCasting();

            if (stateTimer <= 0)
            {
                isLevitating = false;
                rb.gravityScale = originalGravity;
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        createdDomain = false;
        player.Health.SetCanTakeDamage(true);
    }

    private void Levitate()
    {
        isLevitating = true;
        rb.linearVelocity = Vector2.zero;
        // We want the player to float, without falling back down.
        rb.gravityScale = 0;

        stateTimer = skillManager.DomainExpansion.GetDomainDuration();
        Debug.Log($"Player_DomainExpansionState - Levitating! duration = {stateTimer}");

        // Check whether the player has created the Black Hole / Gravity Well effect.
        if (!createdDomain)
        {
            createdDomain = true;
            skillManager.DomainExpansion.CreateDomain();
        }
    }

    private float GetAvailableRiseDistance()
    {
        // Check how far we can rise until the player hits a ceiling,
        // or a rigid body collider above the player's head.
        RaycastHit2D hit =
            Physics2D.Raycast(player.transform.position, Vector2.up, player.riseMaxDistance, player.whatIsGround);

        return hit.collider != null ? hit.distance - 1 : player.riseMaxDistance;
    }
}
