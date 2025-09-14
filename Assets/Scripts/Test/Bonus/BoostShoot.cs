using UnityEngine;

public class BoostShoot : MonoBehaviour
{
    public float multiplier = 2f; // x2 la cadence
    public float duration = 8f; // 8 secondes par défaut

    private void OnTriggerEnter2D(Collider2D other)
    {
        // On ne cible que le joueur OU le Friend selon ton architecture
        Player player = other.GetComponent<Player>();
        if (null != player && player.friendInstance != null)
        {
            Friend friend = player.friendInstance.GetComponent<Friend>();
            if (friend != null)
            {
                friend.ActivateBoostShoot(multiplier, duration);
                Destroy(gameObject); // disparaît une fois ramassé
            }
        }
    }
}
