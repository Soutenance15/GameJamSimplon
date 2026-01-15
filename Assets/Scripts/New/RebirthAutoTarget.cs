using System.Collections.Generic;
using UnityEngine;

public class RebirthAutoTarget : MonoBehaviour
{
    public float detectionRadius = 5f;
    public Transform firePoint;
    public Transform droneBody; // pour l'orientation
    public LayerMask enemyLayer;

    private List<Transform> targetsInRange = new List<Transform>();
    public Transform currentTarget;

    void Update()
    {
        FindClosestTarget();

        if (currentTarget != null)
        {
            Vector2 direction = currentTarget.position - firePoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            droneBody.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void FindClosestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, enemyLayer);
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit.transform;
            }
        }

        currentTarget = closest;
    }

    public bool HasTarget()
    {
        return currentTarget != null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
