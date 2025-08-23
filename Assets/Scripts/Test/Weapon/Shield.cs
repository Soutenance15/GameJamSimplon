using UnityEngine;

// Le shield ne gère pas la vie, seulement blocage et tir éventuel
public class Shield : MonoBehaviour
{
    [Header("Tir (optionnel)")]
    public GameObject projectilePrefab;
    public Transform[] firePoints; // Ajoute 2 firepoints dans ton prefab Shield
    public float fireRate = 2f; // Tir toutes les X secondes
    private float fireCooldown = 0f;
    private Transform target; 

    void Start()
    {
        // Recherche automatique de la cible
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
            target = foundPlayer.transform;

        fireCooldown = 0; // Tir immédiat au début possible
    }

    void Update()
    {
        GérerTir();
    }

    // Bloquer les projectiles entrants
    void OnTriggerEnter2D(Collider2D other)
    {
        // Blocage: si c'est un projectile du joueur ou de l'ami
        if (other.CompareTag("ProjectileFriend"))
        {
            Destroy(other.gameObject); // Le bouclier absorbe le projectile
        }
    }

    void GérerTir()
    {
        // Optionnel : test de tir automatique toutes les X secondes
        if (projectilePrefab == null || firePoints.Length == 0 || target == null)
            return;
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            ShootFromAllFirePoins();
            fireCooldown = fireRate;
        }
    }

    void ShootFromAllFirePoins()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform fp = firePoints[i];
            if (fp == null || target == null)
                continue;

            // Décalage vertical (ex : +1 pour au-dessus, -1 pour en dessous)
            float verticalOffset = 0f;
            if (i == 0)
                verticalOffset = 2f; // Premier firepoint tire un peu au-dessus
            else if (i == 1)
                verticalOffset = -2f; // Deuxième firepoint tire un peu en dessous

            Vector3 offsetTargetPos = target.position + new Vector3(0, verticalOffset, 0);
            Vector2 direction = (offsetTargetPos - fp.position).normalized;

            GameObject proj = Instantiate(projectilePrefab, fp.position, Quaternion.identity);

            var projScript = proj.GetComponent<ProjectileBasic>();
            if (projScript != null)
            {
                projScript.Init(direction, ProjectileSource.Enemy, Color.cyan);
            }
            else
            {
                Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = direction * ProjectileBasic.speed;
            }

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
