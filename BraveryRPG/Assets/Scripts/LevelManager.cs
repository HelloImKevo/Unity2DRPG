using UnityEngine;

/// <summary>NOT A SINGLETON. Controls background music for levels of our game.</summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private string musicGroupName;

    private void Start()
    {
        AudioManager.instance.StartBGM(musicGroupName);
    }
}
