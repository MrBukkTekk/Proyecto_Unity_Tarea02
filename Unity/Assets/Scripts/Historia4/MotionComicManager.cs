using UnityEngine;

public class MotionComicManager : MonoBehaviour
{
    [Header("Conexiones Visuales")]
    public Animator animadorEscena;

    [Header("Conexiones de Audio")]
    public AudioSource reproductorEfectos;
    public AudioClip[] audiosPorToma;

    private int tomaActual = 0;

    public void AvanzarToma()
    {
        tomaActual++;

        // 1. Le dice a la animación que cambie
        if (animadorEscena != null)
        {
            animadorEscena.SetInteger("NumeroToma", tomaActual);
        }

        // 2. Reproduce el audio de esa toma
        if (audiosPorToma.Length >= tomaActual)
        {
            AudioClip audioAsonar = audiosPorToma[tomaActual - 1];
            if (audioAsonar != null)
            {
                reproductorEfectos.PlayOneShot(audioAsonar);
            }
        }
    }
}
