using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static event Action<float, float> OnMove;
    public static event Action<bool> OnJump;

    public float speed = 2f;
    public float rotationSpeed = 100f;
    public float jumpForce = 2f;
    public float gravity = 9.81f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private Transform cameraTransform;

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

        if (isRightMouseHeld)
        {
            // Если ПКМ зажат, персонаж движется вместе с камерой
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0; // Убираем влияние наклона камеры
            right.y = 0;

            Vector3 movementDirection = forward * input.y + right * input.x;
            moveDirection = movementDirection.normalized * speed;
            
            if (movementDirection.magnitude > 0)
                transform.rotation = Quaternion.LookRotation(movementDirection);
        }
        else
        {
            // Если ПКМ НЕ зажат – поворот персонажа на месте (как в старых RPG)
            float turn = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);

            // Двигаемся вперёд-назад по направлению персонажа
            Vector3 forwardMovement = transform.forward * input.y * speed;
            moveDirection = new Vector3(forwardMovement.x, moveDirection.y, forwardMovement.z);
        }

        OnMove?.Invoke(input.x, input.y);
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
