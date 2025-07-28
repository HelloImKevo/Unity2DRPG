using UnityEngine;

public class AudioRangeController : MonoBehaviour
{
    private AudioSource source;
    private Transform player;

    [SerializeField] private float minDistanceToHearSound = 12f;
    [SerializeField] private bool showDistanceGizmo;

    private float maxVolume;

    private void Start()
    {
        player = Player.GetInstance().transform;
        source = GetComponent<AudioSource>();

        maxVolume = source.volume;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(player.position, transform.position);
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));

        // Use exponential falloff.
        float targetVolume = Mathf.Lerp(0, maxVolume, t * t);
        // Lerp the volume gradually, in case the player is quickly dashing or
        // teleporting past an audio source.
        source.volume = Mathf.Lerp(source.volume, targetVolume, Time.deltaTime * 3);
    }

    private void OnDrawGizmos()
    {
        if (showDistanceGizmo)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, minDistanceToHearSound);
        }
    }
}
