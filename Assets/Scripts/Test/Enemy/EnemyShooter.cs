using UnityEngine;

public class EnemyShooter : Enemy
{
    [Header("Tir")]
    public GameObject projectilePrefab; // Le prefab du projectile à instancier
    public Transform firePoint; // Point depuis lequel tirer (mettre un enfant vide ici)
    public float fireRate = 2f; // Tir toutes les X secondes
    public float projectileSpeed = 8f; // Vitesse du projectile

    private float fireCooldown = 0f;
    private Transform player;

    public override void Start()
    {
        base.Start();
        // Recherche automatique du joueur via le tag "Player"
        GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
        if (foundPlayer != null)
            player = foundPlayer.transform;

        if (firePoint == null)
            Debug.LogWarning("firePoint non assigné sur EnemyShooter !");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        // On NE fait pas base.FixedUpdate() : on gère le déplacement de tir à la main ici si besoin

        // Gestion cooldown de tir
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;

        if (player != null && fireCooldown <= 0f)
        {
            ShootAtPlayer();
            fireCooldown = fireRate;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        // Direction vers le joueur à ce frame
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Instancie le projectile au point de tir
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Initialisation selon le camp d'origine
        var projScript = proj.GetComponent<ProjectileBasic>();
        if (projScript != null)
        {
            // Passe la direction ET la source (ici Enemy)
            projScript.Init(direction, ProjectileSource.Enemy);
        }
        else
        {
            // Fallback si ton script ne prend que la direction
            Rigidbody2D rbProj = proj.GetComponent<Rigidbody2D>();
            if (rbProj != null)
                rbProj.linearVelocity = direction * projectileSpeed;
        }

        // Optionnel : oriente le sprite du projectile vers la cible
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
