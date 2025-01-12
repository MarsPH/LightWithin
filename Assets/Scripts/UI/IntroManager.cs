using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup blackScreen; // Black background (fade in/out)
    public TextMeshProUGUI introText; // Text element to display messages
    public float textFadeDuration = 1.0f; // Fade-in/out duration for text
    public float textDisplayDuration = 2.0f; // Time each text stays visible
    public string[] introMessages; // Array of messages to display
    public string mainSceneName = "MainScene"; // The scene to load after the intro

    private void Start()
    {
        // Ensure black screen is fully opaque and text is hidden initially
        blackScreen.alpha = 1f;
        introText.text = "";
        introText.GetComponent<CanvasGroup>().alpha = 0f;

        // Start the intro sequence
        StartCoroutine(PlayIntroSequence());
    }

    private IEnumerator PlayIntroSequence()
    {
        // Fade in the first message without fading out the black screen yet
        foreach (string message in introMessages)
        {
            yield return StartCoroutine(DisplayMessageWithBlackScreen(message));
        }

        // Keep the black screen fully visible before loading the scene
        blackScreen.alpha = 1f;

        // Load the next scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(mainSceneName);
        asyncLoad.allowSceneActivation = false; // Prevent the scene from activating immediately

        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                // Allow the scene to activate once it’s ready
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        // Fade out the black screen after the new scene is loaded
        yield return StartCoroutine(FadeCanvasGroup(blackScreen, 1, 0, textFadeDuration));
    }

    private IEnumerator DisplayMessageWithBlackScreen(string message)
    {
        // Display the new message
        introText.text = message;

        // Fade in the text while keeping the black screen
        yield return StartCoroutine(FadeCanvasGroup(introText.GetComponent<CanvasGroup>(), 0, 1, textFadeDuration));

        // Wait for the display duration
        yield return new WaitForSeconds(textDisplayDuration);

        // Fade out the text
        yield return StartCoroutine(FadeCanvasGroup(introText.GetComponent<CanvasGroup>(), 1, 0, textFadeDuration));
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = endAlpha;
    }
}
