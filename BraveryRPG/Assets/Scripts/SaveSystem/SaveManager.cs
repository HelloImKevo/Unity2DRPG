using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Expose the static SaveManager instance to be available as a Singleton from any script.
    public static SaveManager instance;

    private FileDataHandler dataHandler;
    private GameData gameData;

    [SerializeField] private string fileName = "gamesavedata.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;

        StartCoroutine(InitializeSaveSystem());
    }

    private IEnumerator InitializeSaveSystem()
    {
        // macOS Default Path:
        // /Users/johnny/Library/Application Support/DefaultCompany/BraveryRPG
        // Windows Default Path:
        // C:/Users/Johnny/AppData/LocalLow/DefaultCompany/WindowsRPGGame
        Debug.Log($"{GetType().Name}.Start() -> Data Path: {Application.persistentDataPath}");
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);

        // Let all other Unity components get initialized before loading the game.
        yield return null;
        LoadGame();
    }

    /// <summary>Lifecycle function sequence: OnApplicationQuit -> OnDisable -> OnDestroy</summary>
    void OnApplicationQuit()
    {
        // This will save the game whenever you close the game or stop the play mode.
        SaveGame();
    }

    public void SaveGame()
    {
        List<ISaveable> allSaveables = FindISaveables();

        Debug.Log($"{GetType().Name}.SaveGame() -> Saving data for {allSaveables.Count} objects");

        foreach (var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData);
        }

        dataHandler.SaveData(gameData);
    }

    public void LoadSceneData()
    {
        LoadGame();
    }

    private void LoadGame()
    {
        Debug.Log($"{GetType().Name}.LoadGame() -> Loading Game Data...");

        gameData = dataHandler.LoadData();

        if (gameData == null)
        {
            Debug.Log("No save data found, creating new save!");
            gameData = new GameData();
            return;
        }

        List<ISaveable> allSaveables = FindISaveables();

        Debug.Log($"{GetType().Name}.LoadGame() -> Loading data for {allSaveables.Count} objects");

        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    public GameData GetGameData() => gameData;

    private List<ISaveable> FindISaveables()
    {
        return FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .OrderBy(saveable => saveable is GameManager ? 1 : 0) // 0 = non-GameManager, 1 = GameManager
            .ToList();
    }

    #region Unity Menu Actions

    [ContextMenu("*** Delete Save Data ***")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();

        // TODO: We should show a 'Confirm Action' modal dialog.
        if (Application.isPlaying)
        {
            ToastStyle warningStyle = new()
            {
                textColor = Color.white,
                backgroundColor = new Color(0.2f, 0, 0),
                blinkColor = Color.red,
                enableBlink = true,
                duration = 2f
            };

            ToastManager.Instance.ShowToast(
                "Save data deleted!!",
                warningStyle,
                ToastAnchor.BottomCenter
            );
        }

        // If the game had already been loaded in-memory, we need to reload
        // everything back to a fresh start.
        LoadGame();
    }

    [ContextMenu("*** Clear All Checkpoints ***")]
    public void ClearAllCheckpoints()
    {
        var handler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        var data = handler.LoadData();

        if (data == null)
        {
            Debug.LogWarning("ClearAllCheckpoints() -> data is null!");
            return;
        }

        data.unlockedCheckpoints.Clear();
        handler.SaveData(data);
    }

    #endregion
}
