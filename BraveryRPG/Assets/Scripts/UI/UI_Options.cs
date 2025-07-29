using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class UI_Options : MonoBehaviour
{
    private Player player;
    [SerializeField] private Toggle healthBarToggle;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float mixerMultiplier = 25f;

    [Header("BGM Volume Settings")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private string bgmParameter;

    [Header("SFX Volume Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private string sfxParameter;

    private void Start()
    {
        player = FindFirstObjectByType<Player>();

        healthBarToggle.onValueChanged.AddListener(OnHealthBarToggleChanged);
    }

    public void BGMSliderValue(float value)
    {
        audioMixer.SetFloat(bgmParameter, ConvertSliderToDecibels(value));
    }

    public void SFXSliderValue(float value)
    {
        audioMixer.SetFloat(sfxParameter, ConvertSliderToDecibels(value));
    }

    /// <summary>Slider values default to a range 0 - 1</summary>
    private float ConvertSliderToDecibels(float value) => MathF.Log10(value) * mixerMultiplier;

    private void OnHealthBarToggleChanged(bool isOn)
    {
        AudioManager.instance.PlayGlobalSFX("button_click");
        // TODO: Currently this setting only works for the Player Health Bar.
        // We could persist this setting to the game data, and disable enemy
        // health bars also.
        // TODO: This null-check is just a quick & dirty fix for the Main Menu.
        // Revisit this and implement a more robust solution.
        if (player != null)
        {
            player.Health.EnableHealthBar(isOn);
        }
    }

    public void GoMainMenuBTN()
    {
        AudioManager.instance.PlayGlobalSFX("button_click");
        GameManager.instance.ChangeScene("MainMenu", RespawnType.NonSpecific);
    }

    private void OnEnable()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.6f);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(sfxParameter, sfxSlider.value);
        PlayerPrefs.SetFloat(bgmParameter, bgmSlider.value);
    }

    public void LoadUpVolume()
    {
        sfxSlider.value = PlayerPrefs.GetFloat(sfxParameter, 0.6f);
        bgmSlider.value = PlayerPrefs.GetFloat(bgmParameter, 0.6f);
    }
}
