using UnityEngine;

/// <summary>
/// Interactive object that applies temporary stat buffs to entities that collide with it.
/// Features visual floating animation and timed buff duration management.
/// </summary>
public class Object_Buff : MonoBehaviour
{
    private Player_Stats statsToModify;

    [Header("Buff Details")]
    [SerializeField] private BuffEffectData[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private string buffDescription;
    [Tooltip("Seconds duration of the buff when picked up.")]
    [SerializeField] private float buffDuration = 4f;

    [Header("Floaty Movement")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;

    private Vector3 startPosition;

    /// <summary>
    /// Initializes component references and stores the starting position for floating animation.
    /// </summary>
    private void Awake()
    {
        startPosition = transform.position;
    }

    /// <summary>
    /// Updates the floating animation by smoothly moving the object up and down over time.
    /// </summary>
    private void Update()
    {
        // Smoothly fluctuates between -1 and 1 over time.
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        // Gently move the object higher and lower for a visual floating effect.
        transform.position = startPosition + new Vector3(0, yOffset);
    }

    /// <summary>
    /// Handles collision detection and initiates the buff application process when
    /// triggered by an entity. See also: <see cref="Player_Stats"/>.
    /// </summary>
    /// <param name="collision">The collider that triggered the interaction.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        statsToModify = collision.GetComponent<Player_Stats>();

        if (statsToModify.CanApplyBuffOf(buffName))
        {
            statsToModify.ApplyBuff(buffs, buffDuration, buffName);
            Destroy(gameObject);
        }
    }
}
