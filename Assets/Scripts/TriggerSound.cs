using UnityEngine;
using UnityEditor;

public class TriggerSound : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
           // AudioManager.Instance.PlayNextSong();
            Destroy(this.gameObject);
        }
    }

}
