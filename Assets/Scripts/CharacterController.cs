using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using KinematicCharacterController;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public struct PlayerInputs
{
    public float MoveAxisForward;
    public float MoveAxisRight;
    public Quaternion CameraRotation;
    public bool JumpPressed;
    public Vector3 MovementDirection;
}
public class CharacterController : MonoBehaviour, ICharacterController
{
    [SerializeField]
    private KinematicCharacterMotor _motor;

    [SerializeField] private Vector3 _gravity = new Vector3(0f, -30f, 0f);

    [SerializeField] private float _maxStableMoveSpeed = 10f, _stableMovementSharpness = 15f, _orientationSharpness = 10f;

    [SerializeField] private float _jumpSpeed = 30f;
    
    private Vector3 _moveInputVector, _lookInputVector;
    private bool _jumpRequested;

    private void Start()
    {
        _motor.CharacterController = this;
    }

    public void SetInputs(ref PlayerInputs inputs)
    {
        // Get clamped movement input
        Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

        // Calculate planar directions based on the camera
        Vector3 cameraPlanarForward = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Vector3.up).normalized;
        Vector3 cameraPlanarRight = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.right, Vector3.up).normalized;

        // Determine movement direction relative to the camera
        _moveInputVector = (cameraPlanarForward * moveInputVector.z) + (cameraPlanarRight * moveInputVector.x);

        // Ensure character always faces forward
        _lookInputVector = cameraPlanarForward;

        // Handle jump input
        if (inputs.JumpPressed)
        {
            _jumpRequested = true;
        }
        
    }
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (_lookInputVector.sqrMagnitude > 0.01f && _orientationSharpness > 0f)
        {
            // Always face forward relative to the camera
            Vector3 smoothedLookInputDirection = Vector3.Slerp(
                _motor.CharacterForward,
                _lookInputVector,
                1 - Mathf.Exp(-_orientationSharpness * deltaTime)
            ).normalized;

            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, _motor.CharacterUp);
        }
    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        if (_motor.GroundingStatus.IsStableOnGround)
        {
            // Reset vertical velocity when grounded to prevent snapping to ground
            if (currentVelocity.y < 0f)
            {
                currentVelocity.y = 0f;
            }

            // Calculate horizontal velocity for grounded movement
            Vector3 effectiveGroundNormal = _motor.GroundingStatus.GroundNormal;
            Vector3 inputRight = Vector3.Cross(_moveInputVector, _motor.CharacterUp);
            Vector3 reorientInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;

            Vector3 targetMovementVelocity = reorientInput * _maxStableMoveSpeed;

            currentVelocity.x = targetMovementVelocity.x;
            currentVelocity.z = targetMovementVelocity.z;

            // Handle jumping
            if (_jumpRequested)
            {
                currentVelocity.y = _jumpSpeed; // Apply vertical jump velocity
                _jumpRequested = false;
                _motor.ForceUnground(); // Force the motor to consider the character airborne
            }
        }
        else
        {
            // Air movement
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(currentVelocity, _motor.CharacterUp);
            Vector3 targetAirVelocity = _moveInputVector * _maxStableMoveSpeed;

            // Blend horizontal velocity for smoother air control
            currentVelocity.x = Mathf.Lerp(horizontalVelocity.x, targetAirVelocity.x, deltaTime * _stableMovementSharpness);
            currentVelocity.z = Mathf.Lerp(horizontalVelocity.z, targetAirVelocity.z, deltaTime * _stableMovementSharpness);

            // Apply gravity
            currentVelocity += _gravity * deltaTime;
        }
     
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
    }

    public void PostGroundingUpdate(float deltaTime)
    {
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport)
    {
    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition,
        Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
    }
}
