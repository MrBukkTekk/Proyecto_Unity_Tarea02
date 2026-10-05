using UnityEngine;
using UnityEngine.EventSystems;


public class CartuchoDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Objetivo")]
    public RectTransform ranuraObjetivo;
    public float distanciaParaEncajar = 80f;

    [Header("Referencia")]
    public NexoController nexoController;

    RectTransform rect;
    Vector2 posicionOriginal;
    CanvasGroup canvasGroup;
    bool encajado = false;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        posicionOriginal = rect.anchoredPosition;
        encajado = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (encajado) return;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (encajado) return;
        rect.anchoredPosition += eventData.delta / rect.lossyScale.x;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (encajado) return;
        canvasGroup.blocksRaycasts = true;

        float distancia = Vector2.Distance(rect.position, ranuraObjetivo.position);
        if (distancia <= distanciaParaEncajar)
        {
            rect.position = ranuraObjetivo.position;
            encajado = true;
            if (nexoController != null) nexoController.OnCartuchoEncajado();
        }
        else
        {
            rect.anchoredPosition = posicionOriginal; // no encajó, regresa a su lugar
        }
    }
}