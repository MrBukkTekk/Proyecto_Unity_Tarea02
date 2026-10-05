using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

// Guarda en qué estación (historia) vamos y si venimos de "terminar una historia".
// Al ser estático, sobrevive entre cargas de escena dentro de la misma sesión de juego.
public static class NexoState
{
    public static int estacionActual = 0; // 0=Tablet, 1=Cartucho, 2=Libro, 3=Expediente
    public static bool modoSalida = false;
}

// Colócalo en un GameObject vacío llamado "NexoController" dentro de la escena Nexo.
public class NexoController : MonoBehaviour
{
    [Header("Mesa / Zoom")]
    public RectTransform mesaGroup;
    public float escalaZoomIn = 2.5f;
    public float duracionZoom = 1f;

    [Header("Escenas de cada Historia (en orden: Tablet, Cartucho, Libro, Expediente)")]
    public string[] escenasHistorias = { "Historia1", "Historia2", "Historia3", "Historia4" };
    [Tooltip("A dónde ir cuando ya no queda ninguna historia más por jugar (tu escena del Capítulo 5).")]
    public string escenaFinal = "Capitulo5";

    // ==================== ESTACIÓN 0: TABLET ====================
    [Header("--- Estación 0: Tablet ---")]
    public GameObject grupoTablet;
    public RectTransform tabletObject;
    public Vector2 tabletPosicionFuera = new Vector2(0f, -1200f);
    public Vector2 tabletPosicionMesa = Vector2.zero;
    public Image pantallaTabletImage;
    public Button botonPantallaTablet;
    public Sprite spriteUltimaVinetaTablet;
    public Button botonApagadoTablet;

    // ==================== ESTACIÓN 1: CARTUCHO ====================
    [Header("--- Estación 1: Cartucho ---")]
    public GameObject grupoCartucho;
    public RectTransform cartuchoObject;
    public Vector2 cartuchoPosicionFuera = new Vector2(0f, 800f);
    public RectTransform ranuraCartucho;
    public CartuchoDraggable cartuchoDraggable;
    public TMP_Text terminalPantallaTexto;
    public Button botonRojo;
    public Image botonRojoImagen;

    // ==================== ESTACIÓN 2: LIBRO ====================
    [Header("--- Estación 2: Libro ---")]
    public GameObject grupoLibro;
    public Button botonPortadaLibro;
    public Image paginaLibroImage;               // muestra la página / ilustración
    public Sprite spriteIlustracionFinalLibro;    // holder: ilustración final (gore) para la salida
    public ArrastreDireccional paginaDerechaDraggable; // arrastrar hacia la izquierda para cerrar

    // ==================== ESTACIÓN 3: EXPEDIENTE ====================
    [Header("--- Estación 3: Expediente ---")]
    public GameObject grupoExpediente;
    public RectTransform expedienteObject;
    public Vector2 expedientePosicionFuera = new Vector2(0f, 800f);
    public Button botonTapaExpediente;
    public Image reporteImage;                    // muestra fotos / reporte
    public Sprite spriteReporteFinal;             // holder: reporte final para la salida
    public ArrastreDireccional expedienteDraggable; // arrastrar hacia arriba para quemarlo
    public Image luzFuegoImage;                   // opcional: resplandor naranja parpadeante

    [Header("Configuración general")]
    public float velocidadDeslizamiento = 1f;

    [Header("--- SOLO PRUEBAS ---")]
    [Tooltip("-1 = desactivado. 0=Tablet, 1=Cartucho, 2=Libro, 3=Expediente. Da Play directo en la escena Nexo para que esto tenga efecto.")]
    public int estacionDePrueba = -1;
    public bool modoSalidaDePrueba = false;

    bool botonRojoParpadeando = false;
    bool luzFuegoActiva = false;

    void Start()
    {
        if (botonPantallaTablet != null) botonPantallaTablet.onClick.AddListener(OnClicPantallaTablet);
        if (botonApagadoTablet != null) botonApagadoTablet.onClick.AddListener(OnClicBotonApagadoTablet);
        if (botonRojo != null) botonRojo.onClick.AddListener(OnClicBotonRojo);
        if (botonPortadaLibro != null) botonPortadaLibro.onClick.AddListener(OnClicPortadaLibro);
        if (botonTapaExpediente != null) botonTapaExpediente.onClick.AddListener(OnClicTapaExpediente);

        if (estacionDePrueba >= 0)
        {
            NexoState.estacionActual = estacionDePrueba;
            NexoState.modoSalida = modoSalidaDePrueba;
        }

        mesaGroup.localScale = Vector3.one;

        if (NexoState.modoSalida)
            IniciarSalidaEstacionActual();
        else
            IniciarEntradaEstacionActual();
    }

