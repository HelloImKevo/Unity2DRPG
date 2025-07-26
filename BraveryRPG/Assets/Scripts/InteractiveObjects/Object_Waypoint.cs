using UnityEngine;

public class Object_Waypoint : MonoBehaviour
{
    [Tooltip("Literal name of the Unity Scene to transfer to, such as 'Level_1'")]
    [SerializeField] private string transferToScene;
    [Space]
    [Tooltip("Waypoint type for THIS Waypoint object")]
    [SerializeField] private RespawnType thisWaypointType;
    [Tooltip("Waypoint type for the OTHER Waypoint object. E.g., if THIS Waypoint is an 'Entrance', the connected Waypoint should be an 'Exit'")]
    [SerializeField] private RespawnType connectedWaypoint;
    // [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered = true;

    public RespawnType GetWaypointType() => thisWaypointType;

    // public Vector3 GetPositionAndSetTriggerFalse()
    // {
    //     canBeTriggered = false;
    //     return respawnPoint == null ? transform.position : respawnPoint.position;
    // }

    private void OnValidate()
    {
        gameObject.name = "Object_Waypoint - " + thisWaypointType.ToString() + " - " + transferToScene;

        if (RespawnType.Enter == thisWaypointType)
        {
            connectedWaypoint = RespawnType.Exit;
        }

        if (RespawnType.Exit == thisWaypointType)
        {
            connectedWaypoint = RespawnType.Enter;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeTriggered) return;

        // GameManager.instance.ChangeScene(transferToScene, connectedWaypoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }
}
