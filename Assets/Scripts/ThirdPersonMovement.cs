using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Transform groundCheck;
    public LayerMask groundMask;
    public float groundDistance = 0.25f;
    public float speed = 10f;
    public float gravity = -9.82f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 1f;

    float turnSmoothVelocity;
    Vector3 velocity;
    bool isGrounded;

    Vector2 movementInput;

    /* void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    } */
    void Update()
    {
        // GROUND CHECK
        GroundCheck();

        // HANDLE MOVEMENT AND TURNING
        MoveAndTurn(movementInput);
        
        // HANDLE FALLING
        Fall();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void MoveAndTurn(Vector2 movementInput)
    {
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;

        if(direction.magnitude > 0.1)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDirection.normalized * speed * Time.deltaTime);
        }
    }


    void Jump()
    {
        if(isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Fall()
    {
        velocity.y +=gravity * Time.deltaTime;
        controller.Move(velocity *Time.deltaTime);
    }
}
