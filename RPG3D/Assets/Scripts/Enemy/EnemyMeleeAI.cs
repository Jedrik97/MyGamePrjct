using UnityEngine;

public class EnemyMeleeAI : MonoBehaviour
{
    public float chaseSpeed = 3f;
    public float attackDistance = 2f;
    public float attackCooldown = 2f;
    public int attackDamage = 10;

    private Transform player;
    private CharacterController characterController;
    private float lastAttackTime;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackDistance)
        {
            AttemptAttack();
        }
        else if (distance <= 15f) // Ограничиваем радиус преследования
        {
            ChasePlayer();
        }
    }

    public bool ChasePlayer()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > attackDistance && distance <= 15f)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            characterController.Move(direction * chaseSpeed * Time.deltaTime);
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
            return true; // Chasing player
        }
        return false; // Not chasing player
    }

    private void AttemptAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Invoke(nameof(DealDamage), 1.5f);
        }
    }

    private void DealDamage()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= attackDistance)
        {
            HealthPlayerController health = player.GetComponent<HealthPlayerController>();
            health?.TakeDamage(attackDamage);
        }
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }
}