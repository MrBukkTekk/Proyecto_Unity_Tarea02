using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // El Singleton: asegura que solo exista una instancia de esta "mochila" en todo el juego
    public static GameManager Instance;

    [Header("Datos Globales de Partida")]
    public int currentSlot = 1;
    public string escenaActual = "Historia1";

    [Header("Datos de la Escena Actual")]
    public int vnStepIndex = 0; // Índice del diálogo en el que nos quedamos
    public bool pointAndClickCompletado = false; // Controla si ya se resolvió la restauración de la antigüedad u otro acertijo

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destruye los clones si regresas a una escena que ya tenía un GameManager
            Destroy(gameObject);
        }
    }

    // --- TRANSICIONES DE HISTORIA ---

    // Llama a este método cuando termines un capítulo y necesites cargar el siguiente
    public void CambiarDeHistoria(string nombreNuevaEscena)
    {
        escenaActual = nombreNuevaEscena;
        vnStepIndex = 0; // Reinicia el diálogo para el nuevo capítulo
        pointAndClickCompletado = false; // Resetea el estado de los minijuegos
        SceneManager.LoadScene(nombreNuevaEscena);
    }

    // --- SISTEMA DE GUARDADO Y CARGA (PLAYERPREFS) ---

    // Conecta esto al botón de "Guardar" o llámalo automáticamente al avanzar en la novela
    public void GuardarPartida(int slot)
    {
        currentSlot = slot;
        PlayerPrefs.SetString("Slot" + slot + "_Escena", escenaActual);
        PlayerPrefs.SetInt("Slot" + slot + "_PasoVN", vnStepIndex);

        // PlayerPrefs no guarda booleanos directamente, usamos 1 para true y 0 para false
        PlayerPrefs.SetInt("Slot" + slot + "_PyC", pointAndClickCompletado ? 1 : 0);

        PlayerPrefs.Save();
        Debug.Log($"Partida guardada en el Slot {slot} | Escena: {escenaActual} | Diálogo: {vnStepIndex}");
    }

    // Conecta esto a los botones (Slot 1, Slot 2, Slot 3) de tu panel de partidas en el Menú
    public void CargarPartida(int slot)
    {
        if (PlayerPrefs.HasKey("Slot" + slot + "_Escena"))
        {
            currentSlot = slot;
            escenaActual = PlayerPrefs.GetString("Slot" + slot + "_Escena");
            vnStepIndex = PlayerPrefs.GetInt("Slot" + slot + "_PasoVN");

            pointAndClickCompletado = PlayerPrefs.GetInt("Slot" + slot + "_PyC") == 1;

            Debug.Log($"Cargando Slot {slot} | Escena: {escenaActual}");
            SceneManager.LoadScene(escenaActual);
        }
        else
        {
            Debug.LogWarning("No hay datos guardados en el Slot " + slot);
            // Aquí puedes iniciar una partida nueva desde cero cargando "Historia1"
            CambiarDeHistoria("Nexo");
        }
    }
}