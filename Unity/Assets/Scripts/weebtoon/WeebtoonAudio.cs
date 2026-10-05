using UnityEngine;
using UnityEngine.UI;

public class WebtoonAudio : MonoBehaviour
{
    [Header("Conexiones")]
    public Scrollbar barraVertical;

    [Header("Audios")]
    public AudioSource audioAmbiente;
    public AudioSource audioScroll; // El clic de la ruedita del ratón
    public AudioSource audioClimax; // Tu pista maestra (Tensión + Jumpscare + Arrastre)

    private bool yaAsusto = false;

    void Start()
    {
        // Arranca el ambiente de tensión silenciosa en cuanto inicia la escena
        if (audioAmbiente != null) audioAmbiente.Play();
    }

    void Update()
    {
        // Detectar si el jugador mueve físicamente la rueda del ratón
        if (Mathf.Abs(Input.mouseScrollDelta.y) > 0f)
        {
            // Solo suena si no está sonando ya, para evitar ruido de metralleta
            if (audioScroll != null && !audioScroll.isPlaying)
            {
                // Cambiamos el tono (pitch) muy poquito para que suene orgánico
                audioScroll.pitch = Random.Range(0.9f, 1.1f);
                audioScroll.Play();
            }
        }
    }

    // Esta es la trampa del Jumpscare que conectaremos a la barra en Unity
    public void RevisarScroll()
    {
        // Cuando la barra llegue casi al final del cómic (0.1 o menos)
        if (barraVertical.value <= 0.1f && !yaAsusto)
        {
            yaAsusto = true;

            // Dispara tu mega pista de audio mezclada de FL Studio
            if (audioClimax != null) audioClimax.Play();

            // Nota: No apagamos el audioAmbiente para que se quede el silencio incómodo después del ataque
        }
    }
}