using UnityEngine;

public class RebirthShooting : MonoBehaviour
{
    public RebirthAutoTarget autoTarget;
    public GameObject projectilePrefab;
    public Transform firePoint;

    public float energyMax = 10f;
    public float currentEnergy;
    public float rechargeRate = 0.5f;
    public float energyCostPerShot = 2f;
    public float fireCooldown = 0.3f;

    private bool isOverheated = false;
    private float cooldownTimer;

    void Start()
    {
        currentEnergy = energyMax;
    }

    void Update()
    {
        HandleRecharge();
        HandleShooting();
    }

    void HandleRecharge()
    {
        if (currentEnergy < energyMax)
        {
            currentEnergy += rechargeRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0f, energyMax);

            // Débloque le tir après surchauffe
            if (isOverheated && currentEnergy >= energyMax)
                isOverheated = false;
        }
    }

    void HandleShooting()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetButtonDown("Fire1") && !isOverheated && cooldownTimer <= 0f && autoTarget.HasTarget())
        {
            if (currentEnergy >= energyCostPerShot)
            {
                Shoot();
                currentEnergy -= energyCostPerShot;

                if (currentEnergy <= 0f)
                {
                    isOverheated = true;
                }

                cooldownTimer = fireCooldown;
            }
        }
    }

    void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // Ajouter ici le son / effet visuel
    }
}
