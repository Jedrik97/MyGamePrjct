using UnityEngine;

public class EnemyRangedAI : MonoBehaviour
{
    public float chaseSpeed = 2f;
    public float shootingDistance = 10f;
    public float attackCooldown = 2f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int attackDamage = 5;

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

        if (distance <= shootingDistance)
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
        if (distance > shootingDistance && distance <= 15f)
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
            Invoke(nameof(Shoot), 1.5f);
        }
    }

    private void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            bullet.GetComponent<Bullet>().SetDamage(attackDamage);
        }
    }

    public void SetTarget(Transform target)
    {
        player = target;
    }
}