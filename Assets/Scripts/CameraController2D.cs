using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // Player Transform
    public Vector2 offset; // Offset from the player's position

    [Header("Camera Bounds")]
    public float leftBound = -10f;
    public float rightBound = 10f;
    public float bottomBound = -5f;
    public float topBound = 5f;

    [Header("Movement Settings")]
    public float smoothTime = 0.3f; // Smooth damping time
    public float triggerDistance = 2f; // Distance from bounds that triggers camera movement

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (player == null) return;

        // Get the current camera position
        Vector3 targetPosition = transform.position;

        // Calculate player's position relative to camera bounds
        Vector3 playerPosition = player.position;

        // Check if the player is near the left or right bounds
        if (playerPosition.x < transform.position.x - triggerDistance)
        {
            targetPosition.x = Mathf.Clamp(playerPosition.x + offset.x, leftBound, rightBound);
        }
        else if (playerPosition.x > transform.position.x + triggerDistance)
        {
            targetPosition.x = Mathf.Clamp(playerPosition.x - offset.x, leftBound, rightBound);
        }

        // Check if the player is near the bottom or top bounds (z-axis for vertical movement)
        if (playerPosition.z < transform.position.z - triggerDistance)
        {
            targetPosition.z = Mathf.Clamp(playerPosition.z + offset.y, bottomBound, topBound);
        }
        else if (playerPosition.z > transform.position.z + triggerDistance)
        {
            targetPosition.z = Mathf.Clamp(playerPosition.z - offset.y, bottomBound, topBound);
        }

        // Smoothly move the camera towards the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize camera bounds in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(leftBound, 0, bottomBound), new Vector3(leftBound, 0, topBound));
        Gizmos.DrawLine(new Vector3(rightBound, 0, bottomBound), new Vector3(rightBound, 0, topBound));
        Gizmos.DrawLine(new Vector3(leftBound, 0, topBound), new Vector3(rightBound, 0, topBound));
        Gizmos.DrawLine(new Vector3(leftBound, 0, bottomBound), new Vector3(rightBound, 0, bottomBound));
    }
}
