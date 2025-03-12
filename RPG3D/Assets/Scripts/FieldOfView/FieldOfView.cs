using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{
    public float _viewRadius;
    [Range(0, 360)] public float _viewAngle;
    
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    public List<Transform> _targets = new List<Transform>();

    private void Start()
    {
        StartCoroutine(nameof(FindTargetWithDelay), 0.2f);
    }
    IEnumerator FindTargetWithDelay(float seconds)
    {
        while (true)
        {
            yield return new WaitForSeconds(seconds);
            GetVisibleTarget();
        }
    }

    private void GetVisibleTarget()
    {
        _targets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, _viewRadius, _targetMask);
        HashSet<Transform> uniqueTargets = new HashSet<Transform>();

        foreach (Collider targetCollider in targetsInViewRadius)
        {
            Transform target = targetCollider.transform;
            if (uniqueTargets.Contains(target))
                continue;

            Vector3 directionToTarget = (target.position - transform.position).normalized;
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            if (angleToTarget < _viewAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, _obstacleMask))
                {
                    _targets.Add(target);
                    uniqueTargets.Add(target);
                }
            }
        }
    }

    public Vector3 DirectionFromAngle(float verticalAngle, float horizontalAngle)
    {
        float verticalRad = verticalAngle * Mathf.Deg2Rad;
        float horizontalRad = horizontalAngle * Mathf.Deg2Rad;

        return new Vector3(
            Mathf.Sin(horizontalRad) * Mathf.Cos(verticalRad),
            Mathf.Sin(verticalRad),
            Mathf.Cos(horizontalRad) * Mathf.Cos(verticalRad)
        );
    }
}
