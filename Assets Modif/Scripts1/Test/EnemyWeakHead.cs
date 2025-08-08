using UnityEngine;

// Ennemi mobile, meur en un coup sur la tête
public class EnemyWeakHead : Enemy
{
    // Valeur spécifique pour ce type d’ennemi
    public override void Start()
    {
        base.Start();
        maxHealth = 50f;
        damageOnHead = maxHealth;
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
    }
}
