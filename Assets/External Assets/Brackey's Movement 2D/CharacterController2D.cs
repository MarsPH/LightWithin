using UnityEngine;

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
    private bool isPeeking = false;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GroundCheck();
        CeilingCheck();
        Movement();
        Jump();
        ApplyGravity();
        UpdateAnimatorParameters();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && velocity.y <= 0)
        {
            velocity.y = 0f;
        }
    }

    private void CeilingCheck()
    {
        isTouchingCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingCheckRadius, groundLayer);

        if (isTouchingCeiling && velocity.y > 0)
        {
            velocity.y = 0f;
        }
    }

    private void Movement()
    {
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

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
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }

        transform.Translate(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    private void UpdateAnimatorParameters()
    {
        // Update grounded and vertical velocity
        animator.SetBool("isGrounded", isGrounded);

        // Jumping logic
        if (!isGrounded && velocity.y > 0)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }

        // Landing logic
        if (isGrounded && velocity.y <= 0)
        {
            animator.SetBool("isLanding", true);
        }
        else
        {
            animator.SetBool("isLanding", false);
        }

        // Walking logic
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        animator.SetBool("isWalking", moveX != 0 || moveZ != 0);
    }

    private void HandleAnimationStates()
    {
        // Random peeking when idle
        if (isGrounded && !animator.GetBool("isWalking") && !animator.GetBool("isJumping") && !isPeeking)
        {
            if (Random.Range(0f, 100f) < 0.1f) // 0.1% chance per frame
            {
                StartCoroutine(PeekAnimation());
            }
        }
    }

    private System.Collections.IEnumerator PeekAnimation()
    {
        isPeeking = true;
        animator.SetBool("isPeeking", true);
        yield return new WaitForSeconds(1f); // Adjust time based on animation length
        animator.SetBool("isPeeking", false);
        isPeeking = false;
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
