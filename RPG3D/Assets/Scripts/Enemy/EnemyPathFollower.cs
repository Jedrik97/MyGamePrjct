using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyPathFollower : MonoBehaviour
{
    public Transform[] Waypoints;
    public float reachThreshold = 0.5f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private Vector3 lastPatrolPoint;
    private bool isChasing = false;
    private Vector3 chaseStartPoint;

    private NavMeshAgent navMeshAgent;
    private FieldOfView fieldOfView;
    private EnemyMeleeAI meleeAI;
    private Enemy enemy;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
        meleeAI = GetComponent<EnemyMeleeAI>();
        enemy = GetComponent<Enemy>();

        if (Waypoints.Length > 0)
        {
            lastPatrolPoint = Waypoints[0].position;
            navMeshAgent.SetDestination(Waypoints[currentWaypointIndex].position);
        }

        if (meleeAI != null)
        {
            meleeAI.OnReturnToPatrol += StopChaseAndReturn;
        }
    }

    private void OnDisable()
    {
        if (meleeAI != null)
        {
            meleeAI.OnReturnToPatrol -= StopChaseAndReturn;
        }
    }

    private void Update()
    {
        // Проверяем, есть ли хоть одна цель в поле зрения
        if (fieldOfView._targets.Count > 0 && fieldOfView._targets[0] != null)
        {
            Transform playerTransform = fieldOfView._targets[0];

            // Записываем первую обнаруженную цель в EnemyMeleeAI
            if (meleeAI != null)
            {
                StartChase(playerTransform);
            }
        }
        else
        {
            // Если целей нет — сбрасываем преследование
            if (isChasing)
            {
                StartCoroutine(DelayedReturn());
            }
        }

        if (!isChasing)
        {
            Patrol();
        }
    }
    private void StartChase(Transform player)
    {
        if (!isChasing)
        {
            isChasing = true;
            chaseStartPoint = transform.position;
            lastPatrolPoint = transform.position;
            navMeshAgent.isStopped = true;

            if (meleeAI != null)
            {
                meleeAI.SetTarget(player);
                //Debug.Log($"[EnemyPathFollower] Передан игрок в EnemyMeleeAI: {player.name}");
            }
        }
    }

    private IEnumerator DelayedReturn()
    {
        isChasing = false;
        navMeshAgent.isStopped = true;
        yield return new WaitForSeconds(0.5f);
        StopChaseAndReturn();
    }

    private void StopChaseAndReturn()
    {
        isChasing = false;

        if (meleeAI)
        {
            meleeAI.ResetTarget();
            //Debug.Log("[EnemyPathFollower] Враг потерял игрока и возвращается к патрулю");
        }

        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(lastPatrolPoint);
        StartCoroutine(enemy.GradualHeal());
    }

    private void Patrol()
    {
        if (Waypoints.Length == 0) return;

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= reachThreshold)
        {
            if (movingForward)
            {
                if (currentWaypointIndex < Waypoints.Length - 1)
                    currentWaypointIndex++;
                else
                    movingForward = false;
            }
            else
            {
                if (currentWaypointIndex > 0)
                    currentWaypointIndex--;
                else
                    movingForward = true;
            }

            navMeshAgent.SetDestination(Waypoints[currentWaypointIndex].position);
        }
    }
}
