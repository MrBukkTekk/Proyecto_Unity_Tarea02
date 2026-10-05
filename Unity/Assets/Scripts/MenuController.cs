using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("Paneles del Menú")]
    public GameObject panelPrincipal;
    public GameObject panelPartidas;
    public GameObject panelAjustes;

    [Header("Ajustes - Audio")]
    public Slider sliderVolumen;
    public TMP_Text textoValorVolumen; // muestra "75%"

    [Header("Ajustes - Novela Visual")]
    public Slider sliderVelocidadTexto;
    public TMP_Text textoValorVelocidadTexto; // muestra "50%"

    public Toggle toggleAutoAvance;      // activa/desactiva el auto-avance
    public Slider sliderAutoAvance;      // segundos de espera
    public TMP_Text textoValorAutoAvance; // muestra "2.0s"

    [Header("Ajustes - Video")]
    public Toggle togglePantallaCompleta;

    private void Start()
    {
        MostrarPanelPrincipal();

        // --- Volumen ---
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenGeneral", 0.75f);
        AudioListener.volume = volumenGuardado;
        if (sliderVolumen != null) sliderVolumen.value = volumenGuardado;
        ActualizarTextoVolumen(volumenGuardado);

        // --- Velocidad de Texto ---
        float velocidadGuardada = PlayerPrefs.GetFloat("VelocidadTexto", 0.5f);
        if (sliderVelocidadTexto != null) sliderVelocidadTexto.value = velocidadGuardada;
        ActualizarTextoVelocidadTexto(velocidadGuardada);

        // --- Auto-Avance (on/off) ---
        bool autoAvanceActivo = PlayerPrefs.GetInt("AutoAvanceActivo", 1) == 1;
        if (toggleAutoAvance != null) toggleAutoAvance.isOn = autoAvanceActivo;
        if (sliderAutoAvance != null) sliderAutoAvance.interactable = autoAvanceActivo;

        float autoAvanceGuardado = PlayerPrefs.GetFloat("AutoAvance", 2f);
        if (sliderAutoAvance != null) sliderAutoAvance.value = autoAvanceGuardado;
        ActualizarTextoAutoAvance(autoAvanceGuardado);

        // --- Pantalla Completa ---
        if (togglePantallaCompleta != null) togglePantallaCompleta.isOn = Screen.fullScreen;
    }

    // --- NAVEGACIÓN DE PANELES ---
    public void MostrarPanelPrincipal()
    {
        panelPrincipal.SetActive(true);
        panelPartidas.SetActive(false);
        panelAjustes.SetActive(false);
    }

    public void MostrarPanelPartidas()
    {
        panelPrincipal.SetActive(false);
        panelPartidas.SetActive(true);
        panelAjustes.SetActive(false);
    }

    public void MostrarPanelAjustes()
    {
        panelPrincipal.SetActive(false);
        panelPartidas.SetActive(false);
        panelAjustes.SetActive(true);
    }

    // --- ACCIONES DE BOTONES ---
    public void CargarPartida(int numeroDeSlot)
    {
        Debug.Log($"Cargando la partida del Slot {numeroDeSlot}...");
        GameManager.Instance.CargarPartida(numeroDeSlot);
    }

    // --- VOLUMEN ---
    public void CambiarVolumen(float valor)
    {
        AudioListener.volume = valor;
        PlayerPrefs.SetFloat("VolumenGeneral", valor);
        ActualizarTextoVolumen(valor);
    }

    void ActualizarTextoVolumen(float valor)
    {
        if (textoValorVolumen != null)
            textoValorVolumen.text = Mathf.RoundToInt(valor * 100f) + "%";
    }

    // --- VELOCIDAD DE TEXTO ---
    public void CambiarVelocidadTexto(float valor)
    {
        PlayerPrefs.SetFloat("VelocidadTexto", valor);
        ActualizarTextoVelocidadTexto(valor);
    }

    void ActualizarTextoVelocidadTexto(float valor)
    {
        if (textoValorVelocidadTexto != null)
            textoValorVelocidadTexto.text = Mathf.RoundToInt(valor * 100f) + "%";
    }

    // --- AUTO-AVANCE ---
    // Conecta esto al Toggle "Activar Auto-Avance"
    public void ActivarAutoAvance(bool activo)
    {
        PlayerPrefs.SetInt("AutoAvanceActivo", activo ? 1 : 0);
        if (sliderAutoAvance != null) sliderAutoAvance.interactable = activo;
    }

    // Conecta esto al slider de segundos de Auto-Avance
    public void CambiarAutoAvance(float valor)
    {
        PlayerPrefs.SetFloat("AutoAvance", valor);
        ActualizarTextoAutoAvance(valor);
    }

    void ActualizarTextoAutoAvance(float valor)
    {
        if (textoValorAutoAvance != null)
            textoValorAutoAvance.text = valor.ToString("0.0") + "s";
    }

    // --- PANTALLA COMPLETA ---
    public void CambiarPantallaCompleta(bool activo)
    {
        Screen.fullScreen = activo;
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}