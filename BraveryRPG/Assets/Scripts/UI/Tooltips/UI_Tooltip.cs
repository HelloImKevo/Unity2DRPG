using UnityEngine;

/// <summary>
/// Be sure to un-check "Raycast Target" for the child elements of this Tooltip
/// window, otherwise it will intercept mouse hover events and cause an unwanted
// flickering effect.
/// </summary>
public class UI_Tooltip : MonoBehaviour
{
    private RectTransform rect;

    /// <summary>
    /// Used to prevent the Tooltip edges from going off-screen of the visible
    /// screen to the user, and prevents the tooltip from covering the element
    /// being inspected by the user (such as a Skill Node).
    /// </summary>
    [SerializeField] private Vector2 offset = new(280f, 20f);

    protected virtual void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public virtual void ShowTooltip(bool show, RectTransform targetRect)
    {
        if (!show)
        {
            HideTooltip();
            return;
        }

        // Move the tooltip into view for the user.
        UpdatePosition(targetRect);
    }

    public void HideTooltip()
    {
        // Check for async component lifecycle cleanup to prevent MissingReferenceException.
        if (rect == null) return;

        // Hide the tooltip by moving it way off-screen in an area that
        // should never be visible to the user.
        rect.position = new Vector2(9999f, 9999f);
    }

    /// <summary>
    /// Always determine the Center of the tooltip.
    /// </summary>
    private void UpdatePosition(RectTransform targetRect)
    {
        float screenCenterX = Screen.width / 2f;
        float screenTop = Screen.height;
        // Screen height is measured from the bottom of the screen, upwards.
        float screenBottom = 0f;

        Vector2 targetPosition = targetRect.position;

        // Check if our tooltip is on the right side or left side of the screen.
        targetPosition.x = targetPosition.x > screenCenterX
            ? targetPosition.x - offset.x
            : targetPosition.x + offset.x;

        float verticalHalf = rect.sizeDelta.y / 2f;
        float topY = targetPosition.y + verticalHalf;
        float bottomY = targetPosition.y - verticalHalf;

        if (topY > screenTop)
        {
            // Use a slight offset for some margin between the edge
            // of the screen.
            targetPosition.y = screenTop - verticalHalf - offset.y;
        }
        else if (bottomY < screenBottom)
        {
            targetPosition.y = screenBottom + verticalHalf + offset.y;
        }

        rect.position = targetPosition;
    }

    /// <returns>The text wrapped with HTML color tags with the input hex color.</returns>
    protected string GetColoredText(string color, string text)
    {
        return $"<color={color}>{text}</color>";
    }
}
