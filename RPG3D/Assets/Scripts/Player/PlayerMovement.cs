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
    public float rotationSmoothTime = 0.1f; // Добавляем плавность поворота

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private Transform cameraTransform;
    private bool isRunning = false;
    private float rotationVelocity; // Для плавного поворота

    private void OnEnable()
    {
        PlayerInput.OnMoveInput += HandleMoveInput;
        PlayerInput.OnJumpInput += HandleJumpInput;
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform; // Убедитесь, что это работает с Cinemachine
    }

    private void HandleMoveInput(Vector2 input)
    {
        bool isRightMouseHeld = Input.GetMouseButton(1);
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (input.magnitude < 0.1f) // Если нет ввода, сбрасываем горизонтальное движение
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            OnMove?.Invoke(input.x, input.y, isRunning);
            return;
        }

        if (isRightMouseHeld)
        {
            // Движение относительно камеры
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            Vector3 movementDirection = (forward * input.y + right * input.x).normalized;
            moveDirection = new Vector3(movementDirection.x * currentSpeed, moveDirection.y, movementDirection.z * currentSpeed);

            // Плавный поворот персонажа к направлению движения
            if (movementDirection.magnitude > 0)
            {
                float targetAngle = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg;
                float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
                transform.rotation = Quaternion.Euler(0, smoothedAngle, 0);
            }
        }
        else
        {
            // Поворот персонажа без привязки к камере
            float turn = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, turn, 0);

            Vector3 forwardMovement = transform.forward * input.y * currentSpeed;
            moveDirection = new Vector3(forwardMovement.x, moveDirection.y, forwardMovement.z);
        }

        OnMove?.Invoke(input.x, input.y, isRunning);
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