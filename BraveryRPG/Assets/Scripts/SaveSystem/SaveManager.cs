using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager instance;

    private FileDataHandler dataHandler;
    private GameData gameData;
    private List<ISaveable> allSaveables;

    [SerializeField] private string fileName = "gamesavedata.json";
    [SerializeField] private bool encryptData = true;

    private void Awake()
    {
        instance = this;
    }

    private IEnumerator Start()
    {
        // macOS Default Path:
        // /Users/johnny/Library/Application Support/DefaultCompany/BraveryRPG
        // Windows Default Path:
        // C:/Users/Johnny/AppData/LocalLow/DefaultCompany/WindowsRPGGame
        Debug.Log($"{GetType().Name}.Start() -> Data Path: {Application.persistentDataPath}");
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        allSaveables = FindISaveables();

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
        foreach (var saveable in allSaveables)
        {
            saveable.SaveData(ref gameData);
        }

        dataHandler.SaveData(gameData);
    }

    private void LoadGame()
    {
        gameData = dataHandler.LoadData();

        if (gameData == null)
        {
            Debug.Log("No save data found, creating new save!");
            gameData = new GameData();
            return;
        }

        foreach (var saveable in allSaveables)
        {
            saveable.LoadData(gameData);
        }
    }

    public GameData GetGameData() => gameData;

    [ContextMenu("*** Delete Save Data ***")]
    public void DeleteSaveData()
    {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
        dataHandler.Delete();

        LoadGame();
    }

    private List<ISaveable> FindISaveables()
    {
        return
            FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISaveable>()
            .ToList();
    }
}
