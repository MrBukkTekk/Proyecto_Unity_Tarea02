using UnityEngine;
using UnityEngine.SceneManagement;


public class WarningScreenController : MonoBehaviour
{
    [Tooltip("Escena a la que se pasa al aceptar la advertencia (normalmente el Menú Inicial).")]
    public string nextSceneName = "MainMenu";

    public void OnAccept()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    public void OnDecline()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}