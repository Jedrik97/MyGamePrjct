using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [Header("Параметры обзора")]
    public float viewRadius = 10f; // Дальность видимости
    [Range(0, 360)] public float viewAngle = 120f; // Угол обзора

    [Header("Настройки обнаружения")]
    public LayerMask targetMask; // Маска объектов, которые можно видеть (например, игрок)
    public LayerMask obstacleMask; // Маска препятствий (например, стены)

    [HideInInspector] public List<Transform> _targets = new List<Transform>();

    private void Start()
    {
        InvokeRepeating(nameof(FindVisibleTargets), 0f, 0.5f); // Обновлять список каждые 0.5 секунды
    }

    private void FindVisibleTargets()
    {
        _targets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        foreach (Collider target in targetsInViewRadius)
        {
            Transform targetTransform = target.transform;
            Vector3 dirToTarget = (targetTransform.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, targetTransform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distanceToTarget, obstacleMask))
                {
                    _targets.Add(targetTransform);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float angleInDegrees, bool isAngleGlobal)
    {
        if (!isAngleGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewRadius);

        Vector3 leftBoundary = DirectionFromAngle(-viewAngle / 2, false);
        Vector3 rightBoundary = DirectionFromAngle(viewAngle / 2, false);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * viewRadius);

        Gizmos.color = Color.green;
        foreach (Transform target in _targets)
        {
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}
