using UnityEngine;
using System.Collections;

// Скрипт для управления поведением вражеского AI
public class EnemyAI : MonoBehaviour
{
    // Скорость преследования
    public float chaseSpeed = 5f;
    // Нормальная скорость передвижения
    public float normalSpeed = 2f;
    // Ссылка на игрока (цель для преследования)
    private Transform player;
    // Флаг, указывающий, преследует ли враг цель
    public bool isChasing = false;
    // Радиус обзора врага
    public float viewRadius;
    // Дистанция проверки препятствий
    public float obstacleCheckDistance = 1f;

    // Ссылка на компонент FieldOfView (обзор поля зрения)
    private FieldOfView fieldOfView;

    // Инициализация компонентов
    private void Start()
    {
        // Получаем компонент FieldOfView, прикреплённый к объекту
        fieldOfView = GetComponent<FieldOfView>();
    }

    // Обновление состояния каждый кадр
    private void Update()
    {
        // Проверяем, есть ли цели в зоне видимости
        if (fieldOfView._targets.Count > 0)
        {
            // Устанавливаем первую найденную цель как игрока
            player = fieldOfView._targets[0];
            isChasing = true; // Включаем преследование
        }
        else
        {
            isChasing = false; // Отключаем преследование, если цели нет
        }

        // Если враг преследует и цель существует, продолжаем движение
        if (isChasing && player != null)
        {
            ChasePlayer(); // Запускаем логику преследования
        }
    }

    // Метод для преследования игрока
    private void ChasePlayer()
    {
        // Если игрок пропал, отключаем преследование
        if (player == null)
        {
            isChasing = false; 
            return;
        }

        // Определяем направление к игроку
        Vector3 direction = (player.position - transform.position).normalized;
        // Считаем расстояние до игрока
        float distance = Vector3.Distance(transform.position, player.position);

        // Если игрок вышел за пределы радиуса обзора, отключаем преследование
        if (distance > viewRadius) 
        {
            isChasing = false; 
        }
        else
        {
            // Проверяем наличие препятствий между врагом и игроком
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, obstacleCheckDistance))
            {
                // Если столкнулись с препятствием, прекращаем движение
                if (hit.collider != null)
                {
                    return;
                }
            }
            
            // Перемещаем врага в сторону игрока с заданной скоростью
            transform.position = Vector3.MoveTowards(transform.position, player.position, chaseSpeed * Time.deltaTime);
            // Поворачиваем врага лицом к игроку
            transform.LookAt(player);
        }
    }
}
