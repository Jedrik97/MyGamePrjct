using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 3.5f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private float jumpForce = 1f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private bool _isGrounded;
    private Transform cameraTransform;
    private Animator animator;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        PlayerInput input = GetComponent<PlayerInput>();
        input.OnMove += HandleMovement;
        input.OnJump += HandleJump;
    }

    private void Update()
    {
        ApplyGravity();
    }

    private void HandleMovement(Vector2 input, bool isRunning)
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        bool isMovingBackward = input.y < 0;

        if (input.magnitude > 0.1f)
        {
            Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;
            Vector3 worldMove = cameraTransform.forward * moveDirection.z + cameraTransform.right * moveDirection.x;
            worldMove.y = 0;
            worldMove.Normalize();
            _controller.Move(worldMove * currentSpeed * Time.deltaTime);

            if (!isMovingBackward)
            {
                Quaternion targetRotation = Quaternion.LookRotation(worldMove);
                transform.rotation =
                    Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Передаём параметры в анимацию
            animator.SetFloat("MoveSpeed", currentSpeed);
            animator.SetBool("IsMovingBackward", isMovingBackward);
        }
        else
        {
            animator.SetFloat("MoveSpeed", 0);
        }
    }

    private void HandleJump()
    {
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && Input.GetButtonDown("Jump"))
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetBool("IsJumping", true);

            // Выбираем анимацию в зависимости от движения
            if (animator.GetFloat("MoveSpeed") > 0)
            {
                animator.Play("JumpOnMove");
            }
            else
            {
                animator.Play("JumpOnPlace");
            }
        }
    }

    private void ApplyGravity()
    {
        bool wasGrounded = _isGrounded;
        _isGrounded = _controller.isGrounded;

        if (_isGrounded)
        {
            if (!wasGrounded) // Проверяем, был ли персонаж в воздухе
            {
                animator.SetBool("IsJumping", false);
            }

            _velocity.y = -2f;
        }

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}