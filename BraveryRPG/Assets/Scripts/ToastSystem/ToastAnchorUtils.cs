using UnityEngine;

public static class ToastAnchorUtils
{
    public static Vector2 GetAnchorMin(ToastAnchor anchor)
    {
        return anchor switch
        {
            ToastAnchor.TopLeft => new Vector2(0, 1),
            ToastAnchor.TopCenter => new Vector2(0.5f, 1),
            ToastAnchor.TopRight => new Vector2(1, 1),
            ToastAnchor.Right => new Vector2(1, 0.5f),
            ToastAnchor.BottomRight => new Vector2(1, 0),
            ToastAnchor.BottomCenter => new Vector2(0.5f, 0),
            ToastAnchor.BottomLeft => new Vector2(0, 0),
            ToastAnchor.Left => new Vector2(0, 0.5f),

            _ => new Vector2(0.5f, 0.5f),
        };
    }

    public static Vector2 GetAnchorMax(ToastAnchor anchor) => GetAnchorMin(anchor);
}
