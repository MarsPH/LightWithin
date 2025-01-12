using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    [Header("Game Over UI")]
    public GameObject gameOverPanel; // Panel containing the buttons
    public TextMeshProUGUI gameOverText; // Text for "Game Over"
    public Image fadeImage; // Full-screen Image for fading effect (UI Image)

    [Header("Game Over Camera")]
    public Camera gameOverCamera; // Camera for Game Over view
    public GameObject lamp; // Lamp for Game Over lighting

    [Header("Transition Settings")]
    public float fadeDuration = 2f; // Duration of the fade effect
    public float textDelay = 1f; // Delay before showing "Game Over" text

    private bool isGameOver = false;

    void Start()
    {
        // Ensure UI and camera are initially disabled
        gameOverPanel.SetActive(false);
        gameOverCamera.gameObject.SetActive(false);

        // Ensure fade image starts fully transparent
        fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Disable all active cameras
        Camera[] cameras = FindObjectsOfType<Camera>();
        foreach (Camera cam in cameras)
        {
            cam.gameObject.SetActive(false);
        }

        // Begin transition effect
        StartCoroutine(GameOverTransition());
    }

    private IEnumerator GameOverTransition()
    {
        // Gradually fade the screen to black
        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (elapsedTime < fadeDuration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        fadeImage.color = endColor;

        // Show "Game Over" text after a delay
        yield return new WaitForSecondsRealtime(textDelay);
        gameOverText.gameObject.SetActive(true);
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // Enable Game Over camera
        gameOverCamera.gameObject.SetActive(true);

        // Enable lamp
        if (lamp != null)
        {
            lamp.SetActive(true);
        }

        // Show Game Over panel with buttons
        gameOverPanel.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    public void RetryLevel()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMenu()
    {
        // Reset time scale
        Time.timeScale = 1f;

        // Load the main menu scene (replace "MainMenu" with your menu scene's name)
        SceneManager.LoadScene("MenuScene");
    }
}
