using UnityEngine;
using UnityEngine.Events;

public class ShadowWalkerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f; // Movement speed.
    [SerializeField] private float jumpForce = 10f; // Jump force.
    [SerializeField] private float gravity = 20f; // Gravity applied along the global Y-axis.

    [Header("Layer Settings")]
    [SerializeField] private LayerMask groundMask; // LayerMask for ground (e.g., water).
    [SerializeField] private LayerMask shadowMask; // LayerMask for shadows.

    [Header("Check Settings")]
    [SerializeField] private Transform checkPoint; // Transform to specify the check position.
    [SerializeField] private float checkRadius = 0.2f; // Radius for both ground and shadow detection.

    [Header("Shadow Settings")]
    [SerializeField] private Material shadowMaterial; // Material for shadow mode.
    [SerializeField] private Material normalMaterial; // Regular material.

    [Header("Events")]
    public UnityEvent OnLandEvent;
    public UnityEvent OnEnterShadowEvent;
    public UnityEvent OnExitShadowEvent;

    private Vector3 velocity; // Custom velocity vector.
    private bool isGrounded = false; // Is the character on the ground?
    private bool isInShadow = false; // Is the character in a shadow?
    private bool facingRight = true; // Character facing direction.
    private bool shadowMode = false; // Is the character currently in shadow mode?

    private void Awake()
    {
        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnEnterShadowEvent == null)
            OnEnterShadowEvent = new UnityEvent();

        if (OnExitShadowEvent == null)
            OnExitShadowEvent = new UnityEvent();
    }

    private void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        bool jump = Input.GetKeyDown(KeyCode.Space);
        bool enterShadow = Input.GetKeyDown(KeyCode.E); // Key to enter shadow mode.

        if (enterShadow && isInShadow)
        {
            ToggleShadowMode();
        }
        else if (!shadowMode)
        {
            Move(moveX, jump);
        }
    }

    private void FixedUpdate()
    {
        CheckEnvironment();
        if (!shadowMode)
        {
            ApplyGravity();
            ApplyVelocity();
        }
    }

    private void Move(float moveX, bool jump)
    {
        Vector3 moveDirection = transform.right * moveX;
        velocity.x = moveDirection.x * moveSpeed;

        if (isGrounded && jump)
        {
            velocity.y = jumpForce;
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
            velocity.y -= gravity * Time.fixedDeltaTime;
        }
    }

    private void CheckEnvironment()
    {
        isGrounded = Physics.CheckSphere(checkPoint.position, checkRadius, groundMask);
        isInShadow = Physics.CheckSphere(checkPoint.position, checkRadius, shadowMask);

        if (isGrounded && !isInShadow)
        {
            velocity.y = 0f;
            OnLandEvent.Invoke();
        }

        // Debugging: Visualize checks
        Debug.DrawRay(checkPoint.position, -transform.up * checkRadius, isGrounded ? Color.green : Color.red);
        Debug.DrawRay(checkPoint.position, -transform.up * checkRadius, isInShadow ? Color.blue : Color.yellow);
    }

    private void ApplyVelocity()
    {
        transform.position += velocity * Time.fixedDeltaTime;
    }

    private void ToggleShadowMode()
    {
        shadowMode = !shadowMode;

        if (shadowMode)
        {
            EnterShadowMode();
        }
        else
        {
            ExitShadowMode();
        }
    }

    private void EnterShadowMode()
    {
        velocity = Vector3.zero; // Stop all movement.
        SetShadowAppearance(true);
        OnEnterShadowEvent.Invoke();
        Debug.Log("Entered shadow mode.");
    }

    private void ExitShadowMode()
    {
        SetShadowAppearance(false);
        OnExitShadowEvent.Invoke();
        Debug.Log("Exited shadow mode.");
    }

    private void SetShadowAppearance(bool isShadowMode)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = isShadowMode ? shadowMaterial : normalMaterial;
        }
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
        if (checkPoint != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(checkPoint.position, checkRadius);

            Gizmos.color = isInShadow ? Color.blue : Color.yellow;
            Gizmos.DrawWireSphere(checkPoint.position, checkRadius);
        }
    }
}
