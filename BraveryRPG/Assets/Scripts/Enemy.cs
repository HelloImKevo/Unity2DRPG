using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SpriteRenderer sr;
    [SerializeField] private float redColorDuration = 1;

    public float timer;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ChangeColorIfNeeded();
    }

    private void ChangeColorIfNeeded()
    {
        timer -= Time.deltaTime;
        timer = Mathf.Max(0, timer);

        if (timer <= 0 && sr.color != Color.white)
        {
            TurnWhite();
        }
    }

    public void TakeDamage()
    {
        Debug.Log(gameObject.name + " took some damage!");

        sr.color = Color.red;

        // Alternative example to Invoke function after time elapsed:
        // Invoke(nameof(TurnWhite), redColorDuration);

        // Reset cooldown timer back to default.
        timer = redColorDuration;
    }

    private void TurnWhite()
    {
        sr.color = Color.white;
    }
}
