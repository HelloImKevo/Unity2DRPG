using UnityEngine;
using UnityEngine.SceneManagement;

public class Object_Portal : MonoBehaviour, ISaveable
{
    public static Object_Portal instance;

    public bool isActive { get; private set; }

    [Tooltip("Where the Portal appears in Town.")]
    [SerializeField] private Vector2 defaultPosition;
    [SerializeField] private string townSceneName = "Level_0";

    [SerializeField] private Transform respawnPoint;
    [SerializeField] private bool canBeTriggered;

    private string currentSceneName;
    /// <summary>The destination scene the player will be returned to.</summary>
    private string returnSceneName;
    private bool returningFromTown;

    private void Awake()
    {
        instance = this;
        currentSceneName = SceneManager.GetActiveScene().name;
        // Hide by default.
        HidePortal();
    }

    public void ActivatePortal(Vector3 position, int facingDir = 1)
    {
        isActive = true;
        transform.position = position;
        SaveManager.instance.GetGameData().inScenePortals.Clear();

        if (facingDir == -1)
        {
            transform.Rotate(0, 180, 0);
        }
    }

    public void DisableIfNeeded()
    {
        if (!returningFromTown) return;

        SaveManager.instance.GetGameData().inScenePortals.Remove(currentSceneName);
        isActive = false;
        HidePortal();
    }

    private void HidePortal()
    {
        // Move the portal into outer space.
        transform.position = new Vector3(9999, 9999);
    }

    private void UseTeleport()
    {
        string destinationScene = InTown() ? returnSceneName : townSceneName;
        GameManager.instance.ChangeScene(destinationScene, RespawnType.Portal);
    }

    public void SetTrigger(bool trigger) => canBeTriggered = trigger;

    public Vector3 GetPosition() => respawnPoint != null ? respawnPoint.position : transform.position;

    private bool InTown() => currentSceneName == townSceneName;

    #region Collider Logic

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeTriggered) return;

        UseTeleport();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canBeTriggered = true;
    }

    #endregion

    #region ISaveable

    public void SaveData(ref GameData data)
    {
        data.returningFromTown = InTown();

        if (isActive && !InTown())
        {
            data.inScenePortals[currentSceneName] = transform.position;
            data.portalDestinationSceneName = currentSceneName;
        }
        else
        {
            data.inScenePortals.Remove(currentSceneName);
        }
    }

    public void LoadData(GameData data)
    {
        if (InTown() && data.inScenePortals.Count > 0)
        {
            transform.position = defaultPosition;
            isActive = true;
        }
        else if (data.inScenePortals.TryGetValue(currentSceneName, out Vector3 portalPosition))
        {
            transform.position = portalPosition;
            isActive = true;
        }

        returningFromTown = data.returningFromTown;
        returnSceneName = data.portalDestinationSceneName;

        Debug.Log($"{GetType().Name}.LoadData() -> Returning From Town? {returningFromTown}" +
                  $" Return Scene Name = '{returnSceneName}'");
    }

    #endregion
}
