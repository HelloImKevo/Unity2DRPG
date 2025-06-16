using UnityEngine;

public class VFX_AutoController : MonoBehaviour
{
    [SerializeField] private bool autoDestroy = true;
    [SerializeField] private float destroyDelay = 1f;
    [Space]
    [SerializeField] private bool randomOffset = true;
    [SerializeField] private bool randomRotation = true;

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

    private void Start()
    {
        ApplyRandomOffset();
        ApplyRandomRotation();

        if (autoDestroy) Destroy(gameObject, destroyDelay);
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

        float zRotation = Random.Range(0, 360);
        transform.Rotate(0, 0, zRotation);
    }
}
