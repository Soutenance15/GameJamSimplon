using UnityEngine;

public class PushZone : MonoBehaviour
{
    public float pushForce = 10f;

    // Appelée quand un autre collider entre dans le trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<Player>();
            if (player != null)
                player.PushMeInDirection(transform.up, pushForce, 0.22f); // 0.22s bloqué par exemple
        }
    }
}
