using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

// NOTE: Unity framework has a 'SceneManager' built-in type.
public class GameManager : MonoBehaviour, ISaveable
{
    public static GameManager instance;
    private Vector3 lastPlayerPosition;

    private string lastScenePlayed;
    private bool dataLoaded;

    private void Awake()
    {
        // Prevent one-off instances of *** GAME MANAGER *** from being initialized
        // in other scenes.
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // Do not destroy the object when changing scenes.
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    private void OnDestroy()
    {
        Debug.LogWarning("GameManager has been destroyed.");
    }

    public void SetLastPlayerPosition(Vector3 position) => lastPlayerPosition = position;

    public void ContinuePlay()
    {
        ChangeScene(lastScenePlayed, RespawnType.NonSpecific);
    }

    public void RestartScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"{GetType().Name}.RestartScene() -> Restarting '{sceneName}'");
        ChangeScene(sceneName, RespawnType.NonSpecific);
        HideCurrentSceneGameUI();
    }

    private void HideCurrentSceneGameUI()
    {
        if (TryGetComponent<UI>(out var userInterface))
        {
            userInterface.HideAllUI();
        }
    }

    public void ChangeScene(string sceneName, RespawnType respwanType)
    {
        SaveManager.instance.SaveGame();

        // Unpause the game, for situations where we are transitioning from a Pause Menu.
        UnpauseGame();
        StartCoroutine(ChangeSceneCo(sceneName, respwanType));
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        Debug.Log($"{GetType().Name}.ChangeSceneCo() -> Showing Black Screen, then transitioning to = {sceneName}");

        UI_FadeScreen fadeScreen = FindFadeScreenUI();
        fadeScreen.FadeToBlack();
        yield return fadeScreen.fadeEffectCo;

        HideCurrentSceneGameUI();

        SceneManager.LoadScene(sceneName);

        // Data loaded becomes true when you load game from save manager.
        dataLoaded = false;
        // Add a 1-frame delay before proceeding to the while loop.
        yield return null;

        SaveManager.instance.LoadSceneData();

        while (!dataLoaded)
        {
            yield return null;
        }

        Player player = Player.GetInstance();

        if (player == null)
        {
            Debug.Log($"{GetType().Name}.ChangeSceneCo() -> Player instance is null, hiding the Fade Loading Screen ...");
            yield return HideFadeScreenUI();
            yield break;
        }

        Vector3 position = GetNewPlayerPosition(respawnType);

        if (position != Vector3.zero)
        {
            Debug.Log($"{GetType().Name}.ChangeSceneCo() -> Teleporting Player to New Position = {position}");
            player.TeleportPlayer(position);
        }

        Debug.Log($"{GetType().Name}.ChangeSceneCo() -> Hiding the Fade Loading Screen ...");
        yield return HideFadeScreenUI();
    }

    private IEnumerator HideFadeScreenUI()
    {
        // We have to re-find the Loading screen after calling LoadScene().
        UI_FadeScreen fadeScreen = FindFadeScreenUI();

        yield return new WaitForSeconds(0.2f);

        fadeScreen.FadeToTransparent();

        yield return fadeScreen.fadeEffectCo;
    }

    private UI_FadeScreen FindFadeScreenUI()
    {
        if (TryGetComponent<UI>(out var userInterface))
        {
            return userInterface.fadeScreenUI;
        }
        else
        {
            return FindFirstObjectByType<UI_FadeScreen>(FindObjectsInactive.Include);
        }
    }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        if (RespawnType.Portal == type)
        {
            Object_Portal portal = Object_Portal.instance;

            Vector3 position = portal.GetPosition();

            portal.SetTrigger(false);
            portal.DisableIfNeeded();

            return position;
        }

        // Prevent player from respawning close to the Exit waypoint
        // (so the Waypoint system cannot be easily exploited).
        if (RespawnType.NonSpecific == type)
        {
            var data = SaveManager.instance.GetGameData();
            var checkpoints = FindObjectsByType<Object_Checkpoint>(FindObjectsSortMode.None);
            var unlockedCheckpoints = checkpoints
                .Where(cp => data.unlockedCheckpoints.TryGetValue(
                    cp.GetCheckpointId(), out bool unlocked) && unlocked
                )
                .Select(cp => cp.GetPosition())
                .ToList();

            var enterWaypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None)
                .Where(wp => wp.GetWaypointType() == RespawnType.Enter)
                // Deactivate the Entrance Waypoints, so that we don't immediately trigger
                // the teleport mechanism when respawning the player on the Waypoint.
                .Select(wp => wp.GetPositionAndSetTriggerFalse())
                .ToList();

            // Combine two lists into one.
            var selectedPositions = unlockedCheckpoints.Concat(enterWaypoints).ToList();

            if (selectedPositions.Count == 0) return Vector3.zero;

            // Arrange from lowest to highest by comparing distance.
            return selectedPositions.
                OrderBy(position => Vector3.Distance(position, lastPlayerPosition))
                .First();
        }

        return GetWaypointPosition(type);
    }

    private Vector3 GetWaypointPosition(RespawnType type)
    {
        var waypoints = FindObjectsByType<Object_Waypoint>(FindObjectsSortMode.None);

        foreach (var point in waypoints)
        {
            if (point.GetWaypointType() == type)
            {
                return point.GetPositionAndSetTriggerFalse();
            }
        }

        return Vector3.zero;
    }

    #region ISaveable

    public void SaveData(ref GameData data)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // We don't want to save the player's last played scene as "Main Menu".
        if (currentScene == "MainMenu") return;

        if (TryGetComponent<Player>(out var player))
        {
            // This approach would require refinement for multiplayer.
            data.lastPlayerPosition = player.transform.position;
        }
        data.lastScenePlayed = currentScene;

        dataLoaded = false;
    }

    public void LoadData(GameData data)
    {
        lastScenePlayed = data.lastScenePlayed;
        lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
        {
            lastScenePlayed = "Level_0";
        }

        Debug.Log($"{GetType().Name}.LoadData() -> Data is loaded!");
        dataLoaded = true;
    }

    #endregion
}
