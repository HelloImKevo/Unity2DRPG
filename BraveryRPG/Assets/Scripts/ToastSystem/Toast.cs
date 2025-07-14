using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Toast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private Image backgroundImage;

    private float lifetime;
    private Color normalColor;
    private Color blinkColor;
    private bool blinking;

    public void Initialize(
        string message,
        ToastStyle style,
        ToastAnchor anchor,
        Transform relativeTo,
        float screenMargin
    )
    {
        toastText.text = message;
        toastText.color = style.textColor;
        backgroundImage.color = style.backgroundColor;

        normalColor = style.textColor;
        blinkColor = style.blinkColor;
        blinking = style.enableBlink;
        lifetime = style.duration;

        SetAnchorPosition(anchor, relativeTo, screenMargin);
        StartCoroutine(HandleLifecycle());
    }

    private void SetAnchorPosition(ToastAnchor anchor, Transform relativeTo, float screenMargin)
    {
        RectTransform rt = GetComponent<RectTransform>();

        if (relativeTo != null)
        {
            rt.position = relativeTo.position;

            switch (anchor)
            {
                case ToastAnchor.Above:
                    rt.anchoredPosition += new Vector2(0, screenMargin);
                    break;
                case ToastAnchor.Below:
                    rt.anchoredPosition += new Vector2(0, -screenMargin);
                    break;
                case ToastAnchor.Left:
                    rt.anchoredPosition += new Vector2(-screenMargin, 0);
                    break;
                case ToastAnchor.Right:
                    rt.anchoredPosition += new Vector2(screenMargin, 0);
                    break;

                default:
                    Debug.LogWarning($"[Toast] Relative positioning only supports Above, Below, Left, Right. " +
                                     $"'{anchor}' was provided — defaulting to centered overlay.");
                    break;
            }
        }
        else
        {
            rt.anchorMin = ToastAnchorUtils.GetAnchorMin(anchor);
            rt.anchorMax = ToastAnchorUtils.GetAnchorMax(anchor);
            rt.pivot = new Vector2(0.5f, 0.5f); // Center pivot
            rt.anchoredPosition = GetOffsetForAnchor(anchor, screenMargin);
        }
    }

    private Vector2 GetOffsetForAnchor(ToastAnchor anchor, float margin)
    {
        return anchor switch
        {
            ToastAnchor.TopLeft => new Vector2(margin, -margin),
            ToastAnchor.TopCenter => new Vector2(0, -margin),
            ToastAnchor.TopRight => new Vector2(-margin, -margin),

            ToastAnchor.Right => new Vector2(-margin, 0),
            ToastAnchor.Left => new Vector2(margin, 0),

            ToastAnchor.BottomRight => new Vector2(-margin, margin),
            ToastAnchor.BottomCenter => new Vector2(0, margin),
            ToastAnchor.BottomLeft => new Vector2(margin, margin),

            _ => Vector2.zero,
        };
    }

    private IEnumerator HandleLifecycle()
    {
        float timePassed = 0f;
        while (timePassed < lifetime)
        {
            if (blinking)
            {
                toastText.color = Color.Lerp(normalColor, blinkColor, Mathf.PingPong(Time.time * 4, 1));
            }
            timePassed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
