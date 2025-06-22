using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    [Header("On Damaged VFX")]
    [Tooltip("Shader effect to be activated during the VFX duration")]
    [SerializeField] private Material onDamageMaterial;
    [Tooltip("How many seconds to activate the VFX event - should be something small like 0.15")]
    [SerializeField] private float onDamageVfxDuration = 0.15f;

    [Header("On Being Hit VFX")]
    [Tooltip("Prefab with a contact sparks animation")]
    [SerializeField] private GameObject hitVfx;
    [Tooltip("Tint color to apply to the Hit VFX")]
    [SerializeField] private Color hitVfxColor = Color.white;
    [Tooltip("Prefab with larger green energy wave")]
    [SerializeField] private GameObject critHitVfx;

    [Header("Element Colors")]
    [SerializeField] private Color chillVfx = Color.cyan;
    private Color originalHitVfxColor;

    private Entity entity;
    private SpriteRenderer sr;

    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    private void Awake()
    {
        entity = GetComponent<Entity>();
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
        originalHitVfxColor = hitVfxColor;
    }

    public void PlayOnStatusBlinkVfx(float duration, ElementType element)
    {
        if (ElementType.Ice == element)
        {
            StartCoroutine(PlayStatusBlinkVfxCo(duration, chillVfx));
        }
    }

    private IEnumerator PlayStatusBlinkVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.25f;
        float timeHasPassed = 0;

        Color lightColor = effectColor * 1.2f;
        Color darkColor = effectColor * 0.9f;

        bool toggle = false;

        while (timeHasPassed < duration)
        {
            sr.color = toggle ? lightColor : darkColor;
            toggle = !toggle;

            // Pause the coroutine briefly.
            yield return new WaitForSeconds(tickInterval);

            // Increment time tracker by the pause interval.
            timeHasPassed += tickInterval;
        }

        sr.color = Color.white;
    }

    // Summary:
    //     Creates an instance of [hitVfx] at the [target] location,
    //     with the same rotation (quaternion).
    public void CreateOnHitVfx(Transform target, bool isCrit)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        if (!isCrit)
        {
            // Note: We could recolor the Crit VFX asset to be grayscale, so that
            // it can be colorized with our tint.
            vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;
        }

        if (entity.FacingDir == -1 && isCrit)
        {
            // Rotate the prefab VFX to face the opposite direction.
            vfx.transform.Rotate(0, 180, 0);
        }
    }

    public void UpdateOnHitColor(ElementType element)
    {
        if (ElementType.Ice == element) hitVfxColor = chillVfx;

        // Reset "On Hit" visual effect tint to original configuration.
        if (ElementType.None == element) hitVfxColor = originalHitVfxColor;
    }

    public void PlayOnDamageVfx()
    {
        // Check if task is already running, and stop it.
        if (onDamageVfxCoroutine != null) StopCoroutine(onDamageVfxCoroutine);

        // Spawn the new coroutine.
        onDamageVfxCoroutine = StartCoroutine(LaunchDamageVfxTask());
    }

    private IEnumerator LaunchDamageVfxTask()
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }
}
