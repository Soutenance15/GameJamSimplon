using System;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public float multiplier = 2f; // x2 la cadence
    public float duration = 8f; // 8 secondes par défaut

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && player.friendInstance != null)
        {
            Friend friend = player.friendInstance.GetComponent<Friend>();
            if (friend != null)
            {
                ActivateBonus(player, friend, multiplier, duration);
                Destroy(gameObject); // disparaît une fois ramassé
            }
        }
    }

    public virtual void ActivateBonus(
        Player player,
        Friend friend,
        float multiplier,
        float duration
    )
    {
        if (!player.isFriendDeployed)
        {
            player.DeployFriend();
        }
    }
}
