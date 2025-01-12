using UnityEngine;

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

    [Header("Sound Effects")]
    public AudioClip jumpSound;
    public AudioClip walkSound;
    public AudioClip fallSound;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isTouchingCeiling;
    private bool isTouchingLeftSide;
    private bool isTouchingRightSide;

    private Animator animator;
    private AudioSource walkAudioSource;
    private AudioSource effectAudioSource;
    private bool wasGroundedLastFrame = true;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // Assign the primary AudioSource for walking sound
        walkAudioSource = GetComponent<AudioSource>();

        // Create a separate AudioSource for effects like jumping
        effectAudioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Update()
    {
        GroundCheck();
        CeilingCheck();
        SideCheck();
        Movement();
        Jump();
        ApplyGravity();
        UpdateAnimatorParameters();
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            velocity.y = 0f; // Reset vertical velocity when grounded
        }
    }

    private void CeilingCheck()
    {
        isTouchingCeiling = Physics.CheckSphere(ceilingCheck.position, ceilingCheckRadius, groundLayer);

        if (isTouchingCeiling && velocity.y > 0)
        {
            velocity.y = 0f; // Stop upward movement when hitting the ceiling
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

        if (move.magnitude > 0 && isGrounded)
        {
            if (!walkAudioSource.isPlaying)
            {
               //
               // walkAudioSource.loop = true; // Ensure the sound loops
               // walkAudioSource.clip = walkSound; // Set the walk sound clip
               // walkAudioSource.Play(); // Start playing the sound
            }
        }
        else
        {
            // Stop walking sound if movement stops or when not grounded
            if (walkAudioSource.isPlaying)
            {
                walkAudioSource.Stop();
            }
        }

        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Play the jump sound using the effects AudioSource
            effectAudioSource.PlayOneShot(jumpSound);

            // Calculate and apply the jump velocity
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            velocity.y += gravity * Time.deltaTime; // Apply gravity when not grounded
        }

        // Apply vertical velocity
        transform.Translate(new Vector3(0, velocity.y, 0) * Time.deltaTime);
    }

    private void UpdateAnimatorParameters()
    {
        animator.SetBool("isGrounded", isGrounded);

        // Jumping logic
        animator.SetBool("isJumping", !isGrounded && velocity.y > 0);

        // Landing logic
        animator.SetBool("isLanding", isGrounded && velocity.y <= 0);

        // Walking logic
        float moveZ = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");
        animator.SetBool("isWalking", moveX != 0 || moveZ != 0);
    }
}

