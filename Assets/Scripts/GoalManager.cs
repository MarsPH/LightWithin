using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TextMeshPro
using System.Collections;
using UnityEngine.SceneManagement;

public class GoalManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup whiteOverlay; // White screen overlay for the glow effect
    public TextMeshProUGUI subtitleText; // Text element for subtitles
    public string[] subtitles; // Subtitles to display
    public float subtitleFadeDuration = 1.0f; // Time for subtitles to fade in/out
    public float subtitleDisplayDuration = 2.0f; // How long each subtitle stays visible
    public float glowIncreaseDuration = 5.0f; // Duration to increase glow
    public string mainMenuSceneName = "MainMenu"; // Scene to return to after the sequence

    private bool goalReached = false; // Ensure the sequence only happens once

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player character triggered the goal
        if (other.gameObject.CompareTag("Player") && !goalReached)
        {
            goalReached = true;
            StartCoroutine(PlayEndSequence());
        }
    }

    private IEnumerator PlayEndSequence()
    {
        // Ensure the white overlay is fully transparent at the start
        whiteOverlay.alpha = 0f;

        // Loop through the subtitles
        foreach (string subtitle in subtitles)
        {
            yield return StartCoroutine(DisplaySubtitle(subtitle));
        }

        // Gradually brighten the screen (glow effect)
        yield return StartCoroutine(IncreaseGlow());

        // Load the main menu scene
        Application.Quit();
    }

    private IEnumerator DisplaySubtitle(string subtitle)
    {
        // Set the subtitle text
        subtitleText.text = subtitle;

        // Fade in the subtitle
        yield return StartCoroutine(FadeCanvasGroup(subtitleText.GetComponent<CanvasGroup>(), 0, 1, subtitleFadeDuration));

        // Wait for the display duration
        yield return new WaitForSeconds(subtitleDisplayDuration);

        // Fade out the subtitle
        yield return StartCoroutine(FadeCanvasGroup(subtitleText.GetComponent<CanvasGroup>(), 1, 0, subtitleFadeDuration));
    }

    private IEnumerator IncreaseGlow()
    {
        float elapsedTime = 0f;

        while (elapsedTime < glowIncreaseDuration)
        {
            whiteOverlay.alpha = Mathf.Lerp(0, 1, elapsedTime / glowIncreaseDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the overlay is fully white
        whiteOverlay.alpha = 1f;
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
