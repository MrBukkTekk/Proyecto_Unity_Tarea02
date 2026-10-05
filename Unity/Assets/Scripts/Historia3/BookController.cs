using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Para detectar el clic
using UnityEngine.SceneManagement;

public class BookController : MonoBehaviour, IPointerClickHandler
{
    [Header("Configuración del Libro")]
    [Tooltip("El componente Image que muestra las páginas")]
    public Image bookImage;

    [Tooltip("Arrastra aquí todas las imágenes del libro en orden")]
    public Sprite[] pages;

    [Tooltip("Duración total de la vuelta de hoja en segundos")]
    public float flipDuration = 0.4f;

    [Header("Transición")]
    public string siguienteEscena; // Aquí escribirás "Historia 4", "Historia 5", etc.

    private int currentPageIndex = 0;
    private bool isFlipping = false; // Evita que el jugador rompa la página haciendo doble clic rápido

    void Start()
    {
        GameManager.Instance.GuardarPartida(GameManager.Instance.currentSlot);

        // Al iniciar, mostramos la primera página asegurándonos de que esté en tamaño original
        if (pages.Length > 0 && bookImage != null)
        {
            bookImage.sprite = pages[0];
            bookImage.rectTransform.localScale = Vector3.one;
        }
    }

    // Esto detecta cuando le das clic a la imagen
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFlipping || pages == null || pages.Length == 0) return;

        // Si hay más páginas, pasa a la siguiente. Si no, salimos del minijuego.
        if (currentPageIndex < pages.Length - 1)
        {
            StartCoroutine(FlipPageRoutine());
        }
        else
        {
            TerminarLibro();
        }
    }

    private IEnumerator FlipPageRoutine()
    {
        isFlipping = true;
        float halfDuration = flipDuration / 2f;
        float t = 0f;
        Vector3 currentScale = bookImage.rectTransform.localScale;

        // 1. Aplastar la imagen (X va de 1 a 0)
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            currentScale.x = Mathf.Lerp(1f, 0f, t / halfDuration);
            bookImage.rectTransform.localScale = currentScale;
            yield return null;
        }

        // 2. Justo cuando está aplastada (invisible), cambiamos el Sprite a la página nueva
        currentPageIndex++;
        bookImage.sprite = pages[currentPageIndex];

        // 3. Expandir la nueva imagen (X va de 0 a 1)
        t = 0f;
        while (t < halfDuration)
        {
            t += Time.deltaTime;
            currentScale.x = Mathf.Lerp(0f, 1f, t / halfDuration);
            bookImage.rectTransform.localScale = currentScale;
            yield return null;
        }

        // Nos aseguramos de que quede perfectamente en 1 al terminar
        currentScale.x = 1f;
        bookImage.rectTransform.localScale = currentScale;
        isFlipping = false;
    }

    private void TerminarLibro()
    {
        Debug.Log("Libro terminado. Regresando a la novela...");

        // Avisamos a la mochila que terminamos este evento
        if (GameManager.Instance != null)
        {
            GameManager.Instance.pointAndClickCompletado = true;
        }

        // Pon el nombre de tu escena principal de la novela visual
        if (GameManager.Instance != null && !string.IsNullOrEmpty(siguienteEscena))
        {
            NexoState.modoSalida = true;
            GameManager.Instance.CambiarDeHistoria(siguienteEscena);
        }
    }
}