using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public float lifeTime = 3f;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyOnGround enemy = other.GetComponent<EnemyOnGround>();
            NotMovableEnemy enemy2 = other.GetComponent<NotMovableEnemy>();
            // TODO
            // CODE MAL STRUCTURER
            // trop d'improvisation
            //c'est uniquement pour la version alpha
            //utilise l'heritage ici après
            NotTargetableEnemy enemy3 = other.GetComponent<NotTargetableEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(25f); // Valeur des dégâts, à ajuster
            }
            
            if (enemy2 != null)
            {
                enemy2.Explode();
                // enemy2.TakeDamage(25f); // Valeur des dégâts, à ajuster
            }
            
            if (enemy3 != null)
            {
                // meme si on peut pas target
                // si un projectile viens toucher de facon hasardeuse
                // on calcule les damage
                enemy3.TakeDamage(25f); // Valeur des dégâts, à ajuster
            }


            Destroy(gameObject);
        }
    }

}
