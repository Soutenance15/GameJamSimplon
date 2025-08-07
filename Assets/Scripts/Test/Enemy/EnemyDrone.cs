using UnityEngine;

public class EnemyDrone : Enemy
{
    [Header("Ligne de vol")]
    public float xMin = -4f;
    public float xMax = 4f;
    public float yLevel = 3f;
    public float flySpeed = 2.5f;

    [Header("Corps à pivoter")]
    public Transform body; // À assigner dans l’inspecteur (pivot visuel et canon)

    [Header("Détection/ciblage")]
    public float detectionRange = 7f;
    public float stopDistance = 0.2f;
    public Transform player;

    [Header("Tir")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1.2f;
    private float fireCooldown = 0f;

    private int moveDir = 1; // 1=droite, -1=gauche
    private bool targetingPlayer = false;
    private float currentAngle = 0f;

    public override void Start()
    {
        base.Start();
        // RÉGLAGE PHYSIQUE PRO : empêche tout drift vertical
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // ou rb.gravityScale = 0f;
        }
        if (player == null)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                player = go.transform;
        }
        if (body == null)
            Debug.LogWarning("body n'est pas assigné sur EnemyShooterFlyer!");
        if (firePoint == null)
            Debug.LogWarning("firePoint non assigné sur EnemyShooterFlyer!");
    }

    public override void FixedUpdate()
    {
        Vector3 pos = transform.position;
        pos.y = yLevel;
        transform.position = pos;

        Vector2 droneToPlayer = player != null ? (player.position - transform.position) : Vector2.zero;
        float distToPlayer = droneToPlayer.magnitude;
        targetingPlayer = (player != null && distToPlayer <= detectionRange);

        float targetX = transform.position.x;

        if (targetingPlayer)
        {
            float playerXClamp = Mathf.Clamp(player.position.x, xMin, xMax);
            float diffX = playerXClamp - transform.position.x;

            if (Mathf.Abs(diffX) > stopDistance)
            {
                float dir = Mathf.Sign(diffX);
                targetX = transform.position.x + dir * flySpeed * Time.fixedDeltaTime;
                targetX = Mathf.Clamp(targetX, xMin, xMax);
                moveDir = (int)dir;
            }
            else
            {
                targetX = transform.position.x;
            }
        }
        else
        {
            targetX = transform.position.x + moveDir * flySpeed * Time.fixedDeltaTime;
            if (targetX <= xMin)
            {
                targetX = xMin;
                moveDir = 1;
            }
            else if (targetX >= xMax)
            {
                targetX = xMax;
                moveDir = -1;
            }
        }
        transform.position = new Vector3(targetX, yLevel, transform.position.z);

        if (body != null && player != null)
        {
            Vector2 dirVec = (player.position - transform.position).normalized;
            float angle = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg;
            currentAngle = Mathf.LerpAngle(currentAngle, angle, 0.2f);
            body.rotation = Quaternion.Euler(0, 0, currentAngle);
        }

        if (targetingPlayer && fireCooldown <= 0f && projectilePrefab != null && firePoint != null)
        {
            ShootAtPlayer();
            fireCooldown = fireRate;
        }
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;

        if (healthBarCanvas != null)
        {
            healthBarCanvas.transform.rotation = Quaternion.identity;
            healthBarCanvas.transform.position = transform.position + new Vector3(0f, 1.1f, 0f);
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        var projScript = proj.GetComponent<ProjectileBasic>();
        if (projScript != null)
            projScript.Init(direction, ProjectileSource.Enemy);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
