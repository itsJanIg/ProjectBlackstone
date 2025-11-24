using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    public float moveForce = 20f;

    [Header("Jump")]
    public float jumpForce = 6.5f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundMask;

    // How long after leaving ground we still allow a jump
    public float coyoteTime = 0.1f;

    [Header("Refs")]
    public Rigidbody rb;

    private Vector2 moveInputVector;

    private bool jumpQueued;
    private float coyoteTimer;
    private bool isGrounded;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.freezeRotation = false; // let the ball roll
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    // Input System: "Move" action
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputVector = context.ReadValue<Vector2>();
    }

    // Input System: "Jump" action
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        // Just queue the jump; actual jump happens in FixedUpdate
        jumpQueued = true;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        // --- Grounding ---
        isGrounded = IsGrounded();

        if (isGrounded)
        {
            coyoteTimer = coyoteTime; // reset grace timer
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        // --- Movement (rolling) ---
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 forward = cam.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.0001f)
                forward = Vector3.forward;
            forward.Normalize();

            Vector3 right = new Vector3(forward.z, 0f, -forward.x);
            Vector3 moveDir = right * moveInputVector.x + forward * moveInputVector.y;

            rb.AddForce(moveDir * moveForce, ForceMode.Force);
        }

        // --- Jump consume ---
        if (jumpQueued && coyoteTimer > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = Mathf.Max(0f, vel.y);
            rb.linearVelocity = vel;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            jumpQueued = false; // consume jump
        }
        else if (jumpQueued && coyoteTimer <= 0f)
        {
            // Pressed jump too late (not grounded and outside coyote time)
            jumpQueued = false;
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null)
            return false;

        return Physics.CheckSphere(
            groundCheck.position,
            groundCheckRadius,
            groundMask,
            QueryTriggerInteraction.Ignore
        );
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
