using UnityEngine;

// Ennemi explosif tombe sur le sol et reste fixe
public class EnemyFixed : EnemyExplosive
{
#pragma warning disable CS0114 // Member hides inherited member; missing override keyword
    public void OnCollisionEnter2D(Collision2D collision)
#pragma warning restore CS0114 // Member hides inherited member; missing override keyword
    // Pas de override ici, on réecris entierement car le parent
    //donne un comportement différent
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Friend")
        )
        {
            Explode();
        }
    }
}
