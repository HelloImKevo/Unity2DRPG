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

    [Range(-50f, 50f)]
    [Tooltip("Allows for fine-tuning of the precise positions of Skill Nodes in the tree.")]
    public float rotation = 0f;
}

/// <summary>
/// Should be attached to an instance of [UI_TreeNode]. Handles configuration
/// of Skill Nodes connected to this Node.
/// </summary>
public class UI_TreeConnectHandler : MonoBehaviour
{
    [Tooltip("Child UI_TreeNode Component, containing child Line Direction and Line Length.")]
    [SerializeField] private UI_TreeConnectDetails[] connectionDetails;

    [Tooltip("The 'Connection' groups (parent object containing one or more child lines) originating from this Skill Node. If this Skill does not have any children, this can be empty.")]
    [SerializeField] private UI_TreeConnection[] myConnectionLines;

    private Image connectionImage;
    private Color originalColor;

    private RectTransform Rect => GetComponent<RectTransform>();

    void Awake()
    {
        if (connectionImage != null) originalColor = connectionImage.color;
    }

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
            // Connect handler gets access to connection details it has in the Unity inspector.
            // One of the tree nodes has a connection line. We get access to the connection image.
            // We take the child node, which is an 'Image'
            Image connectionImage = connection.GetConnectionImage();

            connection.DirectConnection(detail.direction, detail.length, detail.rotation);

            // Guard against MissingReferenceExceptions in the Unity Editor.
            if (detail.childNode == null) continue;

            detail.childNode.SetPosition(targetPosition);
            detail.childNode.SetConnectionImage(connectionImage);
            // Prevents the Red Connection Square Point of parent nodes from being sorted
            // on top of child Skill node images and being visible to the player.
            detail.childNode.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// Do not invoke this function in OnValidate, as it is possible in some circumstances
    /// to trigger endless recursion, resulting in a StackOverflow crash.
    /// </summary>
    public void UpdateAllConnections()
    {
        UpdateConnections();

        foreach (var node in connectionDetails)
        {
            if (node.childNode == null) continue;

            node.childNode.UpdateConnections();
        }
    }

    public void UnlockConnectionImage(bool unlocked)
    {
        if (connectionImage == null) return;

        connectionImage.color = unlocked ? Color.white : originalColor;
    }

    public void SetConnectionImage(Image image) => connectionImage = image;

    /// <summary>
    /// Sets the position of this connection handler's RectTransform.
    /// </summary>
    /// <param name="position">The new anchored position to set.</param>
    public void SetPosition(Vector2 position) => Rect.anchoredPosition = position;
}
