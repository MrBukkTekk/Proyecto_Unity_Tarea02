using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;

namespace SalaDeLaConsola
{
    // Un acertijo individual. Se configura desde el Inspector, sin necesidad
    // de assets finales: solo texto.
    [System.Serializable]
    public class Acertijo
    {
        [TextArea(2, 5)] public string textoPregunta;
        public string respuestaEsperada;   // se compara sin mayúsculas ni espacios extra
        public bool esAcertijoFinal;       // marca el "detonador": true solo en el último
    }

    public class ConsolePuzzleController : MonoBehaviour
    {
        [Header("Referencias UI (placeholders: texto plano + input, sin arte final)")]
        public TMP_Text textoConsola;
        public TMP_InputField campoRespuesta;
        public GameObject luzVerde;
        public GameObject luzRoja;

        [Header("Contenido de los acertijos, en orden")]
        public Acertijo[] acertijos;

        [Header("Eventos - el equipo de arte/sonido conecta acá cuando haya assets")]
        public UnityEvent onRespuestaCorrecta;    // luz verde + sonido de acierto
        public UnityEvent onRespuestaIncorrecta;  // luz roja + sonido de error
        public UnityEvent onSecuenciaFinalizada;  // "LIBERANDO SUJETADORES" -> dispara la trampa
        public UnityEvent onGameOver;             // congela la consola tras la ejecución

        private int indiceActual = 0;
        private bool consolaActiva = true;

        private void Start()
        {
            MostrarAcertijoActual();
        }

        // Conectar este método al botón "ENTER" / OnEndEdit del InputField
        public void OnSubmitPresionado()
        {
            if (!consolaActiva || acertijos == null || indiceActual >= acertijos.Length) return;

            string respuestaJugador = NormalizarTexto(campoRespuesta.text);
            string respuestaCorrecta = NormalizarTexto(acertijos[indiceActual].respuestaEsperada);
            bool esCorrecta = respuestaJugador == respuestaCorrecta;

            campoRespuesta.text = "";

            if (!esCorrecta)
            {
                onRespuestaIncorrecta?.Invoke();
                if (luzRoja) luzRoja.SetActive(true);
                return;
            }

            // Punto clave del diseño: en el acertijo final, la respuesta
            // "lógicamente correcta" NO dispara la victoria, dispara la trampa.
            if (acertijos[indiceActual].esAcertijoFinal)
            {
                CongelarConsola();
                if (luzVerde) luzVerde.SetActive(true);
                onSecuenciaFinalizada?.Invoke(); // -> engancha acá la cinemática de la silla
                return;
            }

            onRespuestaCorrecta?.Invoke();
            if (luzVerde) luzVerde.SetActive(true);
            indiceActual++;
            MostrarAcertijoActual();
        }

        private void MostrarAcertijoActual()
        {
            if (acertijos == null || indiceActual >= acertijos.Length) return;
            if (textoConsola) textoConsola.text = acertijos[indiceActual].textoPregunta;
            if (luzVerde) luzVerde.SetActive(false);
            if (luzRoja) luzRoja.SetActive(false);
        }

        private string NormalizarTexto(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Trim().ToUpperInvariant();
        }

        // Llamar desde la cinemática cuando el Autómata termina y la cámara cae
        public void CongelarConsola()
        {
            consolaActiva = false;
            onGameOver?.Invoke();
            FinalizarMinijuegoYRegresar();
        }

        // Añade el using arriba del todo si no lo tienes:
        // using UnityEngine.SceneManagement;

        public void FinalizarMinijuegoYRegresar()
        {
            // 1. Le avisamos a la mochila que el minijuego terminó
            if (GameManager.Instance != null)
            {
                GameManager.Instance.pointAndClickCompletado = true;

                // 2. Cargamos de vuelta la escena de la que veníamos automáticamente
                UnityEngine.SceneManagement.SceneManager.LoadScene(GameManager.Instance.escenaActual);
            }
            else
            {
                // Fallback por si corres la escena solita para hacer pruebas
                UnityEngine.SceneManagement.SceneManager.LoadScene("Historia2");
            }
        }
    }
}
