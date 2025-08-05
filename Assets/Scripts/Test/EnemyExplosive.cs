using UnityEngine;

// Ennemi explosif (tombant sur le sol et explose)
public class EnemyExplosive : Enemy
{
    public GameObject explosionEffectPrefab;
    public float explosionRadius;

    public override void Start()
    {
        base.Start();
        Debug.Log("start");
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

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (!collision.gameObject.CompareTag("EnemyPC"))
        {
            ExplodeDamage(collision);
        }
    }

    public void ExplodeDamage(Collision2D collision)
    {
        if (explosionRadius <= 0f)
        {
            explosionRadius = 2.5f;
        }

        Explode();
        float damageDealt = 80f;

        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
                player.TakeDamage(damageDealt);
            else if (collision.gameObject.CompareTag("Enemy") && collision.gameObject != gameObject)
            {
                var enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damageDealt);
            }
        }
    }
}
