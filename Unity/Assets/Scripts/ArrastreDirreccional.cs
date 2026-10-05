using UnityEngine;
using UnityEngine.EventSystems;

public enum DireccionArrastre { Izquierda, Derecha, Arriba, Abajo }

// A diferencia de CartuchoDraggable (que necesita soltarse en un punto exacto),
// este detecta un arrastre tipo "swipe": basta con mover lo suficiente en la
// dirección indicada, sin importar dónde sueltes el mouse.
// Úsalo para: cerrar el libro (arrastrar página hacia la izquierda) y
// quemar el expediente (arrastrarlo hacia arriba).
public class ArrastreDireccional : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DireccionArrastre direccion = DireccionArrastre.Izquierda;
    public float distanciaMinima = 150f;
    public NexoController nexoController;
    [Tooltip("Texto libre que identifica esta acción, ej. 'cerrarLibro' o 'quemarExpediente'. NexoController lo usa para saber qué hacer.")]
    public string idAccion;

    RectTransform rect;
    Vector2 posicionOriginal;
    Vector2 inicioArrastre;
    CanvasGroup canvasGroup;
    bool completado = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        posicionOriginal = rect.anchoredPosition;
        completado = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (completado) return;
        inicioArrastre = rect.anchoredPosition;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (completado) return;
        rect.anchoredPosition += eventData.delta / rect.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (completado) return;
        canvasGroup.blocksRaycasts = true;

        Vector2 desplazamiento = rect.anchoredPosition - inicioArrastre;
        bool logrado = false;

        switch (direccion)
        {
            case DireccionArrastre.Izquierda: logrado = desplazamiento.x <= -distanciaMinima; break;
            case DireccionArrastre.Derecha: logrado = desplazamiento.x >= distanciaMinima; break;
            case DireccionArrastre.Arriba: logrado = desplazamiento.y >= distanciaMinima; break;
            case DireccionArrastre.Abajo: logrado = desplazamiento.y <= -distanciaMinima; break;
        }

        if (logrado)
        {
            completado = true;
            if (nexoController != null) nexoController.OnArrastreCompletado(idAccion);
        }
        else
        {
            rect.anchoredPosition = posicionOriginal; // no llegó lo suficiente, regresa a su lugar
        }
    }
}