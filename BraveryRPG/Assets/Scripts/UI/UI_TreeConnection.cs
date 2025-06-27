using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages individual connection lines between skill tree nodes, handling direction, length, and positioning.
/// </summary>
public class UI_TreeConnection : MonoBehaviour
{
    [Tooltip("Should be the parent 'Connection' object that serves as the rotation pivot point.")]
    [SerializeField] private RectTransform centerNodePoint;

    [Tooltip("Should be the child 'ConnectionLine' object.")]
    [SerializeField] private RectTransform connectionLine;

    [Tooltip("Point at the 'End of the Line', which is what child nodes will connect to.")]
    [SerializeField] private RectTransform childNodeConnectionPoint;

    /// <summary>
    /// Configures the connection line's direction, length, and rotation based on the specified parameters.
    /// </summary>
    /// <param name="direction">The direction type for the connection line.</param>
    /// <param name="length">The length of the connection line.</param>
    /// <param name="offset">Additional rotation offset to apply.</param>
    public void DirectConnection(NodeDirectionType direction, float length, float offset)
    {
        bool shouldBeActive = direction != NodeDirectionType.None;
        float finalLength = shouldBeActive ? length : 0;
        float angle = GetDirectionAngle(direction);

        centerNodePoint.localRotation = Quaternion.Euler(0, 0, angle + offset);
        connectionLine.sizeDelta = new Vector2(finalLength, connectionLine.sizeDelta.y);
    }

    /// <summary>
    /// Calculates the world position of the connection endpoint for child node positioning.
    /// </summary>
    /// <param name="rect">The parent RectTransform to calculate relative position from.</param>
    /// <returns>The local position of the connection endpoint.</returns>
    public Vector2 GetConnectionPoint(RectTransform rect)
    {
        // Handles World Position, Local Position & Anchored Position,
        // and calculates exact point within relative Screen dimensions.
        RectTransformUtility.ScreenPointToLocalPointInRectangle
            (
                rect.parent as RectTransform,
                // World position of 'End of the Line' point.
                childNodeConnectionPoint.position,
                null,
                out var localPosition
            );

        return localPosition;
    }

    /// <summary>
    /// Converts a NodeDirectionType enum value to its corresponding rotation angle in degrees.
    /// </summary>
    /// <param name="type">The direction type to convert.</param>
    /// <returns>The rotation angle in degrees for the specified direction.</returns>
    private float GetDirectionAngle(NodeDirectionType type)
    {
        switch (type)
        {
            case NodeDirectionType.UpLeft: return 135f;
            case NodeDirectionType.Up: return 90f;
            case NodeDirectionType.UpRight: return 45f;
            case NodeDirectionType.Left: return 180f;
            case NodeDirectionType.Right: return 0f;
            case NodeDirectionType.DownLeft: return -135f;
            case NodeDirectionType.Down: return -90;
            case NodeDirectionType.DownRight: return -45f;

            default: return 0f;
        }
    }
}

/// <summary>
/// Defines the eight directional types for skill tree node connections.
/// </summary>
public enum NodeDirectionType
{
    None,
    UpLeft,
    Up,
    UpRight,
    Right,
    DownRight,
    Down,
    DownLeft,
    Left
}
