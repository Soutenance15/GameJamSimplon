using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float moveSpeed = 2f;
    public bool isMovingUp = false;

    void Update()
    {
        if (isMovingUp)
        {
            MoveUp();
        }
    }

    public void StopMoving()
    {
        isMovingUp = false;
    }
    public void MoveUp()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    }
}
