using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAI : MonoBehaviour
{
    public Transform player;
    public float attackRange = 2f;
    public float attackDelay = 1.5f;
    public float chaseSpeed = 3.5f;
    public float attackSpeed = 0f;
    
    private NavMeshAgent agent;
    private bool isPreparingAttack = false;
    private bool isAttacking = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
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

    public bool ChasePlayer()
    {
        if (isPreparingAttack || isAttacking)
        {
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        return true;
            
    }

    private IEnumerator PrepareAttack()
    {
        isPreparingAttack = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(attackDelay);

        if (Vector3.Distance(transform.position, player.position) <= attackRange)
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
        
        agent.speed = attackSpeed;
        Debug.Log("Enemy attacks!");

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        ChasePlayer();
    }
}