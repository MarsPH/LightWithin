using UnityEngine;
using UnityEngine.Events;

public class ShadowController : MonoBehaviour
{
     [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Physics Settings")]
    public float gravity = -9.81f;
    public LayerMask groundLayer;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Ceiling Check Settings")]
    public Transform ceilingCheck;
    public float ceilingCheckRadius = 0.2f;

    [Header("Side Check Settings")]
    public Transform leftSideCheck;
    public Transform rightSideCheck;
    public float sideCheckRadius = 0.2f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isTouchingCeiling;
    private bool isTouchingLeftSide;
    private bool isTouchingRightSide;

    private void Update()
    {
        GroundCheck();
        CeilingCheck();
        SideCheck();
        Movement();
        Jump();
        ApplyGravity();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            // Reset vertical velocity when grounded
            velocity.y = 0f;
        }
    }

    private void CeilingCheck()
    {
        isTouchingCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingCheckRadius, groundLayer);

        if (isTouchingCeiling && velocity.y > 0)
        {
            // Stop upward movement when hitting the ceiling
            velocity.y = 0f;
        }
    }

    private void SideCheck()
    {
        isTouchingLeftSide = Physics.CheckSphere(leftSideCheck.position, sideCheckRadius, groundLayer);
        isTouchingRightSide = Physics.CheckSphere(rightSideCheck.position, sideCheckRadius, groundLayer);
    }

    private void Movement()
    {
        float moveZ = Input.GetAxis("Vertical"); // Forward/backward movement
        float moveX = Input.GetAxis("Horizontal"); // Left/right movement

        // Block movement into walls
        if (isTouchingLeftSide && moveX < 0)
        {
            moveX = 0; // Stop left movement
        }
        if (isTouchingRightSide && moveX > 0)
        {
            moveX = 0; // Stop right movement
        }

        // Prevent backward movement while grounded
        if (isGrounded && moveZ < 0)
        {
            moveZ = 0;
        }

        // Apply movement
        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Calculate jump velocity
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded) // Apply gravity when not grounded
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply vertical velocity
        transform.Translate(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        // Ground Check Gizmo
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        // Ceiling Check Gizmo
        if (ceilingCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }

        // Left Side Check Gizmo
        if (leftSideCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(leftSideCheck.position, sideCheckRadius);
        }

        // Right Side Check Gizmo
        if (rightSideCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(rightSideCheck.position, sideCheckRadius);
        }
    }
}
