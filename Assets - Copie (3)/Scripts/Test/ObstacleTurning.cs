using UnityEngine;

public class ObstacleTurning : MonoBehaviour
{
    public Vector3 axis = Vector3.forward; // Axe de rotation (Z = Vector3.forward pour 2D, Y pour 3D)
    public float speedRotation = 30f; // Degrés par seconde

    void Update()
    {
        // transform.Rotate(axis, speedRotation * Time.deltaTime);
        transform.Rotate(axis, speedRotation * Time.deltaTime);
    }
}
