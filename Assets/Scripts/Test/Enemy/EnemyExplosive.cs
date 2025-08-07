using UnityEngine;

// Ennemi explosif (tombant sur le sol et explose)
public class EnemyExplosive : Enemy
{
    public GameObject explosionEffectPrefab;
    public float explosionRadius;
    public float damageDealt = 50f;

    public override void Start()
    {
        base.Start();
        moveSpeed = 0f; // Pour éviter tout déplacement
        giveJumpForce = 0f; // Pas d'impulsion donné
        giveKnockBackForce = 10f; // Poussé trés forte
    }

    public override void Die()
    {
        Explode();
        base.Die();
    }

    public void Explode()
    {
        // Génère l'effet visuel
        if (explosionEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                explosionEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            float effectScale = explosionRadius / 3f;
            effect.transform.localScale = new Vector3(effectScale, effectScale, 1f);
            Destroy(effect, 1.5f);
        }
        // Récupère tout ce qui est dans le rayon d'explosion
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.gameObject == gameObject)
                continue; // Ignore soi-même, étonnant mais bon
            // TODO S'assurer que c'est utile

            if (hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<Player>();
                if (player != null)
                    player.TakeDamage(damageDealt);
            }
            else if (hit.CompareTag("Enemy") && hit.gameObject != gameObject)
            {
                var enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damageDealt);
            }
            // Ajoute ici d'autres comportements si tu veux que d'autres objets "prennent" l'explosion
        }

        Destroy(gameObject);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (
            !collision.gameObject.CompareTag("EnemyPC") || !collision.gameObject.CompareTag("Enemy")
        )
        {
            Explode();
        }
    }
}
