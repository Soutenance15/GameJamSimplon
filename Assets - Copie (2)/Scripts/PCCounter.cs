using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PCCounter : MonoBehaviour
{
    public TextMeshProUGUI  counterText;         // Assigne le Text UI ici dans l’inspecteur
    public int totalPC = 4;          // À ajuster selon ton niveau
    private int deactivatedPC = 0;   // Compte les PC désactivés

    void Start()
    {
        totalPC = Object.FindObjectsByType<PCEnemyInteraction>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
        UpdateHUD();
    }

    // Appelle cette méthode à chaque PC désactivé
    public void PCDeactivated()
    {
        deactivatedPC++;
        UpdateHUD();
    }

    void UpdateHUD()
    {
        counterText.text = "PC désactivés : " + deactivatedPC + " / " + totalPC;
    }
}
