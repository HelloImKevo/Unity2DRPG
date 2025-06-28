using System.Collections;
using UnityEngine;

public class Player_VFX : Entity_VFX
{
    [Header("Image Echo VFX")]
    [Range(0.01f, 0.2f)]
    [SerializeField] private float imageEchoInterval = 0.05f;
    [SerializeField] private GameObject imageEchoPrefab;

    private Coroutine imageEchoCo;

    public void DoImageEchoEffect(float duration)
    {
        if (imageEchoCo != null)
        {
            StopCoroutine(imageEchoCo);
        }

        imageEchoCo = StartCoroutine(ImageEchoEffectCo(duration));
    }

    private IEnumerator ImageEchoEffectCo(float duration)
    {
        float timeTracker = 0;

        // Create several afterimage echo objects, which achieves the cool Alucard dash
        // effect from Castlevania: Symphony of the Night.
        while (timeTracker < duration)
        {
            CreateImageEcho();

            yield return new WaitForSeconds(imageEchoInterval);
            timeTracker += imageEchoInterval;
        }
    }

    private void CreateImageEcho()
    {
        // Create an afterimage object, which will quickly fade away.
        GameObject imageEcho = Instantiate(imageEchoPrefab, transform.position, transform.rotation);
        // Change the placeholder VFX image with an image matching the current
        // sprite animation frame of the player.
        imageEcho.GetComponentInChildren<SpriteRenderer>().sprite = sr.sprite;
    }
}
