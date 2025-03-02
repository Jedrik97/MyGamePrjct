using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float, bool> OnMove; // Добавляем bool для бега
    public static event Action<bool> OnJump;

    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 100f;
    public float jumpForce = 2f;
    public float gravity = 9.81f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private Transform cameraTransform;
    private bool isRunning = false;

    private void OnEnable()
    {
        PlayerInput.OnMoveInput += HandleMoveInput;
        PlayerInput.OnJumpInput += HandleJumpInput;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void HandleMoveInput(Vector2 input)
    {
        bool isRightMouseHeld = Input.GetMouseButton(1);
        isRunning = Input.GetKey(KeyCode.LeftShift); // Проверяем зажат ли Shift
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isRightMouseHeld)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;

            Vector3 movementDirection = forward * input.y + right * input.x;
            moveDirection = movementDirection.normalized * currentSpeed;

            if (movementDirection.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(movementDirection);
        }
        else
        {
            float turn = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);

            Vector3 forwardMovement = transform.forward * input.y * currentSpeed;
            moveDirection = new Vector3(forwardMovement.x, moveDirection.y, forwardMovement.z);
        }

        OnMove?.Invoke(input.x, input.y, isRunning); // Передаём бег в аниматор
    }

    private void HandleJumpInput()
    {
        if (characterController.isGrounded && !isJumping)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
            OnJump?.Invoke(true);
        }
    }

    void Update()
    {
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else if (isJumping)
        {
            isJumping = false;
            OnJump?.Invoke(false);
        }

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void OnDisable()
    {
        PlayerInput.OnMoveInput -= HandleMoveInput;
        PlayerInput.OnJumpInput -= HandleJumpInput;
    }
}
