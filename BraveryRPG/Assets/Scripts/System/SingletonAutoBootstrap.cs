using UnityEngine;

public static class SingletonAutoBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeSingletons()
    {
        if (GameManager.instance == null || SaveManager.instance == null || AudioManager.instance == null)
        {
            GameObject singletonPrefab = Resources.Load<GameObject>("___SINGLETONS___");
            GameObject instance = Object.Instantiate(singletonPrefab);
            Object.DontDestroyOnLoad(instance);
        }
    }
}
