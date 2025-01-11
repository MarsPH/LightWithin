using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///   Revision History
///   Mahan Poor Hamidian   2025/1/11   Created PlayerCamera Script
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _defaultDistance = 6f,
        _minDistance = 3f,
        _maxDistance = 10f,
        _distanceMovementSpeed = 5f,
        _distanceMovementSharpness = 10f,
        _rotationSpeed = 10f,
        _rotationSharpness = 10000f,
        _followSharpness = 10000f,
        _minVerticalAngle = -30f,
        _maxVerticalAngle = 60f,
        _minHorizontalAngle = -70f,
        _maxHorizontalAngle = 70f,
        _defaultVerticalAngle = 20f;
    
    private Transform _followTransform;
    private Vector3 _currentFollowPosition, _planarDirection;
    private float _targetVerticalAngle;

    private float _currentDistance, _targetDistance;

    private void Awake()
    {
        _currentDistance = _defaultDistance;
        _targetDistance = _currentDistance;
        _targetVerticalAngle = 0f;
        _planarDirection = Vector3.forward; 
    }

    public void SetFollowTransform(Transform t)
    {
        _followTransform = t;
        _currentFollowPosition = t.position;
        _planarDirection = t.forward;
    }

    private void OnValidate()
    {
        _defaultDistance = Mathf.Clamp(_defaultDistance, _minDistance, _maxDistance);
        _defaultVerticalAngle = Mathf.Clamp(_defaultVerticalAngle, _minVerticalAngle, _maxVerticalAngle);

        // Clamp values to prevent backward rotation
        _minVerticalAngle = Mathf.Clamp(_minVerticalAngle, -30f, 0f); // Adjust to your needs
        _maxVerticalAngle = Mathf.Clamp(_maxVerticalAngle, 0f, 60f);  // Adjust to your needs
    
    }
    
    private bool _isMovingBackward; // Add this to store the backward movement state

    public void SetIsMovingBackward(bool isMovingBackward)
    {
        _isMovingBackward = isMovingBackward;
    }
    
    private void HandleRotationInput(float deltaTime, Vector3 rotationInput, out Quaternion targetRotation)
    {
        float horizontalAngle = Vector3.SignedAngle(_followTransform.forward, _planarDirection, Vector3.up);

        // Ignore rotation input when moving backward
        if (!_isMovingBackward)
        {
            horizontalAngle += rotationInput.x * _rotationSpeed;
        }

        horizontalAngle = Mathf.Clamp(horizontalAngle, _minHorizontalAngle, _maxHorizontalAngle);

        _planarDirection = Quaternion.Euler(0f, horizontalAngle, 0f) * _followTransform.forward;

        _targetVerticalAngle -= rotationInput.y * _rotationSpeed;
        _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, _minVerticalAngle, _maxVerticalAngle);

        Quaternion planarRot = Quaternion.LookRotation(_planarDirection, Vector3.up);
        Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0f, 0f);

        targetRotation = Quaternion.Slerp(transform.rotation, planarRot * verticalRot, _rotationSharpness * deltaTime);
        transform.rotation = targetRotation;
    }

    private void HandlePosition(float deltaTime, float zoomInput, Quaternion targetRotation)
    {
        _targetDistance += zoomInput * _distanceMovementSharpness;
        _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        
        _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, _followTransform.position, 1f - Mathf.Exp(-_followSharpness * deltaTime));
        Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);
        
        _currentDistance = Mathf.Lerp(_currentDistance, _targetDistance, 1 - Mathf.Exp(-_distanceMovementSharpness * deltaTime));
        transform.position = targetPosition;
    }

    public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput)
    {
        if (_followTransform)
        {
            HandleRotationInput(deltaTime, rotationInput, out Quaternion targetRotation);
            HandlePosition(deltaTime, zoomInput, targetRotation);
        }
    }

}
