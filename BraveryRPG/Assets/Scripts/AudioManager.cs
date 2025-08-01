using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] private AudioDatabaseSO audioDB;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Space]
    private Transform player;

    private AudioClip lastMusicPlayed;
    private string currentBgmGroupName;
    private Coroutine currentBgmCo;
    [SerializeField] private bool bgmShouldPlay;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (!bgmSource.isPlaying && bgmShouldPlay)
        {
            if (!string.IsNullOrEmpty(currentBgmGroupName))
            {
                NextBGM(currentBgmGroupName);
            }
        }

        if (bgmSource.isPlaying && !bgmShouldPlay)
        {
            StopBGM();
        }
    }

    public void StartBGM(string musicGroup)
    {
        bgmShouldPlay = true;

        if (musicGroup == currentBgmGroupName) return;

        NextBGM(musicGroup);
    }

    public void NextBGM(string musicGroup)
    {
        bgmShouldPlay = true;
        currentBgmGroupName = musicGroup;

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
        }

        currentBgmCo = StartCoroutine(SwitchMusicCo(musicGroup));
    }

    public void StopBGM()
    {
        bgmShouldPlay = false;

        StartCoroutine(FadeVolumeCo(bgmSource, 0, 1));

        if (currentBgmCo != null)
        {
            StopCoroutine(currentBgmCo);
        }
    }

    /// <summary>Cross-fade to a new BGM (Background Music) music track.</summary>
    private IEnumerator SwitchMusicCo(string musicGroup)
    {
        AudioClipData data = audioDB.Get(musicGroup);
        AudioClip nextMusic = data.GetRandomClip();

        if (data == null || data.clips.Count == 0)
        {
            Debug.Log("No audio found for group" + musicGroup);
            yield break;
        }

        // If there is only 1 music option in the clips group, play that one.
        // Otherwise, pick a different music track different from the last one played.
        if (data.clips.Count > 1)
        {
            while (nextMusic == lastMusicPlayed)
            {
                nextMusic = data.GetRandomClip();
            }
        }

        // Fade out the existing music track.
        if (bgmSource.isPlaying)
        {
            yield return FadeVolumeCo(bgmSource, 0, 1f);
        }

        lastMusicPlayed = nextMusic;
        bgmSource.clip = nextMusic;
        bgmSource.volume = 0;
        bgmSource.Play();

        StartCoroutine(FadeVolumeCo(bgmSource, data.maxVolume, 1f));
    }

    /// <summary>Fade out BGM (Background Music) instead of a harsh cut.</summary>
    private IEnumerator FadeVolumeCo(AudioSource source, float targetVolume, float duration)
    {
        float time = 0;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.deltaTime;

            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }

        source.volume = targetVolume;
    }

    public void PlaySFX(string soundName, AudioSource sfxSource, float minDistanceToHearSound = 5)
    {
        if (player == null)
        {
            player = Player.GetInstance().transform;
        }

        var data = audioDB.Get(soundName);
        if (data == null)
        {
            Debug.Log("Attempt to play sound - " + soundName);
            return;
        }

        var clip = data.GetRandomClip();
        if (clip == null) return;

        float maxVolume = data.maxVolume;
        float distance = Vector2.Distance(sfxSource.transform.position, player.position);
        // Use exponential falloff to decrease the volume of the SFX the
        // further away the player is from this audio source.
        float t = Mathf.Clamp01(1 - (distance / minDistanceToHearSound));

        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = Mathf.Lerp(0, maxVolume, t * t); // exponential falloff
        sfxSource.PlayOneShot(clip);

        // To add Enemy Footsteps / Movement Noise, you would add another
        // Audio Source with Looping enabled, and then use loopingSource.Play()
        // and loopingSource.Stop()
    }

    /// <summary>
    /// Play a sound audible to the player, regardless of their distance to the source.
    /// Example: A key event triggers a church bell ring.
    /// </summary>
    public void PlayGlobalSFX(string soundName)
    {
        var data = audioDB.Get(soundName);
        if (data == null) return;

        var clip = data.GetRandomClip();
        if (clip == null) return;

        Debug.Log("Play Global Audio SFX: " + soundName);
        sfxSource.pitch = Random.Range(0.95f, 1.1f);
        sfxSource.volume = data.maxVolume;
        sfxSource.PlayOneShot(clip);
    }
}
