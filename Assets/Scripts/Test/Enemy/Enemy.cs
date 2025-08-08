using UnityEngine;

// Classe de base pour tous les ennemis au sol. Gère la vie, le déplacement, le flip, la barre de vie.
// Les classes filles spécialisent les comportements.
// public abstract class Enemy : MonoBehaviour
public abstract class Enemy : MonoBehaviour
{
    [Header("Mouvement")]
    public float moveSpeed = 2f;
    public Transform groundCheck; // Point de vérification du sol
    public float groundCheckRadius = 0.2f; // Rayon de détection du sol
    public LayerMask groundLayer; // Masque pour détecter le sol

    //TODO
    // Il est possible d'integrer un flipWaitTime pou eviter les repetitions de Flip
    // Por le moment ce n'est pas le cas
    // SOit integrer la fonctionalite, soit supprimer ce commentaire
    // Pour le moment l'inclinaison du sol choisit évite les repetitins de flip
    // TODO
    protected Rigidbody2D rb;
    private bool isCollidedPlayer = false;
    public int moveDirection = 1;

    [Header("Vie")]
    public float maxHealth = 100f;
    public float damageOnHead = 50f;
    public float currentHealth;

    [Header("Effets divers")]
    public float giveJumpForce = 6f; // Force jump (vers le haut) transmise au joueur par l'ennemi
    public float giveKnockBackForce = 6f; // Force knockBack vers un côté) transmise au joueur par l'ennemi
    public float giveDamage = 20f;

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

    // Retourne horizontalement le sprite de l'ennemi.
    public virtual void Flip()
    {
        moveDirection *= -1;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
        // Réinverser la healthbar pour qu'elle reste toujours orientée dans le même sens
        if (healthBarCanvas != null)
        {
            Vector3 hbScale = healthBarCanvas.transform.localScale;
            hbScale.x *= -1; // Inverse localScale.x pour annuler le flip venant du parent
            healthBarCanvas.transform.localScale = hbScale;
        }
    }

    // Initialise la vie et récupère le rigidbody.
    public virtual void Start()
    {
        // canvasHealthBar = transform.Find("CanvasHealthBar/HealthBar");
        // healthBar = transform.Find("Canvas/HealthBar");
        groundLayer = LayerMask.GetMask("GroundLayer");
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (
            collision.gameObject.CompareTag("Enemy")
            || collision.gameObject.CompareTag("Front Collision")
            || collision.gameObject.CompareTag("FixedBlockObject")
        )
        {
            Flip();
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidedPlayer = true;
        }
    }

    public virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isCollidedPlayer = false;
        }
    }

    public virtual void FixedUpdate()
    {
        // Déplacement horizontal automatique de l'ennemi
        rb.linearVelocity = new Vector2(moveDirection * moveSpeed, rb.linearVelocity.y);

        // Détection du sol sous le point groundCheck
        // Sans RayCast, le groundCheck doit être particulièrement grand afin de gérer les pentes aussi.
        bool onGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Retourne l'ennemi seulement s'il n'y a plus de sol sous le pied avant
        if (!onGround && !isCollidedPlayer)
        {
            Flip();
        }
    }
}
