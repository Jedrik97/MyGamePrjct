using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;

    // Ссылка на камеру для движения относительно её направления
    private Transform cameraTransform;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        ApplyGravity();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float moveVertical = Input.GetAxis("Vertical");   // W/S
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Если ПКМ зажата – движение относительно камеры
        if (Input.GetMouseButton(1))
        {
            Vector3 inputDirection = new Vector3(moveHorizontal, 0f, moveVertical);
            if (inputDirection.sqrMagnitude > 0.01f)
            {
                inputDirection.Normalize();

                // Направления относительно камеры (без компонента Y)
                Vector3 camForward = cameraTransform.forward;
                camForward.y = 0;
                camForward.Normalize();

                Vector3 camRight = cameraTransform.right;
                camRight.y = 0;
                camRight.Normalize();

                // Вычисляем направление движения
                Vector3 moveDirection = (camForward * inputDirection.z) + (camRight * inputDirection.x);
                moveDirection.Normalize();

                _controller.Move(moveDirection * currentSpeed * Time.deltaTime);

                // Плавно поворачиваем персонажа в направлении движения
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
        else // Если ПКМ не зажата – стандартное поведение
        {
            if (Mathf.Abs(moveVertical) > 0.01f)
            {
                Vector3 moveDirection = transform.forward * moveVertical;
                _controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
            if (Mathf.Abs(moveHorizontal) > 0.01f)
            {
                transform.Rotate(Vector3.up, moveHorizontal * rotationSpeed * Time.deltaTime);
            }
        }
    }

    private void HandleJump()
    {
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    private void ApplyGravity()
    {
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}
