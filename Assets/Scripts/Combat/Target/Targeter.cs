using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public List<Target> targets = new List<Target>();

    private void Update()
    {
        GetClosestEnemyDirection();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target))
        {
            return;
        }

        targets.Add(target);
        target.OnDestroyed += RemoveTarget;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target))
        {
            return;
        }

        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }

    private void RemoveTarget(Target target)
    {
        target.OnDestroyed -= RemoveTarget;
        targets.Remove(target);
    }

    private Target GetClosestEnemy()
    {
        Target closestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        Transform cameraTransform = Camera.main.transform;
        Vector3 cameraForward = cameraTransform.forward;

        foreach (Target target in targets)
        {
            Vector3 directionToTarget = (target.transform.position - currentPosition).normalized;
            float dot = Vector3.Dot(cameraForward, directionToTarget);

            if (dot > 0)
            {
                float distanceSqrToTarget = (target.transform.position - currentPosition).sqrMagnitude;

                if (distanceSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqrToTarget;
                    closestTarget = target;
                }
            }
        }

        return closestTarget;
    }

    public Vector3 GetClosestEnemyDirection()
    {
        Target closestTarget = GetClosestEnemy();

        if (closestTarget != null)
        {
            Vector3 directionToClosestTarget = closestTarget.transform.position - transform.position;
            directionToClosestTarget.y = 0f;
            return directionToClosestTarget.normalized;
        }

        return Vector3.zero;
    }

    public Vector3 GetClosestEnemyPosition()
    {
        Target closestTarget = GetClosestEnemy();

        if (closestTarget != null)
        {
            return closestTarget.transform.position;
        }

        return Vector3.zero;
    }

    public bool EnemyDragon()
    {
        Target closestTarget = GetClosestEnemy();
        return closestTarget != null && closestTarget.CompareTag("Dragon");
    }

}
