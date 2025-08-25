using UnityEngine;

// Ennemi explosif tombe sur le sol et reste fixe
public class EnemyFixed : EnemyExplosive
{
    public override void OnCollisionEnter2D(Collision2D collision)
    // Pas de override ici, on réecris entierement car le parent
    //donne un comportement différent
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Friend"))
        {
            Explode();
        }
    }
}
