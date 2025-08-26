using UnityEngine;

public class EnemyJumping : Enemy
{
    [Header("Saut")]
    public float jumpForce = 7f; // Composante verticale du saut
    public float jumpHorizontalForce = 3f; // Composante horizontale du saut (augmente si pas d'avancée)
    public float jumpCooldown = 1.2f; // Temps minimum entre deux sauts
    private float jumpTimer = 0f;

    [Header("Allers-retours")]
    public int jumpsForward = 3; // Nombre de sauts dans un sens
    public int jumpsBackward = 3; // Nombre de sauts dans l'autre
    private int jumpsRemaining;
    private bool goingForward = true;

    [Header("Sécurité Respawn")]
    public float maxAirTime = 2.5f; // Temps max sans toucher le sol (avant respawn)
    private float airTimer = 0f;
    private Vector3 spawnPosition; // Point de réapparition d’origine

    public override void Start()
    {
        base.Start();
        jumpsRemaining = jumpsForward;
        goingForward = true;
        spawnPosition = transform.position;
    }

    public override void FixedUpdate()
    //hide parent function
    {
        // NE PAS appeler base.FixedUpdate() sinon velocity.X sera forcée !

        jumpTimer += Time.fixedDeltaTime;

        bool onGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // ----- Gestion du saut -----
        if (onGround && jumpTimer >= jumpCooldown && jumpsRemaining > 0)
        {
            int direction = goingForward ? 1 : -1;

            // On reset la vitesse pour ne pas accumuler d’impulsion
            rb.linearVelocity = Vector2.zero;

            // Impulsion diagonale pour avancer + sauter
            Vector2 impulse = new Vector2(direction * jumpHorizontalForce, jumpForce);
            rb.AddForce(impulse, ForceMode2D.Impulse);

            jumpsRemaining--;
            jumpTimer = 0f;
        }

        // Si la série de sauts est terminée, demi-tour et reset du compteur
        if (jumpsRemaining <= 0 && onGround)
        {
            Flip();
            goingForward = !goingForward;
            jumpsRemaining = goingForward ? jumpsForward : jumpsBackward;
        }

        // ----- Sécurité "perdu dans le vide" (air timer & respawn) -----
        if (!onGround)
        {
            airTimer += Time.fixedDeltaTime;
            if (airTimer >= maxAirTime)
            {
                RespawnAtOrigin();
            }
        }
        else
        {
            airTimer = 0f;
        }
    }

    private void RespawnAtOrigin()
    {
        // Instancie un clone au point de départ, détruit l’ennemi actuel :
        Instantiate(gameObject, spawnPosition, Quaternion.identity);
        Destroy(gameObject);
    }
}
