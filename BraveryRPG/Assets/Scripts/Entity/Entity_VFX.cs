using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    [Header("Image Echo VFX")]
    [Range(0.01f, 0.2f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoPrefab;
    private Coroutine imageEchoCo;

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
    [SerializeField] private Color burnVfx = Color.orangeRed;
    [SerializeField] private Color shockVfx = Color.yellow;
    private Color originalHitVfxColor;

    private Entity entity;
    protected SpriteRenderer sr;

    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    private Coroutine statusBlinkVfxCo;

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
            statusBlinkVfxCo = StartCoroutine(PlayStatusBlinkVfxCo(duration, chillVfx));
        }

        if (ElementType.Fire == element)
        {
            statusBlinkVfxCo = StartCoroutine(PlayStatusBlinkVfxCo(duration, burnVfx));
        }

        if (ElementType.Lightning == element)
        {
            statusBlinkVfxCo = StartCoroutine(PlayStatusBlinkVfxCo(duration, shockVfx));
        }
    }

    #region Image Echo Effect

    public void DoImageEchoEffect(float duration)
    {
        StopImageEchoEffect();
        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    public void StopImageEchoEffect()
    {
        if (imageEchoCo != null)
        {
            StopCoroutine(imageEchoCo);
        }
    }

    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float timeTracker = 0;

        // Create several afterimage echo objects, which achieves the cool Alucard dash
        // effect from Castlevania: Symphony of the Night.
        while (timeTracker < duration)
        {
            CreateImageEcho();

            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho()
    {
        Vector3 position = entity.Anim.transform.position;
        float scale = entity.Anim.transform.localScale.x;

        // Create an afterimage object, which will quickly fade away.
        GameObject imageEcho = Instantiate(imageEchoPrefab, position, transform.rotation);

        imageEcho.transform.localScale = new Vector3(scale, scale, scale);
        // Change the placeholder VFX image with an image matching the current
        // sprite animation frame of the player or enemy.
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }

    #endregion

    public void StopAllVfx()
    {
        // Stops all coroutines running on this behaviour.
        StopAllCoroutines();
        sr.color = Color.white;
        sr.material = originalMaterial;
    }

    private IEnumerator PlayStatusBlinkVfxCo(float duration, Color effectColor)
    {
        float tickInterval = 0.5f;
        float timeHasPassed = 0;

        // Note: Using a scale less than 1 will result in solid opaque colors,
        // which is not what we want.
        // Using this technique of applying a color to the SpriteRenderer
        // generally does not work with alpha channels - you need to use
        // a Shader to achieve a partially transparent white tint.
        Color lightColor = ColorUtils.GetTintedColor(effectColor, 1.4f);
        Color darkColor = ColorUtils.GetTintedColor(effectColor, 0.9f);

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

        // Remove sprite tint.
        sr.color = Color.white;
    }

    // Summary:
    //     Creates an instance of [hitVfx] at the [target] location,
    //     with the same rotation (quaternion).
    public void CreateOnHitVfx(Transform target, bool isCrit, ElementType element)
    {
        GameObject hitPrefab = isCrit ? critHitVfx : hitVfx;
        GameObject vfx = Instantiate(hitPrefab, target.position, Quaternion.identity);
        if (!isCrit)
        {
            // Note: We could recolor the Crit VFX asset to be grayscale, so that
            // it can be colorized with our tint.
            vfx.GetComponentInChildren<SpriteRenderer>().color = GetElementColor(element);
        }

        if (entity.FacingDir == -1 && isCrit)
        {
            // Rotate the prefab VFX to face the opposite direction.
            vfx.transform.Rotate(0, 180, 0);
        }
    }

    public Color GetElementColor(ElementType element)
    {
        return element switch
        {
            ElementType.Ice => chillVfx,

            ElementType.Fire => burnVfx,

            ElementType.Lightning => shockVfx,

            _ => Color.white,
        };
    }

    public void PlayOnDamageFlashWhiteVfx()
    {
        // Check if task is already running, and stop it.
        if (onDamageVfxCoroutine != null) StopCoroutine(onDamageVfxCoroutine);

        // Spawn the new coroutine.
        // Apply a white blink effect to the Sprite when damaged.
        // TODO: This color (Material) kind of interferes with active blinking status effects.
        onDamageVfxCoroutine = StartCoroutine(LaunchDamageFlashWhiteVfxCo());
    }

    /// <summary>
    /// Makes the entity sprite briefly flash white.
    /// </summary>
    private IEnumerator LaunchDamageFlashWhiteVfxCo()
    {
        sr.material = onDamageMaterial;

        yield return new WaitForSeconds(onDamageVfxDuration);

        sr.material = originalMaterial;
    }
}
