using UnityEngine;

// Place ce script sur le GameObject de la grande Wheel (parent)
public class FerrisWheelAndPlatform : MonoBehaviour
{
    [Header("Roue")]
    public float speedRotation = 30f; // Vitesse de base (degrés/seconde)
    public Transform Wheel; // Assigné automatiquement si null

    [Header("Plateforme centrale")]
    public Transform plateforme; // Glisse ici l’enfant plateforme (PlatTurning) dans l’éditeur
    public bool compenserRotation = true; // Coche pour stationner la plateforme visuellement

    private int rotationState = 0; // 1 = sens normal, 0 = stop, -1 = inverse
    private int rotationStateSave = 0;

    // private bool playerPresent = false;
    public PlateformControlTurning plateform = null;
    private Quaternion platformInitRot;

    // Pour détecter le trigger de la plateforme
    private Collider2D plateformeTrigger;

    void Start()
    {
        if (rotationState != 0)
        {
            rotationStateSave = rotationState;
        }
        platformInitRot = plateforme.rotation;
    }

    void Update()
    {
        // Vérifie l’entrée seulement si le joueur est bien sur la plateforme
        if (plateform.isCollidedPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log(plateform.isCollidedPlayer);

            if (rotationState == 1 || rotationState == -1)
            {
                rotationStateSave = rotationState;
                rotationState = 0;
            }
            else if (rotationState == 0)
                if (0 != rotationStateSave)
                {
                    rotationState = rotationStateSave * -1;
                }
                else
                {
                    //Donne un premier sens de rotation
                    rotationState = 1;
                    rotationStateSave = rotationState;
                }
        }
    }

    void FixedUpdate()
    {
        if (rotationState != 0 && Wheel != null)
            Wheel.Rotate(
                Vector3.forward,
                speedRotation * Time.fixedDeltaTime * Mathf.Sign(rotationState)
            );
    }

    void LateUpdate()
    {
        if (compenserRotation)
        {
            plateform.transform.rotation = platformInitRot; // reste fixe dans le monde
        }
    }
}
