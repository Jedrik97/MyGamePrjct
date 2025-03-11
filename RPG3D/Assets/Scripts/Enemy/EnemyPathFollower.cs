using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyPathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float reachThreshold = 0.5f;
    public float returnThreshold = 15f;
    
    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private Vector3 lastPatrolPoint;
    private bool isChasing = false;

    private NavMeshAgent navMeshAgent;
    private FieldOfView fieldOfView;
    private EnemyMeleeAI meleeAI;
    private EnemyRangedAI rangedAI;
    private Enemy enemy;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        fieldOfView = GetComponent<FieldOfView>();
        meleeAI = GetComponent<EnemyMeleeAI>();
        rangedAI = GetComponent<EnemyRangedAI>();
        enemy = GetComponent<Enemy>();

        if (waypoints.Length > 0)
        {
            lastPatrolPoint = waypoints[0].position;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void Update()
    {
        if (fieldOfView._targets.Count > 1) // Индекс 1 - игрок без препятствий
        {
            StartChase(fieldOfView._targets[1]);
        }
        else if (isChasing)
        {
            if (Vector3.Distance(transform.position, lastPatrolPoint) > returnThreshold)
            {
                StopChaseAndReturn();
            }
        }
        else
        {
            Patrol();
        }
    }

    private void StartChase(Transform player)
    {
        isChasing = true;
        lastPatrolPoint = transform.position;
        navMeshAgent.isStopped = true;
        
        if (meleeAI != null)
            meleeAI.SetTarget(player);
        else if (rangedAI != null)
            rangedAI.SetTarget(player);
    }

    private void StopChaseAndReturn()
    {
        isChasing = false;
        if (meleeAI != null) meleeAI.ResetTarget();
        if (rangedAI != null) rangedAI.ResetTarget();
        
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(lastPatrolPoint);
        
        StartCoroutine(GradualHeal());
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;
        
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= reachThreshold)
        {
            if (movingForward)
            {
                if (currentWaypointIndex < waypoints.Length - 1)
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
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private IEnumerator GradualHeal()
    {
        float healDuration = 3f;
        float healAmount = enemy.maxHealth / healDuration;
        
        for (float t = 0; t < healDuration; t += Time.deltaTime)
        {
            enemy.Heal((int)(healAmount * Time.deltaTime));
            yield return null;
        }
    }
}
