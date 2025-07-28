using System.Collections;
using UnityEngine;

public class Entity_VisionCone : MonoBehaviour
{
    [SerializeField] private float visionDistance = 10f;
    [SerializeField] private float visionAngle = 60f; // Degrees
    [SerializeField] private int rayCount = 8;

    [SerializeField] private float checkFrequency = 0.25f; // Check 4x per second
    [SerializeField] private float maxDetectionDistance = 100f;

    [SerializeField] private Transform eyesOrigin;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask playerMask;

    private void OnEnable()
    {
        StartCoroutine(VisionCheckLoop());
    }

    private Transform GetClosestPlayer()
    {
        return Player.GetInstance().transform;
    }

    private IEnumerator VisionCheckLoop()
    {
        WaitForSeconds wait = new(checkFrequency);

        while (true)
        {
            Transform player = GetClosestPlayer();

            if (player != null && Vector2.Distance(transform.position, player.position) <= maxDetectionDistance)
            {
                if (CanSeePlayer())
                {
                    // Debug.Log($"{gameObject.name} spotted the player!");
                    // Insert behavior trigger logic here (e.g., alert state)
                }
            }

            yield return wait;
        }
    }

    public bool CanSeePlayer()
    {
        Vector2 origin = eyesOrigin.position;
        float halfAngle = visionAngle * 0.5f;
        Vector2 forward = transform.right;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = Mathf.Lerp(-halfAngle, halfAngle, i / (float)(rayCount - 1));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg + angleOffset;
            Vector2 direction = DegreeToVector2(angle);

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, visionDistance, groundMask | playerMask);

            if (hit.collider != null)
            {
                if (((1 << hit.collider.gameObject.layer) & playerMask) != 0)
                {
                    Debug.DrawRay(origin, direction * hit.distance, Color.green);
                    return true; // Player is visible
                }

                // Otherwise blocked by ground
                Debug.DrawRay(origin, direction * hit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(origin, direction * visionDistance, Color.gray);
            }
        }

        return false;
    }

    private Vector2 DegreeToVector2(float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
    }

    private void OnDrawGizmosSelected()
    {
        if (eyesOrigin == null) return;

        float halfAngle = visionAngle * 0.5f;
        Vector2 forward = transform.right;

        for (int i = 0; i < rayCount; i++)
        {
            float angleOffset = Mathf.Lerp(-halfAngle, halfAngle, i / (float)(rayCount - 1));
            float angle = Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg + angleOffset;
            Vector2 dir = DegreeToVector2(angle);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(eyesOrigin.position, eyesOrigin.position + (Vector3)(dir * visionDistance));
        }
    }
}
