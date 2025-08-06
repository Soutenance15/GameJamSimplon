using UnityEngine;

public class PlateformControlTurning : MonoBehaviour
{
    public bool isCollidedPlayer = false;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Collided Enter Player");
            isCollidedPlayer = true;
        }
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.LogWarning("Quitte Collided Player");
            isCollidedPlayer = false;
        }
    }
}
