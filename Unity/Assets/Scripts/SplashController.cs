using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashController : MonoBehaviour
{
    [Header("Referencias")]
    public CanvasGroup logoCanvasGroup;
    public Image logoImage;

    [Header("Logos en orden de aparición")]
    public Sprite[] logos;

    [Header("Tiempos (segundos) por logo")]
    public float fadeInDuration = 1f;
    public float holdDuration = 2f;
    public float fadeOutDuration = 1f;

    [Header("Siguiente escena")]
    public string nextSceneName = "Inicio";

    [Header("Permitir saltar (salta SOLO el logo actual, no toda la secuencia)")]
    public bool allowSkip = true;

    bool skipRequested = false;

    void Start()
    {
        if (logoCanvasGroup != null)
            logoCanvasGroup.alpha = 0f;

        StartCoroutine(PlaySplashSequence());
    }

    void Update()
    {
        if (allowSkip && (Input.GetMouseButtonDown(0) || Input.anyKeyDown))
        {
            skipRequested = true;
        }
    }

    IEnumerator PlaySplashSequence()
    {
        foreach (Sprite logo in logos)
        {
            skipRequested = false;
            if (logoImage != null) logoImage.sprite = logo;

            yield return Fade(0f, 1f, fadeInDuration);
            if (skipRequested) continue;

            float elapsed = 0f;
            while (elapsed < holdDuration && !skipRequested)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return Fade(1f, 0f, fadeOutDuration);
        }

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        if (logoCanvasGroup == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (skipRequested) break;
            elapsed += Time.deltaTime;
            logoCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        if (!skipRequested) logoCanvasGroup.alpha = to;
    }
}