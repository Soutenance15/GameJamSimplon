using UnityEngine;

public enum ProjectileSource
{
    Enemy,
    Friend,
}

public class ProjectileBasic : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;
    private Vector2 direction;
    public ProjectileSource source; // Ajout

    public void Init(Vector2 dir, ProjectileSource src = ProjectileSource.Friend)
    {
        direction = dir.normalized;
        source = src;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // GESTION CIBLE selon source
        if (source == ProjectileSource.Friend)
        {
            if (other.CompareTag("Enemy"))
            {
                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(25f);
                Destroy(gameObject);
            }
        }
        else if (source == ProjectileSource.Enemy)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player>();
                if (player != null)
                    player.TakeDamage(25f, null); // Ou adapte selon ta signature
                Destroy(gameObject);
            }
            // (Optionnel) : empêcher l’auto-destruction si touche un autre ennemi.
            else if (other.CompareTag("Enemy"))
            {
                // Ignore : ne rien faire !
            }
        }
    }
}
