using System;
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
    private bool isPushed = false;
    private float pushDuration = 0.18f; // Durée de l'effet, à ajuster à ton ressenti
    private float pushTimer = 0f;

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

    // Temps d'invulnerabilité après bounce
    private float bounceGraceTime = 0.12f;
    private float bounceTimer = 0f;

    // Knockback
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    private float knockbackDuration = 0.15f; // Durée du repoussement

    [Header("Détection Sol")]
    public GameObject friendInstance;
    public Transform friendSpawnPoint;
    public bool isFriendDeployed = false;
    public bool controlDisable = false;
    private bool collidedObstacleJumpOn = false;
    public bool disableDeployFriend = false;

    void Start()
    {
        if (friendInstance != null)
        {
            // friendInstance.SetActive(false);
            isFriendDeployed = true;
        }

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

    private IEnumerator ScaleFriendOverTime(GameObject friend, float from, float to, float duration)
    {
        if (friend == null)
            yield break;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            float percent = Mathf.Clamp01(timer / duration);
            float scale = Mathf.Lerp(from, to, percent);
            friend.transform.localScale = new Vector3(scale, scale, 1f);
            yield return null;
        }
        friend.transform.localScale = new Vector3(to, to, 1f);
    }

    void DeployFriend()
    {
        if (friendInstance != null && !isFriendDeployed && !disableDeployFriend)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            float height = sr != null ? sr.bounds.size.y : 1f;
            Vector3 spawnPos =
                (friendSpawnPoint != null)
                    ? friendSpawnPoint.position
                    : transform.position + new Vector3(0, height, 0);

            friendInstance.transform.position = spawnPos;
            friendInstance.SetActive(true);
            StartCoroutine(ScaleFriendOverTime(friendInstance, 0.25f, 1f, 0.75f));
            var friendScript = friendInstance.GetComponent<MonoBehaviour>(); // Remplace par le vrai type si besoin

            if (friendScript != null)
            {
                var field = friendScript.GetType().GetField("player");
                if (field != null)
                    field.SetValue(friendScript, this.transform);
            }
            isFriendDeployed = true;
        }
    }

    public void RecallFriend()
    {
        if (friendInstance != null && isFriendDeployed)
        {
            friendInstance.SetActive(false);
            isFriendDeployed = false;
        }
    }

    public void PushMeInDirection(Vector2 direction, float force, float duration = 0.18f)
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        isKnockedBack = true;
        knockbackDuration = duration;
        knockbackTimer = 0f;
    }

    void Control()
    {
        if (isKnockedBack || controlDisable)
            return;

        // Déploiement/rangement du Friend
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (friendInstance != null)
            {
                if (!isFriendDeployed)
                    DeployFriend();
                else
                    RecallFriend();
            }
            else
            {
                Debug.LogWarning("friendInstance n'est pas assigné !");
            }
        }

        // Knockback : gestion dans Update

        // Saut
        if (
            (Input.GetKey(KeyCode.UpArrow) && isGrounded)
            || (Input.GetKey(KeyCode.UpArrow) && collidedObstacleJumpOn)
        )
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Gestion de la grâce après rebond
        if (justBounced)
        {
            bounceTimer += Time.deltaTime;
            if (bounceTimer >= bounceGraceTime)
            {
                justBounced = false;
                bounceTimer = 0f;
            }
        }

        if (Input.GetKey(KeyCode.Escape))
            SceneManager.LoadScene("MainMenu");

        // Déplacement horizontal
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    void Update()
    {
        // Mort si chute trop basse
        if (transform.position.y < -3f)
            Die();

        if (!controlDisable)
            Control();

        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackDuration)
                finishKnockBack();
            return; // Empêche tout contrôle de l'input tant que knockback
        }

        if (isPushed)
        {
            pushTimer += Time.deltaTime;
            if (pushTimer >= pushDuration)
                isPushed = false;
            else
                return; // Ignore inputs tant que push
        }
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
            PushMeBack();
            return;
        }

        if (collision.gameObject.CompareTag("Obstacle Jump On"))
            collidedObstacleJumpOn = true;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                // Si le contact est sur la tête de l'ennemi
                if (IsStomping(contact.point))
                {
                    var enemy = collision.gameObject.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(enemy.damageOnHead);
                        rb.linearVelocity = new Vector2(
                            rb.linearVelocity.x,
                            enemy.giveJumpForce * 0.8f
                        );
                        justBounced = true;
                        bounceTimer = 0f;
                    }
                    return;
                }
            }
            // Si aucun contact n'est sur la tête de l'ennemi,
            // Appliquer perte de vie + knockback (poussé par l'ennemi)
            TakeDamage(20f, collision.gameObject.GetComponent<Enemy>());
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle Jump On"))
            collidedObstacleJumpOn = true;

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

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle Jump On"))
            collidedObstacleJumpOn = false;
    }

    void PushMeBack()
    {
        // Petite poussée vers la gauche ou la droite selon le facing actuel
        float direction = (transform.localScale.x > 0) ? -1f : 1f;
        float knockBackForce = 10f;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * knockBackForce, 0f), ForceMode2D.Impulse);
        isKnockedBack = true;
        knockbackTimer = 0f;
    }

    public void TakeDamage(float amount, Enemy enemy = null)
    {
        ShowDamageScreen();
        currentHealth -= amount;
        UpdateHealthBar();
        if (currentHealth <= 0f)
            Die();
        if (enemy != null)
        {
            Debug.Log(enemy.giveKnockBackForce + "kn");
            // Calcul de la direction (Player ----- Enemy)
            Vector2 pushDirection = (
                (Vector2)transform.position - (Vector2)enemy.transform.position
            ).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(pushDirection * enemy.giveKnockBackForce, ForceMode2D.Impulse);
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

        EnemyPc lastPcDisable = null;
        GameObject[] allPCs = GameObject.FindGameObjectsWithTag("EnemyPC");
        foreach (GameObject pc in allPCs)
        {
            var enemyPc = pc.GetComponent<EnemyPc>();
            if (enemyPc.lastDisable)
                lastPcDisable = enemyPc;
        }
        if (null != lastPcDisable)
            SpawnAtCheckpoint(lastPcDisable);
        else
        {
            //TODO attendre 3 secondes puis restart
            Restart();
        }
    }

    void SpawnAtCheckpoint(EnemyPc enemyPc)
    {
        ResetPlayer();
        gameObject.SetActive(false);
        gameObject.transform.position = new Vector3(
            enemyPc.transform.position.x,
            enemyPc.transform.position.y,
            enemyPc.transform.position.z
        );
        gameObject.SetActive(true);
    }

    void ResetPlayer()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
        isFriendDeployed = false;
        controlDisable = false;
        collidedObstacleJumpOn = false;
        disableDeployFriend = false;
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
