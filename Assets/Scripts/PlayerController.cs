using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;



// TODO
// ATTENTION mauvais code
// Utiliser heritage et polymorhisme apres pour un code structuré 
// et pas de redondance de dégueulasse comme maintenant
// Heritage pour les ennemis
// TODO
public class PlayerController : MonoBehaviour
{
    [Header("Effet de dégât")]
    public SpriteRenderer damageScreen;
    public float damageScreenDuration = 0.05f;

    private Coroutine damageScreenRoutine;

    [Header("Déplacements")]
    public float moveSpeed = 5f;      // Vitesse déplacement horizontal
    public float jumpForce = 7f;      // Puissance du saut

    [Header("Vie")]
    public float maxHealth = 100f;    // Vie max
    private float currentHealth;      // Vie actuelle
    public Transform healthBar;       // Barre de vie (UI ou sprite enfant)

    [Header("Dégâts")]
    public float damageInterval = 0.5f;   // Temps entre deux dégâts successifs
    private float lastDamageTime = -10f;  // Temps du dernier dégât infligé

    [Header("Détection Sol")]
    public Transform groundCheck;     // Point sous le joueur pour vérifier le sol
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;     // Layer des plateformes/sols

    private Rigidbody2D rb;
    private bool isGrounded;

    private bool justBounced = false;
    private float bounceGraceTime = 0.12f; // Durée d'immunité après rebond, à ajuster
    private float bounceTimer = 0f;

    // private int moveDirection = 1;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        currentHealth = maxHealth;
        UpdateHealthBar();

        if (damageScreen != null)
        {
            damageScreen.enabled = false;
        }

        if (groundCheck == null)
        {
            Debug.LogWarning("groundCheck n'est pas assigné ! Assigne un enfant vide sous le joueur.");
        }
    }

    void FixedUpdate()
    {
        // Vérification du sol avec OverlapCircle
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void Update()
    {
        // Mort si chute trop basse
        if (transform.position.y < -3f) // Ajuster ce seuil selon ton niveau
        {
            Die();
        }

        // Gestion du déplacement horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Gestion du saut
        if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space)) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Gestion du délai d'immunité après le rebond
        if (justBounced)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer > bounceGraceTime)
            {
                justBounced = false;
                bounceTimer = 0f;
            }
        }
        if ((Input.GetKey(KeyCode.Escape)))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private bool IsStomping(Vector2 contactPoint)
    {
        // Retourne vrai si le contact est sous le centre du joueur et que le joueur descend
        return (contactPoint.y < transform.position.y - 0.1f) && rb.linearVelocity.y < 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Front Collision"))
        {
            PushMe();
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (IsStomping(contact.point))
                {
                    EnemyOnGround enemyOG = collision.gameObject.GetComponent<EnemyOnGround>();
                    NotTargetableEnemy enemyNT = collision.gameObject.GetComponent<NotTargetableEnemy>();
                    NotMovableEnemy enemyNM = collision.gameObject.GetComponent<NotMovableEnemy>();

                    manageEnemyOG(enemyOG);
                    manageNTEnemy(enemyNT);
                    manageNMEnemy(enemyNM);
                    return;
                    // if (enemy != null)
                    // {
                    //     if (enemy.type == "weakHead")
                    //     {
                    //         enemy.TakeDamage(enemy.maxHealth);
                    //     }
                    //     Debug.Log("enemyGiveJumpForce" + enemy.giveJumpForce);
                    //     rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.giveJumpForce * 0.8f);
                    //     justBounced = true;
                    //     bounceTimer = 0f;
                    //     return;
                    // }
                }
            }
            // Si aucun contact n'était un stomp, appliquer la perte de vie
            TakeDamage(20f);
        }
    }

    void manageEnemyOG(EnemyOnGround enemy)
    {
        if (enemy != null)
        {
            if (enemy.type == "weakHead")
            {
                enemy.TakeDamage(enemy.maxHealth);
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.giveJumpForce * 0.8f);
            justBounced = true;
            bounceTimer = 0f;
            return;
        }
    }
    void manageNMEnemy(NotMovableEnemy enemy)
    {
        if (enemy != null)
        {
            if (enemy.type == "weakHead")
            {
                enemy.TakeDamage(enemy.maxHealth);
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.giveJumpForce * 0.8f);
            justBounced = true;
            bounceTimer = 0f;
            return;
        }
    }
    void manageNTEnemy(NotTargetableEnemy enemy)
    {
        if (enemy != null)
        {
            if (enemy.type == "weakHead")
            {
                enemy.TakeDamage(enemy.maxHealth);
            }
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, enemy.giveJumpForce * 0.8f);
            justBounced = true;
            bounceTimer = 0f;
            return;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (justBounced)
                return; // Ignore toute collision nocive pendant la grâce post-rebond

            foreach (ContactPoint2D contact in collision.contacts)
            {
                bool stomping = IsStomping(contact.point);
                if (!stomping)
                {
                    if (Time.time > lastDamageTime + damageInterval)
                    {
                        TakeDamage(20f);
                        lastDamageTime = Time.time;
                    }
                }
            }
        }
    }

    void PushMe()
    {
        // moveDirection *= -1;
        // Vector3 localScale = transform.localScale;
        // // localScale.x *= -1;
        // localScale.x -= 5;
        // transform.localScale = localScale;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x - 10, rb.linearVelocity.y);
    }

    public void TakeDamage(float amount)
    {
        ShowDamageScreen();
        currentHealth -= amount;
        UpdateHealthBar();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float scale = Mathf.Clamp01(currentHealth / maxHealth);
            Vector3 localScale = healthBar.localScale;
            healthBar.localScale = new Vector3(scale, localScale.y, localScale.z);
        }
    }

    public void ShowDamageScreen()
    {
        if (damageScreen == null)
            return;

        // Affiche le sprite (par exemple, le rend visible)
        damageScreen.enabled = true;

        if (damageScreenRoutine != null)
            StopCoroutine(damageScreenRoutine);

        damageScreenRoutine = StartCoroutine(HideDamageAfterDelay());
    }

    private IEnumerator HideDamageAfterDelay()
    {
        yield return new WaitForSeconds(damageScreenDuration);
        damageScreen.enabled = false;
        damageScreenRoutine = null;
    }
    void Die()
    {
        //TODO
        // Pour plus de réalisme aprés, 
        // on pourrait désactiver le contrôle ici avant reload
        // a voir avec Lucas
        // Pour l'instant il ya un screen qui cache le jeu simplement
        // mais les controls resten Active

        // Recharger la scène actuelle pour respawn
        damageScreen.color = new Color(0, 0, 0, 0.4f);
        // Thread.Sleep(2000);

        Restart();

    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Pour visualiser la zone de groundCheck dans l'éditeur
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
