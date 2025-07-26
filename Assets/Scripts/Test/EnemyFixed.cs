using UnityEngine;

// Ennemi fixe explosif, hérite de Enemy
public class FixedEnemy : Enemy
{
    public GameObject explosionEffectPrefab;

    public override void Start()
    {
        base.Start();
        giveJumpForce = 0f;
        // type = "notMovable";
        moveSpeed = 0f; // Pour éviter tout déplacement
    }

    public override void Die()
    {
        Explode();
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.CompareTag("Player"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        float explosionRadius = 2.5f;
        float damageDealt = 80f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var pc = hit.GetComponent<PlayerController>();
                if (pc != null)
                    pc.TakeDamage(damageDealt);
            }
            else if (hit.CompareTag("Enemy") && hit.gameObject != gameObject)
            {
                var enemy = hit.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(damageDealt);
            }
        }

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
}
