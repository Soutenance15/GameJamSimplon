using UnityEngine;

public class FriendFollower : MonoBehaviour
{

    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 5f; // tirs par seconde
    private float lastShotTime = 0f;

    [Header("Suivi du joueur")]
    public Transform player;
    public float speed = 4f;
    public Vector2 followOffset = new Vector2(1.5f, 2f); // Offset par rapport au joueur

    [Header("Évitement d'obstacles")]
    public float obstacleDetectDistance = 1.0f;
    public float obstacleAvoidHeight = 1f;

    [Header("Détection d'ennemis")]
    public string enemyTag = "Enemy"; // Tous les ennemis doivent avoir ce tag



    void Start()
    {
        // Si le Friend a un Rigidbody2D, désactiver la gravité
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0;
        }
    }

    void Update()
    {
        if (player == null) return;

        // -------- 1. Calcul de la position cible à côté du joueur --------
        float cote = Mathf.Sign(transform.position.x - player.position.x);
        if (cote == 0) cote = Mathf.Sign(followOffset.x);

        Vector2 offset = new Vector2(Mathf.Abs(followOffset.x) * cote, followOffset.y);
        Vector3 targetPos = player.position + (Vector3)offset;

        // -------- 2. Évitement obstacle avec raycast --------
        Vector2 direction = (targetPos - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, obstacleDetectDistance, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            targetPos.y = Mathf.Max(targetPos.y, hit.point.y + obstacleAvoidHeight);
        }

        // -------- 3. Déplacement vers la position cible --------
        float moveDelta = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveDelta);

        // -------- 4. Visée automatique : rotation vers l'ennemi le plus proche --------
        Transform cible = FindClosestEnemy();
        if (cible != null)
        {
            Vector2 dir = (cible.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Revenir à une rotation neutre si aucun ennemi détecté
            transform.rotation = Quaternion.identity;
        }

        bool shootButton = Input.GetKey(KeyCode.LeftControl); 

        if (shootButton && Time.time > lastShotTime + 1f / fireRate)
        {
            Transform target = FindClosestEnemy(); // Fonction déjà existante
            if (target != null && projectilePrefab != null && firePoint != null)
            {
                Vector2 directionProjectile = (target.position - firePoint.position).normalized;
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                proj.GetComponent<Projectile>().Init(directionProjectile);
                lastShotTime = Time.time;
            }
        }

    }

    // Trouver l'ennemi le plus proche portant le tag "Enemy"
    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = enemy.transform;
            }
        }

        return closest;
    }
}
