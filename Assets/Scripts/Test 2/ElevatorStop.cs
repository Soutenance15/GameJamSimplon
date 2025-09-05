using UnityEngine;

public class ElevatorStop : MonoBehaviour
{
    [Header("Elevator")]
    public Transform elevator;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            ElevatorMoveStop(other);
        }
    }

    private void ElevatorMoveStop(Collider2D elevator)
    {
        Elevator elevatorObject = elevator.GetComponent<Elevator>();
        elevatorObject.StopMoving();
    }
}
