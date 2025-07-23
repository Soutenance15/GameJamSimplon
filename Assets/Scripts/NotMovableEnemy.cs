using UnityEngine;

public class NotMovableEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private int moveDirection = 1;
    public string type = "normal";

    [Header("Barre de vie")]
    public float maxHealth = 100f;
    private float currentHealth;

    public GameObject explosionEffectPrefab;

    // private bool hasExploded = false;

    public void Explode()
    {
        // if (hasExploded) return;
        // hasExploded = true;
        float explosionRadius = 2.5f;          // Rayon d’effet
        float damageDealt = 200f;             // Dégâts infligés

        // Optionnel : montrer un cercle dans la scène
        Debug.DrawLine(transform.position, transform.position + Vector3.up * explosionRadius, Color.red, 1f);

        // 1. Détection des objets
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // 2. Joueur ?
            if (hit.CompareTag("Player"))
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null)
                    pc.TakeDamage(damageDealt);
            }

            // 3. Autres ennemis (si souhaité)
            if (hit.CompareTag("Enemy"))
            {
                // TODO
                // CODE MAL STRUCTURER
                // utilise l'heritage pour faire quelque chose de plus propre
                // Ceci UNIQUEMENT POUR LA VERSION ALPHA
                var enemy = hit.GetComponent<EnemyOnGround>();
                var enemy2 = hit.GetComponent<NotMovableEnemy>();
                var enemy3 = hit.GetComponent<NotTargetableEnemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damageDealt);
                }
                if (enemy2 != null)
                {
                    enemy2.TakeDamage(damageDealt);
                }
                if (enemy3 != null)
                {
                    enemy3.TakeDamage(damageDealt);
                }
            }
        }

        // 5. Ajouter effets visuels (facultatif)
        // Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
            // float effectScale = explosionRadius * 2f; // 2x car le radius est le demi-diamètre
            float effectScale = explosionRadius/3; // 2x car le radius est le demi-diamètre
            effect.transform.localScale = new Vector3(effectScale, effectScale, 1f);
            // Détruit l’effet après sa durée
            Destroy(effect, 1.5f); // ou la durée de ton effet
        }
        // 6. Détruire l'ennemi après explosion
        Destroy(gameObject);
    }



    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO
        // Animation, effets, score, etc.
        Destroy(gameObject);
    }

    public Transform healthBar; // À assigner dans l’inspecteur

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float scale = Mathf.Clamp01(currentHealth / maxHealth);
            Vector3 localScale = healthBar.localScale;
            // On ne modifie que la largeur (axe X), pas la hauteur (axe Y) ni la profondeur (axe Z) 
            healthBar.localScale = new Vector3(scale, localScale.y, localScale.z);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Flip();
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    // void FixedUpdate()
    // {
    //     rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

    //     bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    //     if (!onGround)
    //     {
    //         Flip();
    //     }
    // }

    void Flip()
    {
        moveDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }


}
