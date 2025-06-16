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

    private SpriteRenderer sr;
    private Material originalMaterial;
    private Coroutine onDamageVfxCoroutine;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    // Summary:
    //     Creates an instance of [hitVfx] at the [target] location,
    //     with the same rotation (quaternion).
    public void CreateOnHitVfx(Transform target)
    {
        GameObject vfx = Instantiate(hitVfx, target.position, Quaternion.identity);
        vfx.GetComponentInChildren<SpriteRenderer>().color = hitVfxColor;
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
