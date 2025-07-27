using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeScreen : MonoBehaviour
{
    public Coroutine fadeEffectCo { get; private set; }
    private Image fadeImage;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        // Start each scene in a Blackened state.
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.2f);

        // If we are launching a specific scene from the Unity Editor (bypassing
        // the GameManager scene choreography), we should auto-hide the black screen.
        if (ShouldAutoFadeToTransparent())
        {
            Debug.Log($"{GetType().Name}.Start() -> Fade coroutine is not running, starting fade to transparent!");

            // If the fade effect is not running, start the fade effect.
            FadeToTransparent();
        }
    }

    private bool ShouldAutoFadeToTransparent()
    {
        if (fadeImage.color.a != 0f)
        {
            return fadeEffectCo == null;
        }

        return false;
    }

    /// <summary>Transition from Transparent to Black.</summary>
    public void FadeToBlack(float duration = 0.3f)
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        FadeEffect(1f, duration);
    }

    /// <summary>Transition from Black to Transparent.</summary>
    public void FadeToTransparent(float duration = 0.3f)
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        FadeEffect(0f, duration);
    }

    private void FadeEffect(float targetAlpha, float duration)
    {
        if (fadeEffectCo != null)
        {
            StopCoroutine(fadeEffectCo);
        }

        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha, float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = color;

            yield return null;
        }

        fadeImage.color = new Color(
            fadeImage.color.r,
            fadeImage.color.g,
            fadeImage.color.b,
            targetAlpha
        );
    }
}
