using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyMeleeAI : MonoBehaviour
{
    public Transform player;
    
    private NavMeshAgent agent;
    private bool isPreparingAttack = false;
    private bool isAttacking = false;
    private Enemy enemy;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > enemy.attackRange)
        {
            ChasePlayer();
        }
        else
        {
            if (!isPreparingAttack && !isAttacking)
            {
                StartCoroutine(PrepareAttack());
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        player = newTarget;
    }

    private void ChasePlayer()
    {
        if (!isAttacking && !isPreparingAttack)
        {
            agent.speed = enemy.chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    private IEnumerator PrepareAttack()
    {
        isPreparingAttack = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(enemy.attackDelay);

        if (player != null)
        {
            StartCoroutine(Attack());
        }
        else
        {
            isPreparingAttack = false;
            ChasePlayer();
        }
    }

    private IEnumerator Attack()
    {
        isPreparingAttack = false;
        isAttacking = true;
        agent.speed = enemy.attackSpeed;
        Debug.Log("Enemy attacks!");
        
        yield return new WaitForSeconds(0.5f);
        
        if (player != null && Vector3.Distance(transform.position, player.position) <= enemy.attackRange)
        {
            player.GetComponent<HealthPlayerController>()?.TakeDamage(enemy.attackDamage);
        }
        
        isAttacking = false;
        ChasePlayer();
    }

    public void ResetTarget()
    {
        player = null;
        isPreparingAttack = false;
        isAttacking = false;
        agent.isStopped = false;
    }
}
