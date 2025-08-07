using UnityEngine;

public class EnemyInifiniteRespawn : Enemy
{
    private Vector3 spawnPosition; // Point de réapparition d’origine

    public override void Start()
    {
        base.Start();
        spawnPosition = transform.position;
    }

    public override void Die()
    {
        // Le perso ne meurt plus, il va respawn
        RespawnAtOrigin();
    }

    private void RespawnAtOrigin()
    {
        // Instancie un clone au point de départ, détruit l’ennemi actuel :
        currentHealth = maxHealth;
        UpdateHealthBar();
        Instantiate(gameObject, spawnPosition, Quaternion.identity);
        Destroy(gameObject);
    }
}
