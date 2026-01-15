using UnityEngine;
using System; // Pour l'événement Action

public class BossHealth : MonoBehaviour
{
    [Header("Santé du Boss")]
    public float maxHealth = 500f;
    [SerializeField] private float currentHealth; // [SerializeField] permet de voir dans l'Inspector sans être public

    [Header("Bouclier")]
    public bool isShieldActive = true; // Le bouclier est actif par défaut au début du combat
    public GameObject shieldVisual; // Assigne ici le GameObject de l'effet visuel du bouclier (optionnel)

    // Événement déclenché quand le bouclier est désactivé
    public event Action OnShieldDeactivated;
    // Événement déclenché quand le boss prend des dégâts
    public event Action<float> OnBossDamaged;
    // Événement déclenché quand le boss meurt
    public event Action OnBossDie;


    void Start()
    {
        currentHealth = maxHealth;
        // Met à jour la visibilité du bouclier au démarrage
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(isShieldActive);
        }
    }

    // Inflige des dégâts au boss.
    public void TakeDamage(float amount)
    {
        if (isShieldActive)
        {
            Debug.Log("Le bouclier est actif ! Les dégâts n'ont pas été appliqués.");
            // Tu peux jouer un son ou un effet visuel de bouclier ici si tu veux
            return; // Le boss ne prend pas de dégâts si le bouclier est actif
        }

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0); // S'assurer que la vie ne descend pas en dessous de zéro

        Debug.Log("Boss a pris " + amount + " dégâts. Vie restante : " + currentHealth);

        OnBossDamaged?.Invoke(currentHealth); // Informe les abonnés que le boss a été blessé

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Active le bouclier du boss.
    public void ActivateShield()
    {
        isShieldActive = true;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
        Debug.Log("Bouclier du boss activé !");
    }

    // Désactive le bouclier du boss.
    public void DeactivateShield()
    {
        if (!isShieldActive) return; // Si déjà inactif, ne rien faire

        isShieldActive = false;
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
        Debug.Log("Bouclier du boss désactivé ! Boss est vulnérable !");
        OnShieldDeactivated?.Invoke(); // Déclenche l'événement
    }

    // Gère la mort du boss.
    void Die()
    {
        Debug.Log("Le boss est mort !");
        // Ici, tu peux ajouter des animations de mort, des effets sonores,
        // le drop de loots, charger la scène suivante, etc.
        OnBossDie?.Invoke(); // Informe les abonnés que le boss est mort
        Destroy(gameObject); // Détruit le GameObject du boss
    }

    // Fonction pour visualiser le rayon de détection du bouclier (utile pour le debug)
    void OnDrawGizmosSelected()
    {
        // Tu peux ajouter ici une visualisation pour le bouclier si tu en as besoin,
        // par exemple, une sphère autour du boss quand le bouclier est actif.
    }
}