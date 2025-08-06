using UnityEngine;

public class ExplosiveEnemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private int moveDirection = 1;
    public string type = "normal";

    void Die()
    {
        // TODO
        // Animation, effets, score, etc.
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (!onGround)
        {

        }
    }

    void Explode()
    {
        float explosionRadius = 2f;          // Rayon d’effet
        float damageDealt = 30f;             // Dégâts infligés

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
                var enemy = hit.GetComponent<EnemyOnGround>();
                if (enemy != null)
                    enemy.TakeDamage(damageDealt);
            }

            // 4. Objets destructibles (si besoin, mettre un autre tag)
            // if (hit.CompareTag("Destructible"))
            // {
            //     var destructible = hit.GetComponent<DestructibleObject>();
            //     if (destructible != null)
            //         destructible.TakeDamage(damageDealt);
            // }
        }

        // 5. Ajouter effets visuels (facultatif)
        // Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // 6. Détruire l'ennemi après explosion
        Destroy(gameObject);
    }



}
