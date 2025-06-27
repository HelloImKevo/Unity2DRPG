using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Represents a temporary stat modification that can be applied to an entity.
/// </summary>
[Serializable]
public class Buff
{
    public StatType type;
    public float value;
}

/// <summary>
/// Interactive object that applies temporary stat buffs to entities that collide with it.
/// Features visual floating animation and timed buff duration management.
/// </summary>
public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;
    private Entity_Stats statsToModify;

    [Header("Buff Details")]
    [SerializeField] private Buff[] buffs;
    [SerializeField] private string buffName;
    [SerializeField] private string buffDescription;
    [Tooltip("Seconds duration of the buff when picked up.")]
    [SerializeField] private float buffDuration = 4f;
    [SerializeField] private bool canBeUsed = true;

    [Header("Floaty Movement")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float floatRange = 0.1f;

    private Vector3 startPosition;

    /// <summary>
    /// Initializes component references and stores the starting position for floating animation.
    /// </summary>
    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
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
    /// Handles collision detection and initiates the buff application process when triggered by an entity.
    /// </summary>
    /// <param name="collision">The collider that triggered the interaction.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeUsed) return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }

    /// <summary>
    /// Coroutine that manages the buff lifecycle, including application, duration timing, and cleanup.
    /// </summary>
    /// <param name="duration">The duration in seconds for how long the buff should last.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;

        // Hide the object (make it invisible), but do not Destroy the
        // object yet (destroying an object will abort all of its Coroutines).
        sr.color = Color.clear;
        ApplyBuff(true);

        Debug.Log($"Buff '{buffName}' is applied for {duration} seconds");

        yield return new WaitForSeconds(duration);

        ApplyBuff(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// Applies or removes buff effects to the target entity's stats based on the apply parameter.
    /// </summary>
    /// <param name="apply">True to apply buffs, false to remove them.</param>
    private void ApplyBuff(bool apply)
    {
        foreach (var buff in buffs)
        {
            if (apply)
            {
                statsToModify.GetStatByType(buff.type).AddModifier(buff.value, buffName);
            }
            else
            {
                statsToModify.GetStatByType(buff.type).RemoveModifier(buffName);
            }
        }
    }
}
