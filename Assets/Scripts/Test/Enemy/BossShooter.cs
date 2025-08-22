using UnityEngine;

public class BossShooter : EnemyShooter
{
    [Header("Boss: Multi-canons")]
    public Transform[] firePoints; // Les 3 points de tir (à assigner dans l’inspecteur)

    [Header("Visuel à pivoter (optionnel)")]
    public Transform body; // Si tu veux faire pivoter un objet visuel (le corps du boss)

    private float currentAngle = 0f; // Pour le Lerp smooth de l’angle (effet smooth, comme Turret/Drone)

    public override void Start()
    {
        base.Start();
        // Optionnel : sécurité si firePoints pas assignés.
        if (firePoints == null || firePoints.Length == 0)
            Debug.LogWarning("Assignes au moins 1 firePoint dans BossShooter !");
        if (body == null)
            Debug.LogWarning("body du boss non assigné (optionnel, juste visuel) !");
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        // 1. ROTATION VISUELLE DU CANON/BODY VERS LE JOUEUR
        if (player != null && firePoints != null && firePoints.Length > 0)
        {
            Transform mainFirePoint = firePoints[0];
            Vector2 dir = (player.position - mainFirePoint.position).normalized;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, 0.2f);

            // Si tu veux pivoter le corps (body) visuel
            if (body != null)
                body.rotation = Quaternion.Euler(0f, 0f, currentAngle);
            // Sinon pivote juste le canon principal
            else
                mainFirePoint.rotation = Quaternion.Euler(0f, 0f, currentAngle);
        }

        // 2. TIR MULTIPLE (ça ne déplace rien)
        if (fireCooldown > 0f)
            fireCooldown -= Time.fixedDeltaTime;

        if (player != null && fireCooldown <= 0f)
        {
            ShootAtPlayerMultiple();
            fireCooldown = fireRate;
        }
    }

    // TIR VERS JOUEUR - depuis tous les firePoints en même temps
    void ShootAtPlayerMultiple()
    {
        if (projectilePrefab == null || firePoints == null || player == null)
            return;

        foreach (Transform fp in firePoints)
        {
            Vector2 direction = (player.position - fp.position).normalized;
            GameObject proj = Instantiate(projectilePrefab, fp.position, Quaternion.identity);

            var projScript = proj.GetComponent<ProjectileBasic>();
            if (projScript != null)
                projScript.Init(direction, ProjectileSource.Enemy); // Si tu utilises ProjectileSource

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            proj.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
