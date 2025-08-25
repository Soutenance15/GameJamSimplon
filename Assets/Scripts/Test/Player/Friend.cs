using UnityEngine;

public class Friend : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab; // Préfabriqué du projectile
    public Transform firePoint; // Position de tir
    public float fireRate = 5f; // Coups par seconde
    float lastShotTime = 0f;

    [Header("Suivi du joueur")]
    public Transform player; // Cible à suivre
    public float speed = 4f; // Vitesse de déplacement
    public Vector2 followOffset = new Vector2(1.5f, 2f); // Décalage autour du joueur

    [Header("Évitement d'obstacle")]
    public float obstacleDetectDistance = 1.5f; // Distance maximum pour détecter un obstacle
    public float obstacleAvoidHeight = 1.5f; // Hauteur pour contourner l'obstacle
    public LayerMask obstacleLayer; // Layer à assigner dans l'inspecteur (ex : Ground)

    [Header("Détection d'ennemis")]
    public string enemyTag = "Enemy"; // Tag utilisé pour les ennemis

    [Header("Stand-by/Immobilisation")]
    public float blockTimeBeforeIdle = 3f; // Temps sans progrès avant immobilisation (secondes)
    public float minDistanceProgress = 0.5f; // Distance minimale à parcourir en blockTime
    public float reactivationDistance = 12f; // Distance du joueur requise pour réactivation

    float followSide = 1f; // -1 = gauche, 1 = droite
    Vector3 lastPlayerPosition;

    // --- Gestion immobilisation ---
    Vector3 lastCheckedPos;
    float blockTimer = 0f;
    bool immobile = false;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0;
        }
        followSide = Mathf.Sign(transform.position.x - player.position.x);
        if (followSide == 0)
            followSide = 1;
        lastPlayerPosition = player.position;
        lastCheckedPos = transform.position;
    }

    void Update()
    {
        if (player == null)
            return;

        // --- Gestion immobilisation si le Friend ne progresse plus ---
        if (!immobile)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer >= blockTimeBeforeIdle)
            {
                float prog = Vector3.Distance(transform.position, lastCheckedPos);
                if (prog < minDistanceProgress)
                    immobile = true;
                blockTimer = 0f;
                lastCheckedPos = transform.position;
            }
        }
        else
        {
            // Essaye de se réactiver si le joueur redevient accessible
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float directDist = Vector2.Distance(transform.position, player.position);
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dirToPlayer,
                directDist,
                obstacleLayer
            );
            if (distToPlayer <= reactivationDistance && hit.collider == null)
            {
                immobile = false;
                blockTimer = 0f;
                lastCheckedPos = transform.position;
            }
        }

        // --- Logique de mouvement uniquement si actif ---
        if (!immobile)
        {
            // Inverse le côté si le joueur change nettement de direction
            float playerSpeedX =
                (player.position.x - lastPlayerPosition.x) / Mathf.Max(Time.deltaTime, 0.0001f);
            if (Mathf.Abs(playerSpeedX) > 0.1f)
                followSide = Mathf.Sign(playerSpeedX);
            lastPlayerPosition = player.position;

            // Calcule la position cible à côté du joueur
            Vector2 offset = new Vector2(Mathf.Abs(followOffset.x) * followSide, followOffset.y);
            Vector3 targetPos = player.position + (Vector3)offset;

            // Évitement d'obstacle : raycast horizontal
            Vector2 moveDir = (targetPos - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                moveDir,
                obstacleDetectDistance,
                obstacleLayer
            );
            if (hit.collider != null)
            {
                targetPos.y = Mathf.Max(targetPos.y, hit.point.y + obstacleAvoidHeight);
                RaycastHit2D ceiling = Physics2D.Raycast(
                    new Vector2(transform.position.x, hit.point.y + obstacleAvoidHeight),
                    Vector2.up,
                    0.5f,
                    obstacleLayer
                );
                if (ceiling.collider != null)
                    targetPos = transform.position;
            }

            float moveDelta = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveDelta);
        }

        // --- Visée automatique et tir (toujours actifs, même immobile) ---
        Transform closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Vector2 dir = (closestEnemy.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.Euler(0, 0, angle),
                0.2f
            );
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, 0.2f);
        }

        bool shootButton = Input.GetKey(KeyCode.LeftControl);
        if (shootButton && Time.time > lastShotTime + 1f / fireRate)
        {
            if (closestEnemy != null && projectilePrefab != null && firePoint != null)
            {
                Vector2 directionProjectile = (
                    closestEnemy.position - firePoint.position
                ).normalized;
                GameObject proj = Instantiate(
                    projectilePrefab,
                    firePoint.position,
                    Quaternion.identity
                );
                // Friend.cs (quand il tire)
                var projScript = proj.GetComponent<ProjectileBasic>();
                if (projScript != null)
                    projScript.Init(directionProjectile, ProjectileSource.Friend);

                // var projScript = proj.GetComponent<ProjectileN>();
                // if (projScript != null)
                //     projScript.Init(directionProjectile);
                // lastShotTime, valeur amodifié pour rendre plus fort ou pas
                lastShotTime = Time.time;
            }
        }
    }

    // Renvoie l'ennemi le plus proche, en ignorant ceux non ciblables
    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float minDistance = Mathf.Infinity;
        foreach (GameObject enemy in enemies)
        {
            bool isNotTargetable = enemy.GetComponent<EnemyNotTargetable>() != null;
            if (!isNotTargetable)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            player.gameObject.GetComponent<Player>().RecallFriend();
        }
    }
}
