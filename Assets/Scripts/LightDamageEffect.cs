using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using System.Collections;

public class LightDamageEffect : MonoBehaviour
{
    public float damageRate = 5f; // Damage per second to the player
    public float recoveryRate = 2f; // Health recovery per second when out of the light
    public PostProcessVolume postProcessVolume; // Post-processing volume for effects
    public TextMeshProUGUI darknessTextPrefab; // Prefab for "Darkness" text
    public Canvas canvas; // Canvas to spawn "Darkness" texts
    public Renderer playerRenderer; // Renderer for the player
    public float maxBloomIntensity = 3f; // Max bloom intensity for visual feedback
    public float maxVignetteIntensity = 0.6f; // Max vignette intensity for visual feedback
    public float playerHealth = 100f; // Player's health
    public float maximumPlayerHealth = 100f;
    public int maxTextCount = 20; // Maximum number of "Darkness" texts on the screen
    public float maxTextShakeMagnitude = 50f; // Maximum shake intensity of the text
    public float minTextShakeMagnitude = 10f; // Minimum shake intensity of the text
    public CanvasGroup fadeCanvasGroup; // CanvasGroup for fade-to-black effect
    public float fadeDuration = 2f; // Duration of the fade effect

    private Bloom bloomEffect; // Bloom effect for brightness
    private Vignette vignetteEffect; // Vignette effect for darkening edges
    private float originalBloomIntensity = 1f; // Default bloom intensity
    private float originalVignetteIntensity = 0.2f; // Default vignette intensity
    private bool playerInLight = false; // Tracks if the player is in any light zone
    private GameObject[] darknessTexts; // Pool of Darkness text objects
    private bool gameOver = false; // Tracks if the game is over
    private bool isFading = false; // Tracks if fade is in progress

    void Start()
    {
        // Initialize Post-Processing Effects
        if (postProcessVolume != null)
        {
            postProcessVolume.profile.TryGetSettings(out bloomEffect);
            postProcessVolume.profile.TryGetSettings(out vignetteEffect);

            if (bloomEffect != null)
                originalBloomIntensity = bloomEffect.intensity.value;

            if (vignetteEffect != null)
                originalVignetteIntensity = vignetteEffect.intensity.value;
        }

        // Initialize Darkness Text Pool
        darknessTexts = new GameObject[maxTextCount];
        for (int i = 0; i < maxTextCount; i++)
        {
            var textInstance = Instantiate(darknessTextPrefab, canvas.transform);
            textInstance.gameObject.SetActive(false);
            darknessTexts[i] = textInstance.gameObject;
        }

        // Ensure the fadeCanvasGroup starts fully transparent
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
        }
    }

    void Update()
    {
        if (gameOver) return; // If game is over, stop further updates

        if (playerInLight)
        {
            // Apply damage to the player
            playerHealth -= damageRate * Time.deltaTime;
            if (playerHealth <= 0)
            {
                playerHealth = 0;
                GameOver();
            }

            // Dynamic Bloom Intensity (increases as health decreases)
            if (bloomEffect != null)
            {
                float healthPercentage = playerHealth / 100f;
                bloomEffect.intensity.value = Mathf.Lerp(maxBloomIntensity, originalBloomIntensity, healthPercentage);
            }

            // Dynamic Vignette Intensity
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Lerp(originalVignetteIntensity, maxVignetteIntensity, Time.deltaTime * 2f);
            }

            // Update Player Transparency
            UpdatePlayerTransparency();

            // Show and Shake Darkness Texts based on health
            int activeTextCount = Mathf.CeilToInt((1 - playerHealth / 100f) * maxTextCount);
            float textShakeMagnitude = Mathf.Lerp(minTextShakeMagnitude, maxTextShakeMagnitude, 1 - playerHealth / 100f);

            for (int i = 0; i < darknessTexts.Length; i++)
            {
                if (i < activeTextCount)
                {
                    if (!darknessTexts[i].activeSelf)
                    {
                        darknessTexts[i].SetActive(true);
                        darknessTexts[i].transform.localPosition = GetRandomScreenPosition();
                    }

                    // Shake text abruptly
                    darknessTexts[i].transform.localPosition += new Vector3(
                        Random.Range(-textShakeMagnitude, textShakeMagnitude),
                        Random.Range(-textShakeMagnitude, textShakeMagnitude),
                        0
                    ) * Time.deltaTime;
                }
                else
                {
                    darknessTexts[i].SetActive(false);
                }
            }
        }
        else
        {
            // Recover health
            playerHealth += recoveryRate * Time.deltaTime;
            if (playerHealth > maximumPlayerHealth)
            {
                playerHealth = maximumPlayerHealth;
            }

            // Reset Bloom Intensity
            if (bloomEffect != null)
            {
                bloomEffect.intensity.value = Mathf.Lerp(bloomEffect.intensity.value, originalBloomIntensity, Time.deltaTime * 2f);
            }

            // Reset Vignette Intensity
            if (vignetteEffect != null)
            {
                vignetteEffect.intensity.value = Mathf.Lerp(vignetteEffect.intensity.value, originalVignetteIntensity, Time.deltaTime * 2f);
            }

            // Reset Player Transparency
            UpdatePlayerTransparency();

            // Hide Darkness Texts
            foreach (var textObject in darknessTexts)
            {
                textObject.SetActive(false);
            }
        }
    }

    private void UpdatePlayerTransparency()
    {
        if (playerRenderer != null)
        {
            float healthPercentage = playerHealth / 100f;
            Color currentColor = playerRenderer.material.GetColor("_ShadowColor");
            currentColor.a = Mathf.Clamp(healthPercentage, 0.2f, 1f); // Adjust alpha based on health
            playerRenderer.material.SetColor("_ShadowColor", currentColor);
        }
    }

    private void GameOver()
    {
        if (isFading) return; // Prevent multiple triggers

        gameOver = true;

        // Start fade-to-black and reload process
        StartCoroutine(FadeAndReload());
    }

    private IEnumerator FadeAndReload()
    {
        isFading = true;

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(timer / fadeDuration);
            yield return null;
        }

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private Vector3 GetRandomScreenPosition()
    {
        // Generate a random position within the screen bounds
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        float x = Random.Range(-canvasRect.rect.width / 2, canvasRect.rect.width / 2);
        float y = Random.Range(-canvasRect.rect.height / 2, canvasRect.rect.height / 2);
        return new Vector3(x, y, 0);
    }

    public void SetPlayerInLight(bool inLight)
    {
        playerInLight = inLight;
    }
}
