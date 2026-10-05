using UnityEngine;

public class LuzSigueMouse : MonoBehaviour
{
    [Tooltip("Arrastra el Canvas principal aquí")]
    public Canvas canvasPadre;

    private RectTransform luzRect;

    void Start()
    {
        // Toma automáticamente el RectTransform de la imagen a la que se lo pongas
        luzRect = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (canvasPadre != null && luzRect != null)
        {
            Vector2 posicionLocal;
            Camera camaraCanvas = canvasPadre.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvasPadre.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasPadre.transform as RectTransform,
                Input.mousePosition,
                camaraCanvas,
                out posicionLocal);

            luzRect.anchoredPosition = posicionLocal;
        }
    }
}