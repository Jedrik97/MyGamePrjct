using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float chaseSpeed = 5f;
    public float normalSpeed = 2f;
    private Transform player;
    public bool isChasing = false;
    public float viewRadius;
    public float attackDistance = 2f; // Дистанция для атаки
    public float attackDelay = 1f; // Задержка перед атакой
    public float attackCooldown = 2f; // Задержка между атаками
    public int attackDamage = 10; // Урон, наносимый игроку
    public float obstacleCheckDistance = 1f;

    private FieldOfView fieldOfView;
    private CharacterController characterController;
    private Animator animator; // Анимации

    private float lastAttackTime;

    private void Start()
    {
        fieldOfView = GetComponent<FieldOfView>();
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController is missing on EnemyAI!");
        }

        if (animator == null)
        {
            Debug.LogError("Animator is missing on EnemyAI!");
        }
    }

    private void Update()
    {
        if (fieldOfView._targets.Count > 0)
        {
            player = fieldOfView._targets[0];
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackDistance)
            {
                AttemptAttack();
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    private void ChasePlayer()
    {
        if (player == null)
        {
            isChasing = false;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // Stop chasing if the player is out of view radius
        if (distance > viewRadius)
        {
            isChasing = false;
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, obstacleCheckDistance))
            {
                if (hit.collider != null && hit.collider.transform != player)
                {
                    // Obstacle in the way; stop chasing
                    return;
                }
            }

            // Move towards the player using CharacterController
            Vector3 move = direction * chaseSpeed * Time.deltaTime;
            characterController.Move(move);

            // Rotate to look at the player
            Vector3 lookDirection = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(lookDirection);

            // Optionally play run animation
            animator?.SetBool("isRunning", true);
        }
    }

    private void AttemptAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time + attackDelay;

            // Play attack animation
            animator?.SetTrigger("Attack");

            // Wait for the attack delay to apply damage
            Invoke(nameof(DealDamage), attackDelay);
        }

        // Stop running animation if playing
        animator?.SetBool("isRunning", false);
    }

    private void DealDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            // Access player's health and deal damage
            HealthPlayerController health = player.GetComponent<HealthPlayerController>();
            if (health != null)
            {
                health.TakeDamage(attackDamage);
            }
        }
    }
}