    void MostrarSoloGrupo(int estacion)
    {
        if (grupoTablet != null) grupoTablet.SetActive(estacion == 0);
        if (grupoCartucho != null) grupoCartucho.SetActive(estacion == 1);
        if (grupoLibro != null) grupoLibro.SetActive(estacion == 2);
        if (grupoExpediente != null) grupoExpediente.SetActive(estacion == 3);
    }

    void IniciarEntradaEstacionActual()
    {
        MostrarSoloGrupo(NexoState.estacionActual);
        mesaGroup.localScale = Vector3.one;

        switch (NexoState.estacionActual)
        {
            case 0: StartCoroutine(EntradaTablet()); break;
            case 1: StartCoroutine(EntradaCartucho()); break;
            case 2: StartCoroutine(EntradaLibro()); break;
            case 3: StartCoroutine(EntradaExpediente()); break;
        }
    }

    void IniciarSalidaEstacionActual()
    {
        MostrarSoloGrupo(NexoState.estacionActual);
        mesaGroup.localScale = new Vector3(escalaZoomIn, escalaZoomIn, 1f);

        switch (NexoState.estacionActual)
        {
            case 0: StartCoroutine(SalidaTablet()); break;
            case 1: StartCoroutine(SalidaCartucho()); break;
            case 2: StartCoroutine(SalidaLibro()); break;
            case 3: StartCoroutine(SalidaExpediente()); break;
        }
    }

    void AvanzarASiguienteEstacion()
    {
        NexoState.estacionActual++;
        NexoState.modoSalida = false;

        if (NexoState.estacionActual < escenasHistorias.Length)
            IniciarEntradaEstacionActual();
        else
            SceneManager.LoadScene(escenaFinal);
    }

    void CargarHistoriaActual()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.escenaActual = escenasHistorias[NexoState.estacionActual];

