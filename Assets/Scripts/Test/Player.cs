using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Effet de dégât")]
    public SpriteRenderer damageScreen = null;
    public float damageScreenDuration = 0.05f;
    private Coroutine damageScreenRoutine;

    [Header("Déplacements")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Vie")]
    public float maxHealth = 100f;
    private float currentHealth;
    public Transform healthBar;

    [Header("Dégâts")]
    public float damageInterval = 0.5f;
    private float lastDamageTime = -10f;

    [Header("Détection Sol")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private bool justBounced = false;

    // Temps d'invulnerabilite (de grace) durant le bounce pour eviter les multiplications
    // En cas de collisions multiples (en générale c'est le cas)
    private float bounceGraceTime = 0.12f;
    private float bounceTimer = 0f;

    // Knockback
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.15f; // Durée du repoussement

    void Start()
    {
        groundLayer = LayerMask.GetMask("GroundLayer");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        currentHealth = maxHealth;
        UpdateHealthBar();
        if (damageScreen != null)
            damageScreen.enabled = false;
        if (groundCheck == null)
            Debug.LogWarning(
                "groundCheck n'est pas assigné ! Assigne un enfant vide sous le joueur."
            );
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void startKnockBack()
    {
        isKnockedBack = true;
        knockbackTimer = 0f;
    }

    private void finishKnockBack()
    {
        isKnockedBack = false;
    }

    void Update()
    {
        // Mort si chute trop basse
        if (transform.position.y < -3f)
            Die();

        // Gère le knockback (empêche tout contrôle du joueur pendant knockback)
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackDuration)
            {
                finishKnockBack();
            }
            return; // Empêche tout contrôle temps du knockback
        }

        // Déplacement horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Saut
        if (
            (
                Input.GetKey(KeyCode.UpArrow)
                || Input.GetKey(KeyCode.W)
                || Input.GetKey(KeyCode.Space)
            ) && isGrounded
        )
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Gestion de la grâce après rebond
        if (justBounced)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer > bounceGraceTime)
            {
                justBounced = false;
                bounceTimer = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");
    }

    private bool IsStomping(Vector2 contactPoint)
    {
        // Retourne vrai si le contact est sous le centre du joueur et que le joueur descend
        return (contactPoint.y < transform.position.y + 0.025f) && rb.linearVelocity.y < 0f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Front Collision"))
        {
            PushMe();
            return;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                //Si le contact est sur la tête de l'ennemi
                if (IsStomping(contact.point))
                {
                    var enemy = collision.gameObject.GetComponent<Enemy>();
                    // if (enemy != null && enemy.GetType() == typeof(EnemyWeakHead))
                    if (enemy != null)
                    {
                        enemy.TakeDamage(enemy.damageOnHead);
                        rb.linearVelocity = new Vector2(
                            rb.linearVelocity.x,
                            enemy.giveJumpForce * 0.8f
                        );
                        justBounced = true;
                        bounceTimer = 0f;
                        return;
                    }
                }
            }
            // Si aucun contact n'est sur la tête de l'ennemi,
            // Appliquer perte de vie + knockback (poussé par l'ennemi)
            TakeDamage(20f, collision.gameObject.GetComponent<Enemy>());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !justBounced)
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (!IsStomping(contact.point) && Time.time > lastDamageTime + damageInterval)
                {
                    TakeDamage(20f, collision.gameObject.GetComponent<Enemy>());
                    lastDamageTime = Time.time;
                }
            }
        }
    }

    void PushMe()
    {
        // Petite poussée vers la gauche ou la droite selon le facing actuel
        float direction = (transform.localScale.x > 0) ? -1f : 1f;
        float knockBackForce = 10f;
        rb.linearVelocity = new Vector2(direction * knockBackForce, rb.linearVelocity.y);
    }

    // Dégâts et knockback éventuel
    // public void TakeDamage(float amount, Vector2? pushSource = null)
    // {
    //     ShowDamageScreen();
    //     currentHealth -= amount;
    //     UpdateHealthBar();
    //     if (currentHealth <= 0f)
    //         Die();

    //     if (pushSource.HasValue)
    //     {
    //         Vector2 pushDirection = ((Vector2)transform.position - pushSource.Value).normalized;
    //         float knockBackForce = 6.5f; // Ajuste selon le ressenti
    //         rb.linearVelocity = new Vector2(pushDirection.x * knockBackForce, rb.linearVelocity.y);
    //         isKnockedBack = true;
    //         knockbackTimer = 0f;
    //     }
    // }

    public void TakeDamage(float amount, Enemy enemy = null)
    {
        ShowDamageScreen();
        currentHealth -= amount;
        UpdateHealthBar();
        if (currentHealth <= 0f)
            Die();

        if (enemy != null)
        {
            Debug.Log(enemy.giveKnockBackForce);
            // Calcul de la direction (Player ----- Enemy)
            Vector2 pushDirection = (
                (Vector2)transform.position - (Vector2)enemy.transform.position
            ).normalized;
            // float knockBackForce = enemy.giveKnockBackForce; // La force personnalisée par l'ennemi
            rb.linearVelocity = new Vector2(
                pushDirection.x * enemy.giveKnockBackForce,
                rb.linearVelocity.y
            );

            // Lance le knockBack
            startKnockBack();
        }
    }

    // Surcharge pour cas sans knockback
    public void TakeDamage(float amount)
    {
        TakeDamage(amount, null);
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
        if (damageScreen != null)
            damageScreen.color = new Color(0, 0, 0, 0.4f);
        // Restart();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
