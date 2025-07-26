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
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        // Do not destroy the object when changing scenes.
        DontDestroyOnLoad(gameObject);
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
    }

    public void ChangeScene(string sceneName, RespawnType respwanType)
    {
        SaveManager.instance.SaveGame();

        // Time.timeScale = 1;
        StartCoroutine(ChangeSceneCo(sceneName, respwanType));
    }

    private IEnumerator ChangeSceneCo(string sceneName, RespawnType respawnType)
    {
        // UI_FadeScreen fadeScreen = FindFadeScreenUI();
        // fadeScreen.DoFadeOut(); // transperent > black
        // yield return fadeScreen.fadeEffectCo;
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(sceneName);

        // Note: If we don't wait long enough for the scene to load, the  player will be
        // teleported first, and then the default Player-In-Scene position will be loaded,
        // and the Player will snap-back to the wrong "default position".
        yield return new WaitForSeconds(1f);

        // dataLoaded = false; // data loaded becomes true when you load game from save manager
        // yield return null;

        // while (!dataLoaded)
        // {
        //     yield return null;
        // }

        // fadeScreen = FindFadeScreenUI();
        // fadeScreen.DoFadeIn(); // black > transperent

        Player player = Player.instance;

        if (player == null) yield break;

        Vector3 position = GetNewPlayerPosition(respawnType);

        Debug.Log($"{GetType().Name}.ChangeSceneCo() -> New Player Position = {position}");

        if (position != Vector3.zero)
        {
            player.TeleportPlayer(position);
        }
    }

    // private UI_FadeScreen FindFadeScreenUI()
    // {
    //     if (UI.instance != null)
    //     {
    //         return UI.instance.fadeScreenUI;
    //     }
    //     else
    //     {
    //         return FindFirstObjectByType<UI_FadeScreen>();
    //     }
    // }

    private Vector3 GetNewPlayerPosition(RespawnType type)
    {
        // if (type == RespawnType.Portal)
        // {
        //     Object_Portal portal = Object_Portal.instance;

        //     Vector3 position = portal.GetPosition();

        //     portal.SetTrigger(false);
        //     portal.DisableIfNeeded();

        //     return position;
        // }

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

        if (currentScene == "MainMenu") return;

        // data.lastPlayerPosition = Player.instance.transform.position;
        // data.lastScenePlayed = currentScene;
        dataLoaded = false;
    }

    public void LoadData(GameData data)
    {
        // lastScenePlayed = data.lastScenePlayed;
        // lastPlayerPosition = data.lastPlayerPosition;

        if (string.IsNullOrEmpty(lastScenePlayed))
        {
            lastScenePlayed = "Level_0";
        }

        dataLoaded = true;
    }

    #endregion
}
