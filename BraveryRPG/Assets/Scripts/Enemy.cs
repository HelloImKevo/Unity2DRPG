using UnityEngine;

public class Enemy : Entity
{
    public Enemy_IdleState IdleState { get; protected set; }
    public Enemy_MoveState MoveState { get; protected set; }

    [Header("Enemy Movement Details")]
    public float idleTime = 2f;
    public float moveSpeed = 1.4f;

    [Range(0, 2)]
    [Tooltip("Used to speed up or slow down animations for faster or slower enemies")]
    public float moveAnimSpeedMultiplier = 1;

    [SerializeField] private float redColorDuration = 1;

    public float timer;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        ChangeColorIfNeeded();
    }

    private void ChangeColorIfNeeded()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(0, timer);

        // if (timer <= 0 && sr.color != Color.white)
        // {
        //     TurnWhite();
        // }
    }

    public void TakeDamage()
    {
        Debug.Log(gameObject.name + " took some damage!");

        // sr.color = Color.red;

        // Alternative example to Invoke function after time elapsed:
        // Invoke(nameof(TurnWhite), redColorDuration);

        // Reset cooldown timer back to default.
        timer = redColorDuration;
    }
}
