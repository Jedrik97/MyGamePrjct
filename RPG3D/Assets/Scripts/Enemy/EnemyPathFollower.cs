using UnityEngine;

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

    private void Start()
    {
        meleeAI = GetComponent<EnemyMeleeAI>();
        rangedAI = GetComponent<EnemyRangedAI>();
        fieldOfView = GetComponent<FieldOfView>();
        enemyParameters = GetComponent<Enemy>(); // Инициализируем Enemy

        lastPatrolPoint = waypoints.Length > 0 ? waypoints[0].position : transform.position;
    }

    private void Update()
    {
        // Проверяем, есть ли игрок в поле зрения
        bool playerInSight = fieldOfView._targets.Count > 0;

        if (playerInSight && !isChasing)
        {
            // Устанавливаем первую цель из списка как игрока
            Transform player = fieldOfView._targets[0];
            if (meleeAI != null) meleeAI.SetTarget(player);
            if (rangedAI != null) rangedAI.SetTarget(player);
        }

        // Проверяем, преследует ли враг игрока
        isChasing = playerInSight || 
                    (meleeAI != null && meleeAI.ChasePlayer()) || 
                    (rangedAI != null && rangedAI.ChasePlayer());

        if (isChasing)
        {
            lastPatrolPoint = transform.position; // Запоминаем точку, где начали преследование
            returningToPatrol = false;
            return;
        }

        // Если игрок вне зоны досягаемости, сбрасываем цель
        if (!playerInSight)
        {
            if (meleeAI != null) meleeAI.SetTarget(null);
            if (rangedAI != null) rangedAI.SetTarget(null);
        }

        if (Vector3.Distance(transform.position, lastPatrolPoint) > returnThreshold)
        {
            returningToPatrol = true;
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
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold)
        {
            if (movingForward)
            {
                if (currentWaypointIndex < waypoints.Length - 1)
                    currentWaypointIndex++;
                else
                {
                    movingForward = false;
                    currentWaypointIndex--;
                }
            }
            else
            {
                if (currentWaypointIndex > 0)
                    currentWaypointIndex--;
                else
                {
                    movingForward = true;
                    currentWaypointIndex++;
                }
            }
        }
    }

    private void ReturnToPatrol()
    {
        MoveTowards(lastPatrolPoint);

        if (Vector3.Distance(transform.position, lastPatrolPoint) < reachThreshold)
        {
            returningToPatrol = false;
            enemyParameters?.ResetHealth();
            // Находим ближайший waypoint для продолжения патруля
            currentWaypointIndex = FindNearestWaypoint();
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
        return nearestIndex;
    }

    private void MoveTowards(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        transform.LookAt(target); // Оставляем только здесь, чтобы не конфликтовать с AI
    }
}