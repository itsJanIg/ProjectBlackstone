using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 50;
    public Rigidbody rb;
    Vector2 moveInputVector;
    bool isGrounded = true;
    [Header("Move")]
    public float speed = 10f;

    [Header("Jump")]
    public float jumpForce = 6.5f;
    public Transform groundCheck;           // assign in Inspector
    public float groundCheckRadius = 0.2f;  // ~0.2 for a 0.5 sphere
    public LayerMask groundMask;            // set to Ground layer

    [Header("Refs")]
    public Rigidbody rb;

    private Vector2 moveInputVector;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (IsGrounded())
        {
            // make jump consistent
            var v = rb.linearVelocity;
            v.y = Mathf.Max(0f, v.y);
            rb.linearVelocity = v;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // Camera-relative movement, ignore camera pitch
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();
        Vector3 right = new Vector3(forward.z, 0f, -forward.x);

        Vector3 force = new Vector3(cameraRelativeInput.x * speed, 0, cameraRelativeInput.z * speed);
        rb.AddForce(force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Simple ground detection
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
        Vector3 desiredXZ = (right * moveInputVector.x + forward * moveInputVector.y) * speed;
        Vector3 vel = rb.linearVelocity;
        Vector3 accel = new Vector3(desiredXZ.x - vel.x, 0f, desiredXZ.z - vel.z);

        // Snappy horizontal control while keeping current vertical
        rb.AddForce(accel, ForceMode.VelocityChange);
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
