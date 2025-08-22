using UnityEngine;

public class SpriteColorModifier : MonoBehaviour
{
    public SpriteRenderer spriteToChange;

    private Color originalColor;
    private Color saturatedColor;
    public float transitionSpeed = 1.0f; // Vitesse de transition (ajustable dans l'Inspector)

    void Start()
    {
        if (spriteToChange != null)
        {
            originalColor = spriteToChange.color;
            // On calcule la couleur saturée une fois pour toutes au démarrage
            float luminance = 0.299f * originalColor.r + 0.587f * originalColor.g + 0.114f * originalColor.b;
            saturatedColor = new Color(luminance, luminance, luminance, originalColor.a);
        }
    }

    void Update()
    {
        // Si la touche "I" est maintenue
        if (Input.GetKey(KeyCode.I))
        {
            float t = Time.deltaTime * transitionSpeed;
            spriteToChange.color = Color.Lerp(spriteToChange.color, saturatedColor, t);
            Debug.Log("saturer");
        }

        // Si la touche "U" est maintenue
        else if (Input.GetKey(KeyCode.U))
        {
            float t = Time.deltaTime * transitionSpeed;
            spriteToChange.color = Color.Lerp(spriteToChange.color, originalColor, t);
            Debug.Log("Desaturer");
        }
    }
}