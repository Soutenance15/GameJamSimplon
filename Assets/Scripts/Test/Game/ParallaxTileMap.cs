using UnityEngine;

public class ParallaxTilemap : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f; // 0 = pas de mouvement, 1 = suit exactement la caméra
    private Vector3 previousCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
        previousCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - previousCameraPosition;
        transform.position += delta * parallaxFactor;
        previousCameraPosition = cameraTransform.position;
    }
}
