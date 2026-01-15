using UnityEngine;
using System; // Pour les événements Action
using TMPro; // Pour TextMeshProUGUI si tu l'utilises pour afficher les stats

public class DroneUpgradeManager : MonoBehaviour
{
    public static DroneUpgradeManager instance;
    // Référence au script de tir de Rebirth pour appliquer les upgrades
    public RebirthShooting rebirthShooting;

    [Header("Paramètres d'Upgrade de Base")]
    public float baseFireRate = 5f; // Cadence de tir initiale
    public float baseDamagePerShot = 10f; // Dégâts initiaux par tir
    public float baseEnergyMax = 10f; // Énergie max initiale
    public float baseEnergyRechargeRate = 0.5f; // Taux de recharge initial

    [Header("Bonus par Niveau d'Upgrade")]
    public float fireRateBonusPerLevel = 0.5f;
    public float damageBonusPerLevel = 2f;
    public float energyMaxBonusPerLevel = 2f;
    public float energyRechargeRateBonusPerLevel = 0.1f;

    [Header("Niveaux d'Upgrade Actuels")]
    public int fireRateLevel = 0;
    public int damageLevel = 0;
    public int energyMaxLevel = 0;
    public int energyRechargeRateLevel = 0;

    [Header("Points d'Upgrade")]
    public int availableUpgradePoints = 0;

    [Header("Références UI")]
    public GameObject upgradePanelUI; // Le GameObject qui contient ton UI d'upgrade (avec les boutons)
    public TextMeshProUGUI upgradePointsText; // Text pour afficher les points disponibles

    // Événements pour notifier d'autres scripts des changements
    public event Action OnUpgradePointsChanged;
    public event Action OnDroneUpgraded; // Peut être utilisé pour des effets visuels/sonores

    void Start()
    {
        if (rebirthShooting == null)
        {
            Debug.LogError("RebirthShooting n'est pas assigné au DroneUpgradeManager !");
        }

        // Assure-toi que le panneau d'upgrade est caché au démarrage
        if (upgradePanelUI != null)
        {
            upgradePanelUI.SetActive(false);
        }

        // Applique les stats initiales
        ApplyUpgradesToRebirth();
        UpdateUI();
    }


    // Ajoute un point d'upgrade disponible.
    public void AddUpgradePoint(int amount = 1) // Appelé quand un PC est désactivé.
    {
        availableUpgradePoints += amount;
        Debug.Log($"Nouveau point d'upgrade disponible ! Total : {availableUpgradePoints}");
        OnUpgradePointsChanged?.Invoke(); // Déclenche l'événement
        UpdateUI();

        // Si des points sont disponibles, affiche le panneau d'upgrade
        if (availableUpgradePoints > 0 && upgradePanelUI != null)
        {
            upgradePanelUI.SetActive(true);
            // Optionnel : Mettre le jeu en pause ici
            Time.timeScale = 0f; // Met le jeu en pause
            Debug.Log("Jeu en pause pour choisir l'upgrade.");
        }
    }

    // Applique les bonus d'upgrade au script RebirthShooting.
    public void ApplyUpgradesToRebirth()
    {
        if (rebirthShooting == null) return;

        // rebirthShooting.fireRate = baseFireRate + (fireRateLevel * fireRateBonusPerLevel);
        // rebirthShooting.projectileDamage = baseDamagePerShot + (damageLevel * damageBonusPerLevel); // On va ajouter 'projectileDamage' à RebirthShooting
        // rebirthShooting.energyMax = baseEnergyMax + (energyMaxLevel * energyMaxBonusPerLevel);
        // rebirthShooting.rechargeRate = baseEnergyRechargeRate + (energyRechargeRateLevel * energyRechargeRateBonusPerLevel); //ici

        // Assure que l'énergie actuelle est mise à jour avec le nouveau max
        rebirthShooting.currentEnergy = Mathf.Min(rebirthShooting.currentEnergy, rebirthShooting.energyMax);

        // Debug.Log("Upgrades appliqués à Rebirth : Cadence=" + rebirthShooting.fireRate + ", Dégâts=" + rebirthShooting.projectileDamage + ", EnergieMax=" + rebirthShooting.energyMax + ", Recharge=" + rebirthShooting.rechargeRate); //ici
    }


    // Fonctions appelées par les boutons de l'UI pour choisir une upgrade.
    public void UpgradeFireRate()
    {
        if (availableUpgradePoints <= 0) return;
        fireRateLevel++;
        availableUpgradePoints--;
        Debug.Log("Cadence de tir améliorée ! Niveau : " + fireRateLevel);
        FinishUpgradeChoice();
    }

    public void UpgradeDamage()
    {
        if (availableUpgradePoints <= 0) return;
        damageLevel++;
        availableUpgradePoints--;
        Debug.Log("Dégâts améliorés ! Niveau : " + damageLevel);
        FinishUpgradeChoice();
    }

    public void UpgradeEnergyMax()
    {
        if (availableUpgradePoints <= 0) return;
        energyMaxLevel++;
        availableUpgradePoints--;
        Debug.Log("Énergie Max améliorée ! Niveau : " + energyMaxLevel);
        FinishUpgradeChoice();
    }

    public void UpgradeEnergyRechargeRate()
    {
        if (availableUpgradePoints <= 0) return;
        energyRechargeRateLevel++;
        availableUpgradePoints--;
        Debug.Log("Recharge d'énergie améliorée ! Niveau : " + energyRechargeRateLevel);
        FinishUpgradeChoice();
    }

    private void FinishUpgradeChoice()
    {
        ApplyUpgradesToRebirth(); // Applique les nouvelles stats
        OnDroneUpgraded?.Invoke(); // Déclenche l'événement
        UpdateUI(); // Met à jour l'affichage des points et potentiellement d'autres infos

        // Cache le panneau d'upgrade et reprend le jeu
        if (upgradePanelUI != null)
        {
            upgradePanelUI.SetActive(false);
            Time.timeScale = 1f; // Reprend le jeu
            Debug.Log("Choix d'upgrade effectué. Jeu repris.");
        }
    }

    private void UpdateUI()
    {
        if (upgradePointsText != null)
        {
            upgradePointsText.text = $"Points d'upgrade : {availableUpgradePoints}";
        }
        // Ici, tu pourrais aussi mettre à jour le texte sur les boutons pour montrer le niveau actuel de chaque upgrade
    }
}