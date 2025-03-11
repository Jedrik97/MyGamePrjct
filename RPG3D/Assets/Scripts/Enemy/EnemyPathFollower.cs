using UnityEngine;
using UnityEngine.AI;

public class EnemyPathFollower : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public float reachThreshold = 0.2f;
    public float returnThreshold = 15f;

    private int currentWaypointIndex = 0;
    private bool movingForward = true;
    private Vector3 lastPatrolPoint;
    private bool returningToPatrol = false;

    private Enemy enemyParameters;
    private EnemyMeleeAI meleeAI;
    private EnemyRangedAI rangedAI;
    private FieldOfView fieldOfView;

    private bool isChasing = false;

    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        meleeAI = GetComponent<EnemyMeleeAI>();
        rangedAI = GetComponent<EnemyRangedAI>();
        fieldOfView = GetComponent<FieldOfView>();
        enemyParameters = GetComponent<Enemy>();

        navMeshAgent = GetComponent<NavMeshAgent>();

        if (navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component is missing!");
            return;
        }

        navMeshAgent.speed = speed;
        navMeshAgent.stoppingDistance = reachThreshold;

        lastPatrolPoint = waypoints.Length > 0 ? waypoints[0].position : transform.position;

        Debug.Log("Enemy initialized. Starting patrol.");
        Patrol();
    }

    private void Update()
    {
        bool playerInSight = fieldOfView._targets.Count > 0;
        Debug.Log($"Player in sight: {playerInSight}, Targets count: {fieldOfView._targets.Count}");

        if (playerInSight && !isChasing)
        {
            Debug.Log("Player detected. Starting chase.");
            Transform player = fieldOfView._targets[0];
            if (meleeAI != null) meleeAI.SetTarget(player);
            if (rangedAI != null) rangedAI.SetTarget(player);
            isChasing = true;
        }
        else if (!playerInSight && isChasing)
        {
            Debug.Log("Player lost. Stopping chase.");
            isChasing = false;
            returningToPatrol = true;
        }

        if (isChasing)
        {
            Debug.Log("Chasing player.");
            lastPatrolPoint = transform.position;
            return;
        }

        if (returningToPatrol)
        {
            ReturnToPatrol();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned.");
            return;
        }

        Transform targetWaypoint = waypoints[currentWaypointIndex];

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= reachThreshold)
        {
            Debug.Log($"Reached waypoint {currentWaypointIndex}. Moving to next.");
            lastPatrolPoint = targetWaypoint.position;

            if (movingForward)
            {
                if (currentWaypointIndex < waypoints.Length - 1)
                {
                    currentWaypointIndex++;
                }
                else
                {
                    movingForward = false;
                    currentWaypointIndex--;
                }
            }
            else
            {
                if (currentWaypointIndex > 0)
                {
                    currentWaypointIndex--;
                }
                else
                {
                    movingForward = true;
                    currentWaypointIndex++;
                }
            }

            targetWaypoint = waypoints[currentWaypointIndex];
            navMeshAgent.SetDestination(targetWaypoint.position);
        }
        else if (navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Debug.LogWarning("Path is blocked. Skipping to next waypoint.");
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            navMeshAgent.SetDestination(waypoints[currentWaypointIndex].position);
        }
    }

    private void ReturnToPatrol()
    {
        Debug.Log("Returning to last patrol point.");
        NavMeshPath path = new NavMeshPath();
        if (navMeshAgent.CalculatePath(lastPatrolPoint, path))
        {
            navMeshAgent.SetDestination(lastPatrolPoint);
        }
        else
        {
            Debug.LogWarning("Cannot find path to last patrol point. Resetting patrol.");
            returningToPatrol = false;
            currentWaypointIndex = FindNearestWaypoint();
            Patrol();
        }

        if (Vector3.Distance(transform.position, lastPatrolPoint) < reachThreshold)
        {
            Debug.Log("Returned to patrol path. Resuming patrol.");
            returningToPatrol = false;
            enemyParameters?.ResetHealth();
            currentWaypointIndex = FindNearestWaypoint();
            Patrol();
        }
    }

    private int FindNearestWaypoint()
    {
        int nearestIndex = 0;
        float minDistance = float.MaxValue;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, waypoints[i].position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        Debug.Log($"Nearest waypoint found: {nearestIndex}");
        return nearestIndex;
    }
}