using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle healthBarToggle;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;

    [Header("BGM Volume Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParametr;

    [Header("SFX Volume Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParametr;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    public void BGMSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(bgmParametr, newValue);
    }

    public void SFXSliderValue(float value)
    {
        float newValue = MathF.Log10(value) * mixerMultiplier;
        audioMixer.SetFloat(sfxParametr, newValue);
    }

    private void OnHealthBarToggleChanged(bool isOn)
    {
        // TODO: Currently this setting only works for the Player Health Bar.
        // We could persist this setting to the game data, and disable enemy
        // health bars also.
        player.Health.EnableHealthBar(isOn);
    }

    public void GoMainMenuBTN() => GameManager.instance.ChangeScene("MainMenu", RespawnType.NonSpecific);

    // private void OnEnable()
    // {
    //     sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, 0.6f);
    //     bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, 0.6f);
    // }

    // private void OnDisable()
    // {
    //     PlayerPrefs.SetFloat(sfxParametr, sfxSlider.value);
    //     PlayerPrefs.SetFloat(bgmParametr, bgmSlider.value);
    // }

    // public void LoadUpVolume()
    // {
    //     sfxSlider.value = PlayerPrefs.GetFloat(sfxParametr, 0.6f);
    //     bgmSlider.value = PlayerPrefs.GetFloat(bgmParametr, 0.6f);
    // }
}
