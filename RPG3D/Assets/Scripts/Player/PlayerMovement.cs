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

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement()
    {
        float moveVertical = Input.GetAxis("Vertical");   // W/S (вперед-назад)
        float moveHorizontal = Input.GetAxis("Horizontal"); // A/D (повороты)
        bool isRunning = Input.GetKey(KeyCode.LeftShift); // Бег

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (moveVertical != 0)
        {
            Vector3 moveDirection = transform.forward * moveVertical;
            _controller.Move(moveDirection * currentSpeed * Time.deltaTime);
        }

        if (moveHorizontal != 0)
        {
            transform.Rotate(Vector3.up, moveHorizontal * rotationSpeed * Time.deltaTime);
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