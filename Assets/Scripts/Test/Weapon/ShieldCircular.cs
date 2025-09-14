using UnityEngine;

// Le shield ne gère pas la vie, seulement blocage et tir éventuel
public class ShieldCircular : MonoBehaviour
{
    // Bloquer les projectiles entrants
    void OnTriggerEnter2D(Collider2D other)
    {
        // Blocage: si c'est un projectile du joueur ou de l'ami
        if (other.CompareTag("ProjectileFriend"))
        {
            Destroy(other.gameObject); // Le bouclier absorbe le projectile
        }
    }
}
