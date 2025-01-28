using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    // Радиус обзора
    public float _viewRadius;

    // Угол обзора в градусах
    [Range(0, 360)] public float _viewAngle;

    // Маски слоёв для целей и препятствий
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    // Список видимых целей
    public List<Transform> _targets = new List<Transform>();

    // Переменные для построения меша обзора
    private Mesh _viewMesh;
    public float _meshResolution;
    public MeshFilter _viewMeshFilter;

    private void Start()
    {
        // Инициализация меша и запуск поиска целей с задержкой
        _viewMesh = new Mesh();
        _viewMesh.name = "F0V Mesh";
        _viewMeshFilter.mesh = _viewMesh;
        StartCoroutine(nameof(FindTargetWithDelay), 0.2f);
    }

    IEnumerator FindTargetWithDelay(float seconds)
    {
        // Периодический поиск целей с заданным интервалом
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            GetVisibleTarget();
        }
    }

    private void Update()
    {
        // Построение меша обзора каждый кадр
        DrawFieldOfView();
    }

    private void DrawFieldOfView()
    {
        // Определение количества шагов для построения обзора
        int stepCount = Mathf.RoundToInt(_viewAngle * _meshResolution);
        float angleStep = _viewAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        // Проход по всем углам обзора
        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - _viewAngle / 2 + angleStep * i;
            ViewCastInfo newViewCast = ViewCast(angle);
            viewPoints.Add(newViewCast.point);
        }

        // Создание вершин и треугольников для меша
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Присвоение вершин и треугольников мешу
        _viewMesh.Clear();
        _viewMesh.vertices = vertices;
        _viewMesh.triangles = triangles;
        _viewMesh.RecalculateNormals();
    }

    private void GetVisibleTarget()
    {
        // Очистка списка видимых целей
        _targets.Clear();

        // Поиск всех целей в радиусе обзора
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);
        HashSet<Transform> uniqueTargets = new HashSet<Transform>();

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;

            // Пропуск дубликатов
            if (uniqueTargets.Contains(target))
                continue;

            // Определение направления к цели
            Vector3 directionToTarget = (target.position - transform.position).normalized;

            // Проверка, попадает ли цель в угол обзора
            if (Vector3.Angle(transform.forward, directionToTarget) < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                // Проверка на наличие препятствий между объектом и целью
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    _targets.Add(target);
                    uniqueTargets.Add(target); // Добавление уникальной цели
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool isAngleIsGlobal)
    {
        // Преобразование угла в вектор направления
        if (!isAngleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private ViewCastInfo ViewCast(float angle)
    {
        // Ловим препятствия на линии зрения
        Vector3 dir = DirectionFromAngle(angle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, _viewRadius, _obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, angle);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * _viewRadius, _viewRadius, angle);
        }
    }
}

// Структура для хранения данных о луче зрения
public struct ViewCastInfo
{
    public bool hit;          // Флаг попадания в препятствие
    public Vector3 point;     // Точка попадания
    public float distance;   // Расстояние до препятствия
    public float angle;      // Угол направления

    public ViewCastInfo(bool Hit, Vector3 Point, float Distance, float Angle)
    {
        hit = Hit;
        point = Point;
        distance = Distance;
        angle = Angle;
    }
}