using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 700f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Camera _camera;

    private bool _isGrounded;
    private CharacterController _controller;
    private Vector3 _velocity;
    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        
    }

    private void FixedUpdate()
    {
        Jump();
        Movement();
        
    }

    private void Movement()
    {
        float horizontal = Input.GetAxis("Horizontal");     
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical).normalized;
        if (movement.magnitude >= 0.1f && _isGrounded)
        {
            Vector3 moveDirection = Quaternion.Euler(0, _camera.transform.rotation.eulerAngles.y, 0) * movement;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(moveDirection), rotationSpeed * Time.deltaTime);
            
            _controller.Move(moveDirection * movementSpeed * Time.deltaTime);
        }

        if (!_isGrounded)
        {
            _velocity.y += gravity * Time.deltaTime;
        }
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void Jump()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                
        }
            
    }
}