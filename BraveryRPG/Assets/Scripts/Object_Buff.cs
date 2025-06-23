using System.Collections;
using UnityEngine;

public class Object_Buff : MonoBehaviour
{
    private SpriteRenderer sr;

    [Header("Buff Details")]
    [SerializeField] private float buffDuration = 4;
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

        StartCoroutine(BuffCo(buffDuration));
    }

    private IEnumerator BuffCo(float duration)
    {
        canBeUsed = false;

        // Hide the object (make it invisible), but do not Destroy the
        // object yet (destroying an object will abort all of its Coroutines).
        sr.color = Color.clear;

        Debug.Log("Buff is applied for: " + duration + " seconds");

        yield return new WaitForSeconds(duration);

        Debug.Log("Buff is removed");

        Destroy(gameObject);
    }
}
