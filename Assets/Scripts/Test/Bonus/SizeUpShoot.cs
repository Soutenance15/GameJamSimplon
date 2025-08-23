using UnityEngine;

public class SizeUpShoot : MonoBehaviour
{
    public float multiplier = 1.5f;
    public float duration = 8f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && player.friendInstance != null)
        {
            Friend friend = player.friendInstance.GetComponent<Friend>();
            if (friend != null)
            {
                friend.ActivateProjectileScaleBoost(multiplier, duration);
                Destroy(gameObject);
            }
        }
    }
}
