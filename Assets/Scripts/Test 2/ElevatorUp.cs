using UnityEngine;

public class ElevatorUp : MonoBehaviour
{
    [Header("Elevator")]
    public Transform elevator;

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("ezhfzeh");
        if (other.CompareTag("Player"))
        {
            ElevatorMoveUp();
        }
    }

    private void ElevatorMoveUp()
    {
        Elevator elevatorObject = elevator.GetComponent<Elevator>();
        if (! elevatorObject.isMovingUp)
            elevatorObject.isMovingUp = true;
        // Debug.Log("Elevator Move up");
    }
}
