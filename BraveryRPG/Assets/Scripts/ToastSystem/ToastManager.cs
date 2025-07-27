using UnityEngine;

/// <summary>
/// <example>
/// Example usage:
/// <code>
/// ToastStyle style = new ToastStyle
/// {
///     textColor = Color.white,
///     backgroundColor = new Color(0.2f, 0, 0),
///     blinkColor = Color.red,
///     enableBlink = true,
///     duration = 2f
/// };
///
/// ToastManager.Instance.ShowToast("Invalid Password!", style, ToastAnchor.Above, passwordFieldRect);
/// </code>
/// results in a blinking error Toast to be shown.
/// </example>
/// </summary>
public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    [Tooltip("Toast prefab containing at least a background and a message element.")]
    [SerializeField] private Toast toastPrefab;

    // NOTE: Avoid holding references to Canvas instances in a Singleton construct,
    // as it could cause the Canvas instance to be carried-forward into other scenes.

    [Tooltip("Margin distance from the edges of the visible screen.")]
    [SerializeField] private float screenMargin = 40f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void ShowToast(
        string message,
        ToastStyle style,
        ToastAnchor anchor,
        Transform relativeTo = null
    )
    {
        // The [Graphy] package uses a separate Canvas instance, so we need to perform
        // a more specialized lookup.
        RectTransform parent = FindAnyObjectByType<UI>().GetComponent<RectTransform>();

        Debug.Log("ToastManager.ShowToast() -> Instantiating toast prefab, parent: " + parent.name + ", message: " + message);

        // Instantiate the Toast prefab instance as a child of the Canvas instance
        // (rather than a child of the 'Singletons' object, or whatever this script
        // is a component of). This will reset scale, position, and rotation correctly.
        Toast toast = Instantiate(toastPrefab, parent, false);
        toast.Initialize(message, style, anchor, relativeTo, screenMargin);
    }
}
