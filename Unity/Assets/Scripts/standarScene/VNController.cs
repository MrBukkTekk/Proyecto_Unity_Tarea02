using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class VNController : MonoBehaviour
{
    [Header("Guion")]
    public VNSceneData currentScene;

    [Header("Referencias UI")]
    public Image backgroundImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialogueBox; // Para poder ocultarlo entero si hace falta

    [Header("Transición")]
    public string siguienteEscena; // Aquí escribirás "Historia 4", "Historia 5", etc.

    private int currentStepIndex = 0;

    void Start()
    {
        // 1. REVISAR LA MOCHILA: ¿Veníamos regresando de un Point & Click?
        if (GameManager.Instance != null)
        {
            currentStepIndex = GameManager.Instance.vnStepIndex;
        }

        if (currentScene != null && currentScene.steps.Count > 0)
        {
            ShowStep(currentStepIndex);
        }
    }

    private void ShowStep(int index)
    {
        VNStep step = currentScene.steps[index];

        if (step.artwork != null) backgroundImage.sprite = step.artwork;
        dialogueText.text = step.dialogueText;
        nameText.text = step.speakerName;
        nameText.gameObject.SetActive(!string.IsNullOrEmpty(step.speakerName));

        // 2. DETECTAR EL SALTO AL POINT & CLICK
        if (!string.IsNullOrEmpty(step.triggerEvent))
        {
            // OJO: "iniciar_pnc" es el texto exacto que debes escribir en el Inspector de Unity
            if (step.triggerEvent == "iniciar_pnc")
            {
                if (GameManager.Instance != null)
                {
                    // Guardamos el índice de la SIGUIENTE línea para no repetir diálogo al volver
                    GameManager.Instance.vnStepIndex = currentStepIndex + 1;
                }

                // Saltamos a la escena del minijuego (escribe el nombre exacto de tu escena)
                UnityEngine.SceneManagement.SceneManager.LoadScene("Historia2_P&C");
            }
        }
    }
    void Update()
    {
        // Avanza con Espacio, Enter o clic izquierdo del mouse
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
        {
            Advance();
        }
    }

    public void Advance()
    {
        currentStepIndex++;

        if (currentStepIndex < currentScene.steps.Count)
        {
            ShowStep(currentStepIndex);
        }
        else
        {
            Debug.Log("Fin de esta escena/historia.");
            // Fin de esta escena, saltamos a la siguiente
            if (GameManager.Instance != null && !string.IsNullOrEmpty(siguienteEscena))
            {
                NexoState.modoSalida = true;
                GameManager.Instance.CambiarDeHistoria(siguienteEscena);
            }
        }
    }
}