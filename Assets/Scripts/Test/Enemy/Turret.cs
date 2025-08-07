using UnityEngine;

public class Turret : MonoBehaviour
{
    public Transform body;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float detectionRange = 7f;
    public float fireRate = 1f;
    public Transform player;
    private float fireCooldown;

    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (player == null)
            return;
        float dist = Vector2.Distance(transform.position, player.position);
        bool isPlayerInRange = dist < detectionRange;

        // Rotation canon
        if (isPlayerInRange && body != null)
        {
            Vector2 direction = (player.position - body.position);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float newAngle = Mathf.LerpAngle(body.eulerAngles.z, angle, 0.2f);
            body.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }

        // Tir automatique
        if (isPlayerInRange && fireCooldown <= 0f && projectilePrefab != null && firePoint != null)
        {
            ShootAtPlayer();
            fireCooldown = fireRate;
        }
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    void ShootAtPlayer()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        var projScript = proj.GetComponent<ProjectileBasic>();
        if (projScript != null)
            projScript.Init(dir, ProjectileSource.Enemy);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
