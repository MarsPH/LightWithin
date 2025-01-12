using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class LightAreaCollider : MonoBehaviour
{
    private LightDamageEffect damageManager;

    void Start()
    {
        // Find the centralized damage manager in the scene
        damageManager = FindObjectOfType<LightDamageEffect>();

        // Ensure the trigger collider is set up
        SphereCollider trigger = GetComponent<SphereCollider>();
        trigger.isTrigger = true;

        // Automatically adjust the trigger size to match the light's range
        Light lightSource = GetComponent<Light>();
        if (lightSource != null && lightSource.type == LightType.Point)
        {
            trigger.radius = lightSource.range;
        }
        else
        {
            Debug.LogError("LightTrigger script only works with Point Lights.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && damageManager != null)
        {
            damageManager.SetPlayerInLight(true);
            Debug.Log("Player entered light trigger.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && damageManager != null)
        {
            damageManager.SetPlayerInLight(false);
            Debug.Log("Player exited light trigger.");
        }
    }
}