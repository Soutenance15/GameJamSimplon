using UnityEngine;

// Ennemi explosif : explose à la collision, inflige des dégâts en zone, puis se détruit.
public class EnemyExplosive : Enemy
{
    public GameObject explosionEffectPrefab;

    public override void Start()
    {
        base.Start();
        type = "explosive";
        giveJumpForce = 0f;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        Explode();
    }

    public void Explode()
    {
        float explosionRadius = 1f;
        float damageDealt = 25f;

        // Dégâts de zone autour de l’ennemi
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            // Dégâts au joueur
            if (hit.CompareTag("Player"))
            {
                PlayerController pc = hit.GetComponent<PlayerController>();
                if (pc != null)
                    pc.TakeDamage(damageDealt);
            }
            // Dégâts aux autres ennemis (sauf soi-même)
            else if (hit.CompareTag("Enemy") && hit.gameObject != gameObject)
            {
                Enemy enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damageDealt);
            }
        }

        // Effet visuel d’explosion
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

        Destroy(gameObject);
    }

    public override void Die()
    {
        Explode();
    }
}
