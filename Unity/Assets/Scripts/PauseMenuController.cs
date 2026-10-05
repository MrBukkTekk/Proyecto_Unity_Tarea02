using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject pausePanel; // el Canvas/Panel de pausa, debe empezar desactivado
    public KeyCode pauseKey = KeyCode.Escape;

    [Header("Escenas")]
    public string mainMenuSceneName = "MainMenu";

    bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused) Resume();
            else Pause();
        }
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        AudioListener.pause = true;
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        AudioListener.pause = false;
        if (pausePanel) pausePanel.SetActive(false);
    }

    // Conecta este método al botón "Reintentar capítulo"
    public void RestartChapter()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Conecta este método al botón "Menú principal"
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Conecta este método al botón "Salir"
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}