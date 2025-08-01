using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Die();
        }
    }

    protected override void Die()
    {
        base.Die();

        GameManager.instance.SetLastPlayerPosition(transform.position);
        player.UserInterface.OpenDeathScreenUI();
    }
}
