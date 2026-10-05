using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------
// Va en el prefab "StoryPanelPrefab". Ese prefab debe tener esta jerarquía:
//
// StoryPanelPrefab (RectTransform, LayoutElement)  <- este script
//   - Artwork (Image)
//   - BubblesContainer (RectTransform, mismo tamaño que Artwork)
//
// Y necesitas otro prefab pequeño "BubblePrefab" con:
//   - RectTransform + Image + CanvasGroup
// -----------------------------------------------------------------------
[RequireComponent(typeof(LayoutElement))]
public class PanelView : MonoBehaviour
{
    [Header("Referencias (arrástralas en el prefab)")]
    [SerializeField] private Image artworkImage;
    [SerializeField] private RectTransform bubblesContainer;
    [SerializeField] private GameObject bubblePrefab;

    // Guarda todo lo que necesitamos de cada globo instanciado en runtime.
    private class BubbleRuntime
    {
        public CanvasGroup canvasGroup;
        public RectTransform rect;
        public Vector2 originalAnchoredPos;
        public BubbleData data;
        public Coroutine shakeRoutine;
    }

    private LayoutElement layoutElement;
    private readonly List<BubbleRuntime> bubbles = new List<BubbleRuntime>();
    private int nextBubbleIndex = 0;

    public int TotalBubbles => bubbles.Count;
    public int RevealedCount => nextBubbleIndex;
    public bool FullyRevealed => nextBubbleIndex >= bubbles.Count;

    private void Awake()
    {
        layoutElement = GetComponent<LayoutElement>();

        // Sin importar cómo haya quedado el anchor en el prefab, forzamos a
        // que el arte y el contenedor de globos llenen SIEMPRE el 100% del
        // panel. Así ya no hay que ajustar el Rect Transform del Artwork a mano.
        StretchToFillParent(artworkImage.rectTransform);
        StretchToFillParent(bubblesContainer);
    }

    private static void StretchToFillParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    // Arma el panel: primero el arte (visible de inmediato), luego instancia
    // los globos pero OCULTOS (alpha 0) listos para revelarse.
    // 'referenceWidth' es el ancho real del Content (lo manda el
    // WebtoonController), lo usamos para calcular un alto que respete la
    // proporción real del sprite cuando no defines un Display Height a mano.
    public void Setup(PanelData data, float referenceWidth, float maxAutoHeight = 0f)
    {
        artworkImage.sprite = data.artwork;

        float height = data.displayHeight;
        if (height <= 0f && data.artwork != null && referenceWidth > 0f)
        {
            float spriteAspect = data.artwork.rect.width / data.artwork.rect.height;
            height = referenceWidth / spriteAspect;

            // Solo recortamos el resultado AUTOMÁTICO (no lo que pongas a mano
            // en Display Height), para que un sprite con mucho margen en
            // blanco no dispare el alto del panel.
            if (maxAutoHeight > 0f)
                height = Mathf.Min(height, maxAutoHeight);
        }
        layoutElement.preferredHeight = height;

        foreach (var bubbleData in data.bubbles)
        {
            GameObject bubbleGO = Instantiate(bubblePrefab, bubblesContainer);
            RectTransform rt = bubbleGO.GetComponent<RectTransform>();
            rt.anchoredPosition = bubbleData.anchoredPosition;

            // Fallback: si el size quedó en (0,0), usa el tamaño nativo del sprite.
            Vector2 size = bubbleData.size;
            if (size == Vector2.zero && bubbleData.bubbleSprite != null)
                size = new Vector2(bubbleData.bubbleSprite.rect.width, bubbleData.bubbleSprite.rect.height);
            rt.sizeDelta = size;

            Image img = bubbleGO.GetComponent<Image>();
            img.sprite = bubbleData.bubbleSprite;

            CanvasGroup cg = bubbleGO.GetComponent<CanvasGroup>();
            cg.alpha = 0f;

            var runtime = new BubbleRuntime
            {
                canvasGroup = cg,
                rect = rt,
                originalAnchoredPos = rt.anchoredPosition,
                data = bubbleData
            };
            bubbles.Add(runtime);

            if (bubbleData.autoReveal)
                StartCoroutine(AutoRevealRoutine(runtime, bubbleData.autoRevealDelay));
        }
    }

    // Llamado por el WebtoonController cuando el jugador hace tap para avanzar.
    // Devuelve el CanvasGroup revelado (o null si ya no quedan globos).
    public CanvasGroup RevealNextBubble()
    {
        if (FullyRevealed) return null;
        BubbleRuntime runtime = bubbles[nextBubbleIndex];
        nextBubbleIndex++;
        RevealBubble(runtime);
        return runtime.canvasGroup;
    }

    public RectTransform GetBubbleRect(int index)
    {
        if (index < 0 || index >= bubbles.Count) return null;
        return bubbles[index].rect;
    }

    private void RevealBubble(BubbleRuntime runtime)
    {
        StartCoroutine(FadeIn(runtime));
    }

    private IEnumerator AutoRevealRoutine(BubbleRuntime runtime, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (runtime.canvasGroup.alpha < 1f)
            yield return FadeIn(runtime);
    }

    private IEnumerator FadeIn(BubbleRuntime runtime)
    {
        const float duration = 0.25f;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            runtime.canvasGroup.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
        runtime.canvasGroup.alpha = 1f;

        // Una vez visible, si el globo tiene shake activado en el inspector,
        // arrancamos el temblor.
        if (runtime.data.shakeEnabled)
            runtime.shakeRoutine = StartCoroutine(ShakeRoutine(runtime));
    }

    private IEnumerator ShakeRoutine(BubbleRuntime runtime)
    {
        BubbleData data = runtime.data;
        float seedX = Random.Range(0f, 100f);
        float seedY = seedX + 37f; // offset fijo para que X y Y no queden sincronizados
        float elapsed = 0f;

        while (data.shakeForever || elapsed < data.shakeDuration)
        {
            float t = Time.time * data.shakeSpeed;
            float offsetX = 0f;
            float offsetY = 0f;

            switch (data.shakeAxis)
            {
                case ShakeAxis.Horizontal:
                    offsetX = (Mathf.PerlinNoise(seedX, t) - 0.5f) * 2f * data.shakeIntensity;
                    break;
                case ShakeAxis.Vertical:
                    offsetY = (Mathf.PerlinNoise(seedY, t) - 0.5f) * 2f * data.shakeIntensity;
                    break;
                case ShakeAxis.Both:
                    offsetX = (Mathf.PerlinNoise(seedX, t) - 0.5f) * 2f * data.shakeIntensity;
                    offsetY = (Mathf.PerlinNoise(seedY, t) - 0.5f) * 2f * data.shakeIntensity;
                    break;
            }

            runtime.rect.anchoredPosition = runtime.originalAnchoredPos + new Vector2(offsetX, offsetY);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Al terminar, regresa a su posición original en vez de quedarse
        // congelado a mitad de un temblor.
        runtime.rect.anchoredPosition = runtime.originalAnchoredPos;
    }
}