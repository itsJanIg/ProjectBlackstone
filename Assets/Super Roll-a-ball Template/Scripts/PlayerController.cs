using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 10;
    public float jumpForce = 50;
    public Rigidbody rb;
    Vector2 moveInputVector;
    bool isGrounded = true;

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInputVector = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void FixedUpdate()
    {
        var movementInput = new Vector3(moveInputVector.x, 0, moveInputVector.y);
        var camRotationFlattened = Quaternion.LookRotation(Camera.main.transform.forward);
        var cameraRelativeInput = camRotationFlattened * movementInput;

        Vector3 force = new Vector3(cameraRelativeInput.x * speed, 0, cameraRelativeInput.z * speed);
        rb.AddForce(force);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Simple ground detection
        if (collision.contacts[0].normal.y > 0.5f)
            isGrounded = true;
    }
}
