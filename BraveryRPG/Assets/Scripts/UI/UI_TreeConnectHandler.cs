using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Configuration data for connecting skill tree nodes, including direction and distance information.
/// </summary>
[Serializable]
public class UI_TreeConnectDetails
{
    [Tooltip("A child UI_TreeNode instance, like a Skill Tree Node prefab.")]
    public UI_TreeConnectHandler childNode;

    public NodeDirectionType direction;

    [Range(50f, 350f)]
    public float length;
}

/// <summary>
/// Should be attached to an instance of [UI_TreeNode]. Handles configuration
/// of Skill Nodes connected to this Node.
/// </summary>
public class UI_TreeConnectHandler : MonoBehaviour
{
    [Tooltip("Array containing a child UI_TreeNode endpoint, Line Direction and Line Length.")]
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;

    [Tooltip("The 'Connection' groups (parent object containing one or more child lines) originating from this Skill Node. If this Skill does not have any children, this can be empty.")]
    [SerializeField] private UI_TreeConnection[] myConnectionLines;

    private RectTransform Rect => GetComponent<RectTransform>();

    /// <summary>
    /// Validates connection configuration and updates connections when values change in the Unity editor.
    /// </summary>
    private void OnValidate()
    {
        if (connectionDetails.Length <= 0) return;

        if (connectionDetails.Length != myConnectionLines.Length)
        {
            Debug.Log("Details Array Size should match Connections Array Size - " + gameObject.name);
            return;
        }

        UpdateConnections();
    }

    /// <summary>
    /// Updates all connection lines and positions child nodes according to their configuration.
    /// </summary>
    public void UpdateConnections()
    {
        for (int i = 0; i < connectionDetails.Length; i++)
        {
            var detail = connectionDetails[i];
            var connection = myConnectionLines[i];

            Vector2 targetPosition = connection.GetConnectionPoint(Rect);
            // Image connectionImage = connection.GetConnectionImage();

            connection.DirectConnection(detail.direction, detail.length, 0f); // FIXME

            if (detail.childNode == null) continue;

            detail.childNode.SetPosition(targetPosition);
            // detail.childNode.SetConnectionImage(connectionImage);
            // detail.childNode.transform.SetAsLastSibling();
        }
    }

    // public void SetConnectionImage(Image image) => connectionImage = image;

    /// <summary>
    /// Sets the position of this connection handler's RectTransform.
    /// </summary>
    /// <param name="position">The new anchored position to set.</param>
    public void SetPosition(Vector2 position) => Rect.anchoredPosition = position;
}
