using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class Buff
{
    public StatType type;
    public float value;
}

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

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
    }

    private void Update()
    {
        // Smoothly fluctuates between -1 and 1 over time.
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatRange;
        // Gently move the object higher and lower for a visual floating effect.
        transform.position = startPosition + new Vector3(0, yOffset);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeUsed) return;

        statsToModify = collision.GetComponent<Entity_Stats>();
        StartCoroutine(BuffCo(buffDuration));
    }

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
