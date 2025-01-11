using UnityEngine;
using UnityEngine.Events;

public class CustomPhysicsController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Movement speed.
    [SerializeField] private float jumpForce = 10f; // Jump force.
    [SerializeField] private float gravity = 20f; // Gravity applied along the global Y-axis.

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint; // Transform to specify the ground check position.
    [SerializeField] private float groundCheckRadius = 0.2f; // Radius for ground detection.
    [SerializeField] private LayerMask groundMask; // LayerMask to identify ground objects.

    [Header("Crouching")]
    [SerializeField] private float crouchSpeedMultiplier = 0.5f; // Speed multiplier when crouching.
    [SerializeField] private Collider crouchCollider; // Collider to disable when crouching.

    private Vector3 velocity; // Custom velocity vector.
    private bool isGrounded; // Is the character grounded?
    private bool facingRight = true; // Character facing direction.
    private bool isCrouching = false;

    [Header("Events")]
    public UnityEvent OnLandEvent;
    public UnityEvent<bool> OnCrouchEvent;

    private void Awake()
    {
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new UnityEvent<bool>();
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool crouch = Input.GetKey(KeyCode.LeftControl);

        Move(moveX, crouch, jump);
    }

    private void FixedUpdate()
    {
        CheckGroundStatus();
        ApplyGravity();
        ApplyVelocity();
    }

    public void Move(float moveX, bool crouch, bool jump)
    {
        if (!crouch && isCrouching)
        {
            if (Physics.Raycast(transform.position, transform.up, 0.1f, groundMask))
            {
                crouch = true;
            }
        }

        if (crouch)
        {
            if (!isCrouching)
            {
                isCrouching = true;
                OnCrouchEvent.Invoke(true);
            }

            moveX *= crouchSpeedMultiplier;

            if (crouchCollider != null)
                crouchCollider.enabled = false;
        }
        else
        {
            if (isCrouching)
            {
                isCrouching = false;
                OnCrouchEvent.Invoke(false);
            }

            if (crouchCollider != null)
                crouchCollider.enabled = true;
        }

        Vector3 moveDirection = transform.right * moveX;
        velocity.x = moveDirection.x * moveSpeed;

        if (isGrounded && jump)
        {
            velocity.y = jumpForce; // Apply jump force along the global Y-axis
            isGrounded = false;
        }

        if (moveX > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveX < 0 && facingRight)
        {
            Flip();
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            // Apply gravity along the global Y-axis
            velocity.y -= gravity * Time.fixedDeltaTime;
        }
    }

    private void CheckGroundStatus()
    {
        // Use Physics.CheckSphere to check if the character is on the ground
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundMask);

        if (isGrounded)
        {
            velocity.y = 0f; // Reset vertical velocity when grounded
            OnLandEvent.Invoke();
        }

        // Debugging: Visualize the ground check sphere
        Debug.DrawRay(groundCheckPoint.position, -transform.up * groundCheckRadius, isGrounded ? Color.green : Color.red);
    }

    private void ApplyVelocity()
    {
        // Apply velocity to the position
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the ground check sphere in the editor
        if (groundCheckPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
