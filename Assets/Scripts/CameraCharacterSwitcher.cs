using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraCharacterSwitcher : MonoBehaviour
{
   public GameObject firstPersonCharacter; // 3D first-person character
    public GameObject topDownCharacter;    // 2D top-down character

    public Camera firstPersonCamera;       // Camera for the 3D character
    public Camera topDownCamera;           // Camera for the 2D character

    public MonoBehaviour[] firstPersonMovementScripts; // Movement scripts for the 3D character
    public MonoBehaviour[] topDownMovementScripts;     // Movement scripts for the 2D character

    public Image transitionPanel;          // Reference to the black UI panel
    public float transitionDuration = 1f;  // Time for the fade effect

    private bool isUsingFirstPerson = true; // Tracks which character is active
    private bool isTransitioning = false;

    void Start()
    {
        // Ensure the first-person setup is active at the start
        ActivateFirstPerson(true);
        SetTransitionAlpha(0); // Ensure the panel is invisible at the start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isTransitioning)
        {
            if (isUsingFirstPerson)
            {
                StartCoroutine(SwitchToTopDown());
            }
            else
            {
                StartCoroutine(SwitchToFirstPerson());
            }
        }
    }

    private IEnumerator SwitchToTopDown()
    {
        isTransitioning = true;

        // Fade out
        yield return FadeToBlack();

        // Switch the active character and camera
        ActivateTopDown(true);

        // Fade in
        yield return FadeFromBlack();

        isTransitioning = false;
    }

    private IEnumerator SwitchToFirstPerson()
    {
        isTransitioning = true;

        // Fade out
        yield return FadeToBlack();

        // Switch the active character and camera
        ActivateFirstPerson(true);

        // Fade in
        yield return FadeFromBlack();

        isTransitioning = false;
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

    private void ActivateFirstPerson(bool immediate)
    {
        isUsingFirstPerson = true;

        // Enable/Disable movement scripts
        SetMovementScriptsActive(firstPersonMovementScripts, true);
        SetMovementScriptsActive(topDownMovementScripts, false);

        // Keep both characters visible
        firstPersonCharacter.SetActive(true);
        topDownCharacter.SetActive(true);

        // Activate/Deactivate cameras
        if (immediate)
        {
            firstPersonCamera.gameObject.SetActive(true); // Enable the first-person camera GameObject
            topDownCamera.gameObject.SetActive(false);   // Disable the top-down camera GameObject
        }
    }

    private void ActivateTopDown(bool immediate)
    {
        isUsingFirstPerson = false;

        // Enable/Disable movement scripts
        SetMovementScriptsActive(firstPersonMovementScripts, false);
        SetMovementScriptsActive(topDownMovementScripts, true);

        // Keep both characters visible
        firstPersonCharacter.SetActive(true);
        topDownCharacter.SetActive(true);

        // Activate/Deactivate cameras
        if (immediate)
        {
            firstPersonCamera.gameObject.SetActive(false); // Disable the first-person camera GameObject
            topDownCamera.gameObject.SetActive(true);      // Enable the top-down camera GameObject
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
