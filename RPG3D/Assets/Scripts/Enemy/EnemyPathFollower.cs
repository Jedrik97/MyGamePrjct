using UnityEngine; // Подключаем пространство имён Unity.

public class EnemyPathFollower : MonoBehaviour // Класс движения врага по патрульным точкам.
{
    public Transform[] waypoints; // Массив патрульных точек.
    public float speed = 2f; // Скорость движения.
    public float reachThreshold = 0.2f; // Расстояние для перехода к следующей точке.
    private int currentWaypointIndex = 0; // Индекс текущей точки маршрута.
    private bool movingForward = true; // Направление движения (вперёд/назад).

    private EnemyAI enemyAI; // Ссылка на AI врага.

    private void Start()
    {
        enemyAI = GetComponent<EnemyAI>(); // Получаем компонент AI.
    }

    private void Update()
    {
        if (enemyAI != null && enemyAI.isChasing) // Если враг преследует цель, патрулирование прерывается.
        {
            return;
        }
        Patrol(); // Иначе продолжаем патрулирование.
    }

    private void Patrol() // Логика патрулирования.
    {
        if (waypoints.Length == 0) return; // Если нет точек, выходим.
        
        Transform targetWaypoint = waypoints[currentWaypointIndex]; // Получаем текущую точку назначения.
        Vector3 direction = (targetWaypoint.position - transform.position).normalized; // Вычисляем направление движения.
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime); // Двигаемся к точке.
        
        if (Vector3.Distance(transform.position, targetWaypoint.position) < reachThreshold) // Если точка достигнута.
        {
            if (movingForward) // Если движемся вперёд.
            {
                if (currentWaypointIndex < waypoints.Length - 1) // Если не достигнут конец маршрута.
                {
                    currentWaypointIndex++; // Переход к следующей точке.
                }
                else
                {
                    movingForward = false; // Меняем направление.
                    currentWaypointIndex--;
                }
            }
            else // Если движемся назад.
            {
                if (currentWaypointIndex > 0)
                {
                    currentWaypointIndex--; // Переход к предыдущей точке.
                }
                else
                {
                    movingForward = true; // Меняем направление вперёд.
                    currentWaypointIndex++;
                }
            }
        }
        transform.LookAt(targetWaypoint); // Поворачиваемся к следующей точке.
    }
}