        SceneManager.LoadScene(escenasHistorias[NexoState.estacionActual]);
    }

    IEnumerator Zoom(float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float escala = Mathf.Lerp(from, to, t / duration);
            mesaGroup.localScale = new Vector3(escala, escala, 1f);
            yield return null;
        }
        mesaGroup.localScale = new Vector3(to, to, 1f);
    }

    IEnumerator DeslizarA(RectTransform obj, Vector2 desde, Vector2 hasta, float duracion)
    {
        obj.anchoredPosition = desde;
        float t = 0f;
        while (t < duracion)
        {
            t += Time.deltaTime;
            obj.anchoredPosition = Vector2.Lerp(desde, hasta, t / duracion);
            yield return null;
        }
        obj.anchoredPosition = hasta;
    }

    // Punto de entrada común para los arrastres tipo "swipe" (ArrastreDireccional)
    public void OnArrastreCompletado(string idAccion)
    {
        switch (idAccion)
        {
            case "cerrarLibro": StartCoroutine(CerrarLibroCoroutine()); break;
            case "quemarExpediente": StartCoroutine(QuemarExpedienteCoroutine()); break;
        }
    }

    // ==================== TABLET: ENTRADA ====================
    IEnumerator EntradaTablet()
    {
        if (botonApagadoTablet != null) botonApagadoTablet.gameObject.SetActive(false);
        pantallaTabletImage.color = Color.black;
        if (botonPantallaTablet != null) botonPantallaTablet.interactable = false;

        yield return DeslizarA(tabletObject, tabletPosicionFuera, tabletPosicionMesa, velocidadDeslizamiento);

        if (botonPantallaTablet != null) botonPantallaTablet.interactable = true;
    }

    void OnClicPantallaTablet()
    {
        botonPantallaTablet.interactable = false;
        StartCoroutine(ParpadeoYZoomInTablet());
    }

    IEnumerator ParpadeoYZoomInTablet()
    {
        for (int i = 0; i < 3; i++)
        {
            pantallaTabletImage.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            pantallaTabletImage.color = Color.black;
            yield return new WaitForSeconds(0.1f);
        }
        pantallaTabletImage.color = Color.white;

        yield return Zoom(1f, escalaZoomIn, duracionZoom);
        CargarHistoriaActual();
    }

    // ==================== TABLET: SALIDA ====================
    IEnumerator SalidaTablet()
    {
        if (botonPantallaTablet != null) botonPantallaTablet.gameObject.SetActive(false);
        if (botonApagadoTablet != null)
        {
            botonApagadoTablet.gameObject.SetActive(true);
            botonApagadoTablet.interactable = false;
        }
        tabletObject.anchoredPosition = tabletPosicionMesa;
        if (spriteUltimaVinetaTablet != null)
        {
            pantallaTabletImage.sprite = spriteUltimaVinetaTablet;
            pantallaTabletImage.color = Color.white;
        }

        yield return Zoom(escalaZoomIn, 1f, duracionZoom);

        if (botonApagadoTablet != null) botonApagadoTablet.interactable = true;
    }

    void OnClicBotonApagadoTablet()
    {
        botonApagadoTablet.interactable = false;
        StartCoroutine(ApagadoCRTYSalida());
    }

    IEnumerator ApagadoCRTYSalida()
    {
        float duracionCRT = 0.3f;
        float t = 0f;
        while (t < duracionCRT)
        {
            t += Time.deltaTime;
            float escalaY = Mathf.Lerp(1f, 0f, t / duracionCRT);
            pantallaTabletImage.rectTransform.localScale = new Vector3(1f, escalaY, 1f);
            yield return null;
        }
        pantallaTabletImage.gameObject.SetActive(false);

        yield return DeslizarA(tabletObject, tabletObject.anchoredPosition, tabletPosicionFuera, velocidadDeslizamiento);

        AvanzarASiguienteEstacion();
    }

    // ==================== CARTUCHO: ENTRADA ====================
    IEnumerator EntradaCartucho()
    {
        botonRojo.gameObject.SetActive(false);
        if (terminalPantallaTexto != null) terminalPantallaTexto.text = "";

        yield return DeslizarA(cartuchoObject, cartuchoPosicionFuera, Vector2.zero, velocidadDeslizamiento);
    }

    public void OnCartuchoEncajado()
    {
        StartCoroutine(ZumbidoYZoomInCartucho());
    }

    IEnumerator ZumbidoYZoomInCartucho()
    {
        if (terminalPantallaTexto != null)
        {
            terminalPantallaTexto.color = Color.green;
            terminalPantallaTexto.text = "...";
        }
        yield return new WaitForSeconds(0.5f);

        yield return Zoom(1f, escalaZoomIn, duracionZoom);
        CargarHistoriaActual();
    }

    // ==================== CARTUCHO: SALIDA ====================
    IEnumerator SalidaCartucho()
    {
        botonRojo.gameObject.SetActive(false);
        cartuchoObject.anchoredPosition = Vector2.zero;

        yield return Zoom(escalaZoomIn, 1f, duracionZoom);

        if (terminalPantallaTexto != null)
        {
            terminalPantallaTexto.color = Color.green;
            string simbolos = "!@#$%&*01";
            float duracionCaracteres = 1f;
            float t = 0f;
            while (t < duracionCaracteres)
            {
                t += Time.deltaTime;
                string linea = "";
                for (int i = 0; i < 40; i++) linea += simbolos[Random.Range(0, simbolos.Length)];
                terminalPantallaTexto.text = linea;
                yield return new WaitForSeconds(0.05f);
            }
            terminalPantallaTexto.text = ":(";
        }

        botonRojo.gameObject.SetActive(true);
        botonRojoParpadeando = true;
        StartCoroutine(ParpadeoBotonRojo());
    }

    IEnumerator ParpadeoBotonRojo()
    {
        while (botonRojoParpadeando)
        {
            botonRojoImagen.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            botonRojoImagen.color = new Color(0.4f, 0f, 0f);
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnClicBotonRojo()
    {
        botonRojoParpadeando = false;
        botonRojo.interactable = false;
        if (terminalPantallaTexto != null) terminalPantallaTexto.text = "";
        AvanzarASiguienteEstacion();
    }

    // ==================== LIBRO: ENTRADA ====================
    IEnumerator EntradaLibro()
    {
        if (paginaDerechaDraggable != null) paginaDerechaDraggable.gameObject.SetActive(false); // solo se usa en la salida
        botonPortadaLibro.gameObject.SetActive(true);
        botonPortadaLibro.interactable = true;
        if (paginaLibroImage != null) paginaLibroImage.sprite = null; // portada cerrada (placeholder de color)
        yield return null; // el libro ya "está colocado" en la mesa, sin animación de llegada
    }

    void OnClicPortadaLibro()
    {
        botonPortadaLibro.interactable = false;
        StartCoroutine(AbrirLibroYZoomIn());
    }

    IEnumerator AbrirLibroYZoomIn()
    {
        // Aquí reproduce tu melodía de caja musical si tienes un AudioSource asignado
        if (paginaLibroImage != null) paginaLibroImage.color = Color.white; // holder: "libro abierto"

        yield return Zoom(1f, escalaZoomIn, duracionZoom);
        // La música se corta abruptamente aquí (audioSource.Stop() si tienes una)

        CargarHistoriaActual();
    }

    // ==================== LIBRO: SALIDA ====================
    IEnumerator SalidaLibro()
    {
        botonPortadaLibro.gameObject.SetActive(false);
        if (paginaDerechaDraggable != null)
        {
            paginaDerechaDraggable.gameObject.SetActive(true);
            paginaDerechaDraggable.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        if (spriteIlustracionFinalLibro != null)
        {
            paginaLibroImage.sprite = spriteIlustracionFinalLibro;
            paginaLibroImage.color = Color.white;
        }

        yield return Zoom(escalaZoomIn, 1f, duracionZoom);
        // A partir de aquí espera que el jugador arrastre la página hacia la izquierda
    }

    IEnumerator CerrarLibroCoroutine()
    {
        float duracionCierre = 0.3f;
        float t = 0f;
        RectTransform pagina = paginaDerechaDraggable.GetComponent<RectTransform>();
        while (t < duracionCierre)
        {
            t += Time.deltaTime;
            float escalaX = Mathf.Lerp(1f, 0f, t / duracionCierre);
            pagina.localScale = new Vector3(escalaX, 1f, 1f);
            yield return null;
        }
        pagina.gameObject.SetActive(false);
        // Aquí reproduce tu sonido de "impacto pesado de cartón" si tienes un AudioSource asignado

        AvanzarASiguienteEstacion();
    }

    // ==================== EXPEDIENTE: ENTRADA ====================
    IEnumerator EntradaExpediente()
    {
        if (expedienteDraggable != null) expedienteDraggable.gameObject.SetActive(false); // solo se usa en la salida
        botonTapaExpediente.gameObject.SetActive(true);
        botonTapaExpediente.interactable = false;
        if (luzFuegoImage != null) luzFuegoImage.gameObject.SetActive(false);

        // "Arrojada, golpeando la madera": cae rápido desde arriba
        yield return DeslizarA(expedienteObject, expedientePosicionFuera, Vector2.zero, velocidadDeslizamiento * 0.5f);

        botonTapaExpediente.interactable = true;
    }

    void OnClicTapaExpediente()
    {
        botonTapaExpediente.interactable = false;
        StartCoroutine(AbrirExpedienteYZoomIn());
    }

    IEnumerator AbrirExpedienteYZoomIn()
    {
        if (reporteImage != null) reporteImage.color = Color.white; // holder: primera fotografía
        yield return Zoom(1f, escalaZoomIn, duracionZoom);
        CargarHistoriaActual();
    }

    // ==================== EXPEDIENTE: SALIDA Y CONEXIÓN AL CAPÍTULO 5 ====================
    IEnumerator SalidaExpediente()
    {
        botonTapaExpediente.gameObject.SetActive(false);
        expedienteObject.anchoredPosition = Vector2.zero;

        if (spriteReporteFinal != null) reporteImage.sprite = spriteReporteFinal;
        if (reporteImage != null) reporteImage.color = Color.white;

        yield return Zoom(escalaZoomIn, 1f, duracionZoom);

        if (expedienteDraggable != null)
        {
            expedienteDraggable.gameObject.SetActive(true);
            expedienteDraggable.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        if (luzFuegoImage != null)
        {
            luzFuegoImage.gameObject.SetActive(true);
            luzFuegoActiva = true;
            StartCoroutine(ParpadeoLuzFuego());
        }
        // A partir de aquí espera que el jugador arrastre el expediente hacia arriba
    }

    IEnumerator ParpadeoLuzFuego()
    {
        while (luzFuegoActiva)
        {
            float alpha = 0.3f + 0.3f * Mathf.PingPong(Time.time * 2f, 1f);
            Color c = luzFuegoImage.color;
            c.a = alpha;
            luzFuegoImage.color = c;
            yield return null;
        }
    }

    IEnumerator QuemarExpedienteCoroutine()
    {
        luzFuegoActiva = false;

        // El papel "cae" al fuego: se desvanece y encoge
        float duracion = 0.4f;
        float t = 0f;
        RectTransform exp = expedienteDraggable.GetComponent<RectTransform>();
        Image expImg = exp.GetComponent<Image>();
        while (t < duracion)
        {
            t += Time.deltaTime;
            float escala = Mathf.Lerp(1f, 0f, t / duracion);
            exp.localScale = new Vector3(escala, escala, 1f);
            if (expImg != null)
            {
                Color c = expImg.color;
                c.a = Mathf.Lerp(1f, 0f, t / duracion);
                expImg.color = c;
            }
            yield return null;
        }

        // Transición especial hacia el Capítulo 5: simulamos el giro de cámara
        // colapsando la mesa verticalmente antes de cargar la escena final.
        yield return ColapsoVerticalMesa(0.4f);

        AvanzarASiguienteEstacion(); // como es la última estación, esto carga 'escenaFinal'
    }

    IEnumerator ColapsoVerticalMesa(float duracion)
    {
        float t = 0f;
        Vector3 escalaInicial = mesaGroup.localScale;
        while (t < duracion)
        {
            t += Time.deltaTime;
            float escalaY = Mathf.Lerp(1f, 0f, t / duracion);
            mesaGroup.localScale = new Vector3(escalaInicial.x, escalaY, 1f);
            yield return null;
        }
    }
}