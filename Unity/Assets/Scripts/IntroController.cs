using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [Header("Referencias de la Escena")]
    public RectTransform luzRect;
    public CanvasGroup luzCanvasGroup;
    public CanvasGroup logoCanvasGroup;
    public Canvas canvasPadre;

    [Header("Configuración")]
    public string nombreEscenaMenu = "Menu";
    public float velocidadParpadeo = 0.15f;
    public float velocidadAparicionLogo = 1.5f;

    // Esta es la bandera que bloquea el movimiento al inicio
    private bool puedeSeguirMouse = false;

    private void Start()
    {
        luzCanvasGroup.alpha = 0f;
        logoCanvasGroup.alpha = 0f;

        // Forzamos a que la luz empiece exactamente en el centro de la pantalla
        if (luzRect != null)
        {
            luzRect.anchoredPosition = Vector2.zero;
        }

        StartCoroutine(SecuenciaLuzYLogo());
    }

    private void Update()
    {
        // 1. La luz solo sigue al mouse si la bandera ya es 'true'
        if (puedeSeguirMouse && canvasPadre != null && luzRect != null)
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

        // 2. Ahora detecta Clic Izquierdo, Espacio o Enter
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(nombreEscenaMenu);
        }
    }

    private IEnumerator SecuenciaLuzYLogo()
    {
        // Parpadea 3 veces en el centro
        for (int i = 0; i < 3; i++)
        {
            luzCanvasGroup.alpha = 1f;
            yield return new WaitForSeconds(velocidadParpadeo);
            luzCanvasGroup.alpha = 0f;
            yield return new WaitForSeconds(velocidadParpadeo);
        }

        // Se queda encendida y liberamos a la bestia para que siga el mouse
        luzCanvasGroup.alpha = 1f;
        puedeSeguirMouse = true;

        // Revelamos el logo de Sobremesa
        float t = 0;
        while (t < velocidadAparicionLogo)
        {
            t += Time.deltaTime;
            logoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / velocidadAparicionLogo);
            yield return null;
        }
        logoCanvasGroup.alpha = 1f;
    }
}