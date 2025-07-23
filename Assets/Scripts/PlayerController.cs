using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    // Temps entre deux dégâts successifs lors d'un contact continu avec un ennemi
    public float damageInterval = 0.5f;
    private float lastDamageTime = -10f; // temps du dernier dégat infligé

    public float moveSpeed = 5f;       // Vitesse déplacement
    public float jumpForce = 7f;       // Puissance saut

    private Rigidbody2D rb;            // Rigidbody2D joueur
    private bool isAbove = false;      // Au sol ?

    [Header("Vie")]
    public float maxHealth = 100f;     // Vie max
    private float currentHealth;       // Vie actuelle
    public Transform healthBar;        // Barre de vie (UI ou sprite enfant)

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    void FixedUpdate()
    {
        isAbove = IsGrounded();
    }

    void Update()
    {
        // Déplacement gauche/droite (A/D ou flèches)
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Saut (flèche haut/Espace/W), seulement si au sol
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isAbove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    bool IsGrounded()
    {
        // Détection au sol basique (à améliorer avec un OverlapCircle)
        return Mathf.Abs(rb.linearVelocity.y) < 0.05f;
    }

    void UpdateHealthBar()
    {
        // Met à jour la barre de vie (scale X)
        if (healthBar != null)
        {
            float scale = Mathf.Clamp01(currentHealth / maxHealth);
            Vector3 localScale = healthBar.localScale;
            healthBar.localScale = new Vector3(scale, localScale.y, localScale.z);
        }
    }
    public void TakeDamage(float amount)
    {
        // Retire de la vie et vérifie la mort
        currentHealth -= amount;
        UpdateHealthBar();
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Le joueur est mort");
        // Animation mort (optionnelle)
        // Pour relancer la scène actuelle :
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Contact principal
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 playerCenter = transform.position;
            bool isStomping = (contactPoint.y < playerCenter.y - 0.1f) && rb.linearVelocity.y < 0f;

            if (!isStomping)
            {
                // Dégât régulier si le contact est maintenu
                if (Time.time > lastDamageTime + damageInterval)
                {
                    TakeDamage(20f); // Ajuste la valeur si besoin
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Contact principal
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 playerCenter = transform.position;
            // Si contact par dessus et joueur tombe ⇒ tue l’ennemi et rebondit
            bool isStomping = (contactPoint.y < playerCenter.y - 0.1f) && rb.linearVelocity.y < 0f;

            if (isStomping)
            {
                EnemyOnGround enemy = collision.gameObject.GetComponent<EnemyOnGround>();
                if (enemy != null & enemy.type == "weakHead") enemy.TakeDamage(enemy.maxHealth);
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 0.8f); // petit rebond
            }
            else
            {
                TakeDamage(20f); // Collision latérale : perd de la vie
            }
        }
    }


}
