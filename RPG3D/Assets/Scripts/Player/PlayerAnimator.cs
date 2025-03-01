using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private void OnEnable()
    {
        PlayerMovement.OnMove += HandleMovement;
        PlayerMovement.OnJump += HandleJump;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        animator.SetFloat("WalkX", 0);
        animator.SetFloat("WalkZ", 0);
        animator.SetBool("Idle", true);
        animator.SetBool("Jump", false);
    }

    private void HandleMovement(float x, float z)
    {
        bool isIdle = x == 0 && z == 0;

        animator.SetFloat("WalkX", x);
        animator.SetFloat("WalkZ", z);
        animator.SetBool("Idle", isIdle);
    }

    private void HandleJump(bool isJumping)
    {
        animator.SetBool("Jump", isJumping);
    }

    private void OnDisable()
    {
        PlayerMovement.OnMove -= HandleMovement;
        PlayerMovement.OnJump -= HandleJump;
    }
}