using UnityEngine; 
public class EnemyPathFollower : MonoBehaviour {
    public Transform[] waypoints;     public float speed = 2f;     public float reachThreshold = 0.2f;     private int currentWaypointIndex = 0;     private bool movingForward = true; 
    private EnemyAI enemyAI; 
    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>();     }

    private void Update()
    {
        if (enemyAI != null && enemyAI.isChasing)         {
            return;
        }
        Patrol();     }

    private void Patrol()     {
        if (waypoints.Length == 0) return;         
        Transform targetWaypoint = waypoints[currentWaypointIndex];         Vector3 direction = (targetWaypoint.position - transform.position).normalized;         transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);         
        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold)         {
            if (movingForward)             {
                if (currentWaypointIndex < waypoints.Length - 1)                 {
                    currentWaypointIndex++;                 }
                else
                {
                    movingForward = false;                     currentWaypointIndex--;
                }
            }
            else             {
                if (currentWaypointIndex > 0)
                {
                    currentWaypointIndex--;                 }
                else
                {
                    movingForward = true;                     currentWaypointIndex++;
                }
            }
        }
        transform.LookAt(targetWaypoint);     }
}
