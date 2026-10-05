using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LibroTest : MonoBehaviour
{
    public Image paginaVisual;
    public Color[] coloresPagina;

    [TextArea(3, 5)]
    public string[] textosPaginas;

    public TMP_Text textoPantalla;

    // --- NUEVOS AUDIOS SEPARADOS ---
    public AudioSource audioGrito; // Solo suena una vez
    public AudioSource audioGore;  // Suena en bucle (loop)
    public int paginaDelSusto = 11; // Índice de la página 12

    private int indicePagina = 0;

    void Start()
    {
        if (coloresPagina.Length > 0)
            paginaVisual.color = coloresPagina[0];

        if (textosPaginas.Length > 0 && textoPantalla != null)
            textoPantalla.text = textosPaginas[0];
    }

    public void SiguientePagina()
    {
        indicePagina++;

        // TRAMPA PARA APAGAR EL GORE: Si ya pasamos la página del susto, callamos el bucle
        if (indicePagina > paginaDelSusto && audioGore != null && audioGore.isPlaying)
        {
            audioGore.Stop();
        }

        if (indicePagina < coloresPagina.Length)
            paginaVisual.color = coloresPagina[indicePagina];

        if (indicePagina < textosPaginas.Length && textoPantalla != null)
        {
            textoPantalla.text = textosPaginas[indicePagina];

            // TRAMPA PARA ENCENDER LOS SUSTOS: Si llegamos a la página exacta
            if (indicePagina == paginaDelSusto)
            {
                if (audioGrito != null) audioGrito.Play();
                if (audioGore != null) audioGore.Play();
            }
        }
        else
        {
            Debug.Log("Fin de la historia.");
        }
    }
}