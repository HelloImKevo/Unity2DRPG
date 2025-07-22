using UnityEditor;
using UnityEngine;

public class Object_Checkpoint : MonoBehaviour, ISaveable
{
    [SerializeField] private string checkpointId;
    [SerializeField] private Transform respawnPoint;

    public bool isActive { get; private set; }
    private Animator anim;
    // private AudioSource fireAudioSource;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        // fireAudioSource = GetComponent<AudioSource>();
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(checkpointId))
        {
            // Generate a unique GUID for each Checkpoint.
            checkpointId = System.Guid.NewGuid().ToString();
        }
#endif
    }

    public string GetCheckpointId() => checkpointId;

    public Vector3 GetPosition() => respawnPoint == null ? transform.position : respawnPoint.position;

    public void ActivateCheckpoint(bool activate)
    {
        isActive = activate;
        anim.SetBool("isActive", activate);

        // if (isActive && !fireAudioSource.isPlaying)
        // {
        //     fireAudioSource.Play();
        // }

        // if (!isActive)
        // {
        //     fireAudioSource.Stop();
        // }
    }

    // Should only activate when the Player enters this collider, thanks to
    // the Layer Collisions Matrix.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ActivateCheckpoint(true);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(respawnPoint.position, 1f);
    }

    #region ISaveable

    public void SaveData(ref GameData data)
    {
        if (!isActive) return;

        if (!data.unlockedCheckpoints.ContainsKey(checkpointId))
        {
            data.unlockedCheckpoints.Add(checkpointId, true);
        }
    }

    public void LoadData(GameData data)
    {
        bool active = data.unlockedCheckpoints.TryGetValue(checkpointId, out active);
        ActivateCheckpoint(active);
    }

    #endregion
}
