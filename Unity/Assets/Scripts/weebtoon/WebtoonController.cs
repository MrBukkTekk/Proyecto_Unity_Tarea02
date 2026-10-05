using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// -----------------------------------------------------------------------
// Va en un GameObject vacío en la escena (ej. "WebtoonController").
// Necesita referencias al ScrollRect de tu Canvas:
//
// Canvas
//   - ScrollView (ScrollRect)
//       - Viewport
//           - Content (VerticalLayoutGroup + ContentSizeFitter[Vertical])
// -----------------------------------------------------------------------
public class WebtoonController : MonoBehaviour
{
    [Header("Datos de la historia")]
    [SerializeField] private StoryData storyData;

    [Header("Referencias UI")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject panelPrefab;

    [Header("Input")]
    [Tooltip("Permite avanzar presionando Espacio/Enter, útil para probar en el editor sin mouse")]
    [SerializeField] private bool advanceWithKeyboard = true;

    [Header("Auto-scroll")]
    [SerializeField] private float scrollDuration = 0.3f;

    [Header("Ajuste de tamaño automático")]
    [Tooltip("Alto máximo (en pixeles) que puede tomar un panel cuyo Display Height quedó en 0. Útil mientras tus placeholders traen mucho espacio en blanco alrededor del dibujo. 0 = sin límite.")]
    [SerializeField] private float maxAutoHeight = 1600f;

    [Header("Transición")]
    public string siguienteEscena; // Aquí escribirás "Historia 2" en el Inspector

    private readonly List<PanelView> panelViews = new List<PanelView>();
    private int currentPanelIndex = 0;
    private Coroutine scrollRoutine;

    private void Start()
    {
        GameManager.Instance.GuardarPartida(GameManager.Instance.currentSlot);
        BuildStory();
        StartCoroutine(ResetScrollToTopNextFrame());
    }

    private void Update()
    {
        if (advanceWithKeyboard && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
        {
            Advance();
        }
    }

    // El tap real (que sí distingue de un drag de scroll) lo dispara
    // TapToAdvance.cs vía OnPointerClick, llamando a Advance() público.

    // El Content Size Fitter / Vertical Layout Group tarda un frame en
    // terminar de calcular la altura real tras instanciar los paneles, así
    // que si fijamos el scroll en el mismo frame puede quedar mal. Esperamos
    // un frame y forzamos el rebuild antes de anclar arriba.
    private IEnumerator ResetScrollToTopNextFrame()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 1f;
    }

    // Instancia todos los paneles de la historia dentro del Content del scroll.
    private void BuildStory()
    {
        if (storyData == null)
        {
            Debug.LogWarning("WebtoonController: no hay StoryData asignado.");
            return;
        }

        // El Content ya tiene su ancho resuelto por sus propios anchors
        // (estirado al Viewport), así que este valor es confiable de
        // inmediato, sin esperar ningún rebuild de layout.
        float referenceWidth = content.rect.width > 0f ? content.rect.width : Screen.width;

        foreach (var panelData in storyData.panels)
        {
            GameObject panelGO = Instantiate(panelPrefab, content);
            PanelView view = panelGO.GetComponent<PanelView>();
            view.Setup(panelData, referenceWidth, maxAutoHeight);
            panelViews.Add(view);
        }
    }

    // Lógica principal de "jugabilidad": un tap revela el siguiente globo del
    // panel actual; si ya no quedan globos, pasa al siguiente panel y hace scroll.
    public void Advance()
    {
        // Si por alguna razón damos un clic extra estando ya en el final
        if (currentPanelIndex >= panelViews.Count)
        {
            IrALaSiguienteEscena();
            return;
        }

        PanelView current = panelViews[currentPanelIndex];

        if (!current.FullyRevealed)
        {
            CanvasGroup revealed = current.RevealNextBubble();
            if (revealed != null)
                ScrollTo(revealed.GetComponent<RectTransform>());
        }
        else
        {
            currentPanelIndex++; // Pasamos al siguiente panel

            // Justo aquí revisamos: si al sumar 1 ya nos acabamos la historia, saltamos de una vez
            if (currentPanelIndex >= panelViews.Count)
            {
                IrALaSiguienteEscena();
            }
            else
            {
                // Si aún quedan paneles, hacemos el scroll suave hacia el siguiente
                ScrollTo(panelViews[currentPanelIndex].GetComponent<RectTransform>());
            }
        }
    }

    // Separo esto en otra función chiquita para que quede más limpio
    private void IrALaSiguienteEscena()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(siguienteEscena))
        {
            NexoState.modoSalida = true;
            GameManager.Instance.CambiarDeHistoria(siguienteEscena);
        }
        else
        {
            Debug.LogWarning("Ojo: Falta el GameManager en la escena o dejaste vacía la variable 'Siguiente Escena'.");
        }
    }
    // Hace scroll suave para que 'target' quede visible dentro del viewport.
    private void ScrollTo(RectTransform target)
    {
        if (target == null) return;
        if (scrollRoutine != null) StopCoroutine(scrollRoutine);
        scrollRoutine = StartCoroutine(SmoothScrollTo(target));
    }

    private IEnumerator SmoothScrollTo(RectTransform target)
    {
        // Esperamos un frame para que el layout (VerticalLayoutGroup /
        // ContentSizeFitter) termine de recalcular tamaños tras instanciar.
        yield return null;
        Canvas.ForceUpdateCanvases();

        float contentHeight = content.rect.height;
        float viewportHeight = scrollRect.viewport.rect.height;

        Vector3 targetLocalPos = content.InverseTransformPoint(target.position);
        float targetYFromTop = -targetLocalPos.y;

        float maxScroll = Mathf.Max(contentHeight - viewportHeight, 0.0001f);
        float normalizedFromTop = Mathf.Clamp01(targetYFromTop / maxScroll);
        float targetNormalized = 1f - normalizedFromTop; // ScrollRect: 1 = arriba, 0 = abajo

        float startNormalized = scrollRect.verticalNormalizedPosition;
        float elapsed = 0f;
        while (elapsed < scrollDuration)
        {
            elapsed += Time.deltaTime;
            scrollRect.verticalNormalizedPosition =
                Mathf.Lerp(startNormalized, targetNormalized, elapsed / scrollDuration);
            yield return null;
        }
        scrollRect.verticalNormalizedPosition = targetNormalized;
    }
}