using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCharacterSwitcher : MonoBehaviour
{
    public GameObject topDownCharacter;    // 2D top-down character
    public Camera topDownCamera;           // Camera for the 2D character

    public MonoBehaviour[] topDownMovementScripts;     // Movement scripts for the 2D character

    public Image transitionPanel;          // Reference to the black UI panel
    public float transitionDuration = 1f;  // Time for the fade effect

    private bool isTransitioning = false;

    void Start()
    {
        ActivateTopDown(true);
        SetTransitionAlpha(0); // Ensure the panel is invisible at the start
    }

    void Update()
    {
        // No character switching logic is needed, as we're focused on 2D
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / transitionDuration);
            SetTransitionAlpha(alpha);
            yield return null;
        }
    }

    private IEnumerator FadeFromBlack()
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1 - Mathf.Clamp01(elapsedTime / transitionDuration);
            SetTransitionAlpha(alpha);
            yield return null;
        }
    }

    private void SetTransitionAlpha(float alpha)
    {
        Color color = transitionPanel.color;
        color.a = alpha;
        transitionPanel.color = color;
    }

    private void ActivateTopDown(bool immediate)
    {
        // Enable movement scripts for the 2D character
        SetMovementScriptsActive(topDownMovementScripts, true);

        // Activate the character and its camera
        topDownCharacter.SetActive(true);

        if (immediate)
        {
            topDownCamera.gameObject.SetActive(true);
        }
    }

    private void SetMovementScriptsActive(MonoBehaviour[] scripts, bool isActive)
    {
        foreach (var script in scripts)
        {
            script.enabled = isActive;
        }
    }
}
