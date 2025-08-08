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
            // TODO, attention mauvaise pratique ici,
            // ajoute lheritage aussi pour la turret
            // avec un Ennemyfixed au départ
            if (other.CompareTag("Enemy") && null != other.GetComponent<Enemy>())
            {
                var enemy = other.GetComponent<Enemy>();
                if (enemy != null)
                    enemy.TakeDamage(25f);
                Destroy(gameObject);
            }
            else if (other.CompareTag("Enemy") && null != other.GetComponent<Turret>())
            {
                var turret = other.GetComponent<Turret>();
                if (turret != null)
                    turret.TakeDamage(25f);
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
