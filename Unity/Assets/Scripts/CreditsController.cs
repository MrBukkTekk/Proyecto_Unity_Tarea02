using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour
{
    [Header("Scroll (opcional)")]
    public RectTransform creditsContent;
    public float scrollSpeed = 40f;
    public bool autoScroll = true;

    [Header("Navegación")]
    public string mainMenuSceneName = "MainMenu";

    void Update()
    {
        if (autoScroll && creditsContent != null)
        {
            creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}