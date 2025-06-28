using System.Collections;
using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    private SpriteRenderer sr;

    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;
    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;

    [Header("Fade Effect")]
    [SerializeField] private bool canFade;
    [SerializeField] private float fadeSpeed = 1;

    [Header("Random Rotation")]
    [SerializeField] private float minRotation = 0f;
    [SerializeField] private float maxRotation = 360f;

    [Header("Random Positioning")]
    [Range(-1, 0)]
    [SerializeField] private float xMinOffset = -0.3f;
    [Range(0, 1)]
    [SerializeField] private float xMaxOffset = 0.3f;
    [Space]
    [Range(-1, 0)]
    [SerializeField] private float yMinOffset = -0.3f;
    [Range(1, 0)]
    [SerializeField] private float yMaxOffset = 0.3f;

    void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        if (canFade)
        {
            StartCoroutine(FadeCo());
        }

        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy) Destroy(gameObject, destroyDelay);
    }

    /// <summary>
    /// Used for the Dash afterimage effect.
    /// </summary>
    private IEnumerator FadeCo()
    {
        Color targetColor = Color.white;

        while (targetColor.a > 0)
        {
            targetColor.a -= fadeSpeed * Time.deltaTime;
            sr.color = targetColor;
            // Reduce alpha a little bit, then wait for the next frame.
            yield return null;
        }

        sr.color = targetColor;
    }

    private void ApplyRandomOffset()
    {
        if (!randomOffset) return;

        float xOffset = Random.Range(xMinOffset, xMaxOffset);
        float yOffset = Random.Range(yMinOffset, yMaxOffset);

        transform.position += new Vector3(xOffset, yOffset);
    }

    private void ApplyRandomRotation()
    {
        if (!randomRotation) return;

        float zRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(0, 0, zRotation);
    }
}
