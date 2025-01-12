using UnityEngine;
using UnityEngine.Events;

public class CustomPhysicsController : MonoBehaviour
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

    private Vector3 velocity;
    private bool isGrounded;
    private bool isTouchingCeiling;

    private void Update()
    {
        GroundCheck();
        CeilingCheck();
        Movement();
        Jump();
        ApplyGravity();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            // Ensure the player doesn't keep moving downward when grounded
            velocity.y = 0f;
        }
    }

    private void CeilingCheck()
    {
        isTouchingCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingCheckRadius, groundLayer);

        if (isTouchingCeiling && velocity.y > 0)
        {
            // Stop upward movement if the ceiling is touched
            velocity.y = 0f;
        }
    }

    private void Movement()
    {
        float moveZ = Input.GetAxis("Vertical"); // Move along the Z-axis
        float moveX = Input.GetAxis("Horizontal"); // Move along the X-axis

        Vector3 move = new Vector3(moveX, 0, moveZ).normalized;

        if (move.magnitude >= 0.1f)
        {
            transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded) // Only apply gravity when the player is not grounded
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Apply the vertical velocity
        transform.Translate(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (ceilingCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(ceilingCheck.position, ceilingCheckRadius);
        }
    }
}
