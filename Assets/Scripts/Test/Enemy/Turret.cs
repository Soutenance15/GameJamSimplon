using UnityEngine;

public class Turret : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public Transform body;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float detectionRange = 7f;
    public float fireRate = 1f;
    public Transform player;
    private float fireCooldown;

    [Header("UI")]
    public Canvas healthBarCanvas; // ou Transform ou GameObject
    public Transform healthBar; // Barre de vie (scale X modifiée selon la vie restante)

    // Prend des dégâts et vérifie si l'ennemi doit être détruit.
    public virtual void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Met à jour l'affichage de la barre de vie.
    public virtual void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float scale = Mathf.Clamp01(currentHealth / maxHealth);
            Vector3 localScale = healthBar.localScale;
            healthBar.localScale = new Vector3(scale, localScale.y, localScale.z);
        }
    }

    // Détruit l'ennemi. À personnaliser dans les sous-classes (ex: effets, animation).
    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        currentHealth = maxHealth;
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
