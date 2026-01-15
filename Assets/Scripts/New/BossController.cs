using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Pour charger la scène de jeu après le boss

public class BossController : MonoBehaviour
{
    [Header("Références Générales")]
    public BossHealth bossHealth; // Assigne le script BossHealth
    // public Animator bossAnimator; // Assigne l'Animator du boss
    public Transform playerTransform; // Assigne le Transform de ton joueur (Ash)
    public GameObject groundPlatform; // Assigne le GameObject de la plateforme de la phase 1

    [Header("Références Projectiles/Beam")]
    public Transform firePoint; // Le point de départ des tir
    public GameObject orbProjectilePrefab; // Le prefab de l'orbe normale
    public GameObject beamPrefab; // Le prefab du rayon visuel

    [Header("Paramètres du Rayon de Vulnérabilité")]
    public float beamDuration = 1.5f; // Durée pendant laquelle le rayon est actif
    public float vulnerabilityDuration = 5f; // Durée pendant laquelle le boss est vulnérable après le rayon
    public float delayBeforeBeamAttack = 1f; // Délai avant que le rayon ne se manifeste après l'animation

    [Header("Paramètres des Phases")]
    public float phase1HealthThreshold = 0.66f; // Ex: Passer en Phase 2 quand la vie est <= 66%
    public float phase2HealthThreshold = 0.33f; // Ex: Passer en Phase 3 quand la vie est <= 33%

    [Header("Paramètres de la Phase 1 (Sol)")]
    public float phase1MoveSpeed = 3f;
    public float phase1PatrolRange = 5f; // Distance de déplacement de chaque côté du point de départ
    public float phase1OrbFireRate = 1f; // Fréquence de tir des orbes (tirs/seconde)
    public float phase1TimeBetweenBeamAttacks = 10f; // Temps entre deux attaques de rayon en phase 1

    [Header("Paramètres des Phases 2 & 3 (Air)")]
    public float phase2_3MoveSpeed = 4f;
    public float phase2_3PatrolRange = 7f; // Plus large en l'air
    public float phase2_3OrbFireRate = 1.5f; // Plus rapide
    public float phase2_3TimeBetweenBeamAttacks = 8f; // Plus fréquent en l'air

    private Rigidbody2D rb;
    private Vector3 initialPosition; // Position de départ du boss
    private float currentMoveSpeed;
    private float currentPatrolRange;
    private float currentOrbFireRate;
    private float currentTimeBetweenBeamAttacks;

    private float fireTimer;
    private float beamTimer;

    // Définition des états/patterns du boss
    public enum BossPhase
    {
        Idle,
        Phase1_Ground,
        Phase2_Air,
        Phase3_Air,
        Dying
    }
    public BossPhase currentBossPhase = BossPhase.Idle;

    void Awake() // Utilisez Awake pour s'assurer que Rigidbody est récupéré tôt
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position; // Enregistre la position de départ
    }

    void Start()
    {
        // if (bossHealth == null || bossAnimator == null || playerTransform == null)
        if (bossHealth == null ||  playerTransform == null)
        {
            Debug.LogError("Veuillez assigner toutes les références nécessaires dans l'Inspector du BossController !");
            enabled = false; // Désactive le script si des références manquent
            return;
        }

        // S'abonne aux événements de BossHealth pour réagir à la mort
        bossHealth.OnBossDie += HandleBossDeath;

        StartBossFight();
    }

    void OnDestroy()
    {
        // Se désabonne des événements pour éviter les erreurs
        if (bossHealth != null)
        {
            bossHealth.OnBossDie -= HandleBossDeath;
        }
    }

    // Démarre le combat du boss.
    public void StartBossFight()
    {
        Debug.Log("Le combat du boss commence !");
        StartCoroutine(BossFightSequence());
    }

    // Gère la séquence globale du combat, y compris les transitions de phases.
    IEnumerator BossFightSequence()
    {
        currentBossPhase = BossPhase.Idle;
        // Optionnel : Intro du boss, apparition, etc.
        yield return new WaitForSeconds(2f);

        // --- Phase 1 ---
        Debug.Log("Activation Phase 1: Sol");
        currentBossPhase = BossPhase.Phase1_Ground;
        currentMoveSpeed = phase1MoveSpeed;
        currentPatrolRange = phase1PatrolRange;
        currentOrbFireRate = phase1OrbFireRate;
        currentTimeBetweenBeamAttacks = phase1TimeBetweenBeamAttacks;
        // Assure que le bouclier est actif en début de phase
        bossHealth.ActivateShield();
        StartCoroutine(HandleBossMovement()); // Démarre le mouvement général
        StartCoroutine(HandleOrbShooting()); // Démarre le tir d'orbes
        StartCoroutine(HandleBeamAttackCycle()); // Démarre le cycle d'attaque de rayon

        // Attend que la vie descende sous le seuil pour passer en Phase 2
        // yield return new WaitUntil(() => bossHealth.currentHealth <= bossHealth.maxHealth * phase1HealthThreshold); 

        // Nettoyage et Transition vers Phase 2
        Debug.Log("Transition vers Phase 2...");
        StopAllBossCoroutines(); // Arrête tous les comportements de la phase précédente
        
        // Fait disparaître la plateforme
        if (groundPlatform != null)
        {
            groundPlatform.SetActive(false);
            Debug.Log("Plateforme désactivée !");
        }
        
        // Le boss s'envole si nécessaire
        rb.gravityScale = 0f; // Désactive la gravité
        // Tu peux ajouter une animation de vol ici
        yield return new WaitForSeconds(1f); // Petite pause pour la transition

        // --- Phase 2 ---
        Debug.Log("Activation Phase 2: Air");
        currentBossPhase = BossPhase.Phase2_Air;
        currentMoveSpeed = phase2_3MoveSpeed;
        currentPatrolRange = phase2_3PatrolRange;
        currentOrbFireRate = phase2_3OrbFireRate;
        currentTimeBetweenBeamAttacks = phase2_3TimeBetweenBeamAttacks;
        bossHealth.ActivateShield(); // Réactive le bouclier pour cette phase
        StartCoroutine(HandleBossMovement());
        StartCoroutine(HandleOrbShooting());
        StartCoroutine(HandleBeamAttackCycle());

        // Attend que la vie descende sous le seuil pour passer en Phase 3
        // yield return new WaitUntil(() => bossHealth.currentHealth <= bossHealth.maxHealth * phase2HealthThreshold);

        // Nettoyage et Transition vers Phase 3
        Debug.Log("Transition vers Phase 3...");
        StopAllBossCoroutines(); // Arrête tous les comportements de la phase précédente
        yield return new WaitForSeconds(1f); // Petite pause pour la transition

        // --- Phase 3 ---
        Debug.Log("Activation Phase 3: Air (plus intense)");
        currentBossPhase = BossPhase.Phase3_Air;
        // Les paramètres sont les mêmes que la phase 2, mais tu pourrais les rendre plus agressifs ici
        // currentMoveSpeed *= 1.2f;
        // currentOrbFireRate *= 1.2f;
        // currentTimeBetweenBeamAttacks *= 0.8f;
        bossHealth.ActivateShield(); // Réactive le bouclier
        StartCoroutine(HandleBossMovement());
        StartCoroutine(HandleOrbShooting());
        StartCoroutine(HandleBeamAttackCycle());

        // Attend la mort du boss
        // yield return new WaitUntil(() => bossHealth.currentHealth <= 0); //ici

        // Le combat est terminé, la mort est gérée par HandleBossDeath
    }

    // Arrête toutes les coroutines liées aux patterns du boss.
    void StopAllBossCoroutines()
    {
        StopCoroutine(HandleBossMovement());
        StopCoroutine(HandleOrbShooting());
        StopCoroutine(HandleBeamAttackCycle());
        // On peut utiliser StopAllCoroutines() si ce script n'a pas d'autres coroutines critiques.
        // StopAllCoroutines(); // Peut être trop agressif si d'autres coroutines non-pattern sont en cours
    }
    // Gère le mouvement de patrouille du boss.
    IEnumerator HandleBossMovement()
    {
        float direction = 1f; // 1 pour droite, -1 pour gauche
        float currentXTarget = initialPosition.x + currentPatrolRange; // Cible de départ

        while (currentBossPhase != BossPhase.Dying)
        {
            Vector2 targetPosition = new Vector2(currentXTarget, transform.position.y);
            Vector2 currentPosition = transform.position;

            // Déplace le boss vers la cible
            rb.linearVelocity = new Vector2(direction * currentMoveSpeed, rb.linearVelocity.y);

            // Vérifie si le boss a atteint sa cible ou l'a dépassée
            if ((direction > 0 && currentPosition.x >= currentXTarget) ||
                (direction < 0 && currentPosition.x <= currentXTarget))
            {
                // Change de direction
                direction *= -1f;
                // Calcule la nouvelle cible
                currentXTarget = initialPosition.x + (direction * currentPatrolRange);
                
                // Optionnel : Animer le flip du sprite
                // transform.localScale = new Vector3(direction * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            
            // Pour le mode aérien, on peut aussi contrôler la hauteur
            // if (currentBossPhase == BossPhase.Phase2_Air || currentBossPhase == BossPhase.Phase3_Air)
            // {
            //    // Exemple : léger mouvement vertical sinusoïdal
            //    float airHeightOffset = Mathf.Sin(Time.time * 0.5f) * 0.5f;
            //    transform.position = new Vector3(transform.position.x, initialPosition.y + airHeightOffset, transform.position.z);
            // }

            yield return new WaitForFixedUpdate(); // Mouvement basé sur Rigidbody, donc FixedUpdate
        }
        rb.linearVelocity = Vector2.zero; // Arrête le mouvement à la mort
    }
    // Gère le tir d'orbes normal du boss.
    IEnumerator HandleOrbShooting()
    {
        fireTimer = 0f;
        while (currentBossPhase != BossPhase.Dying)
        {
            // Tir seulement si le bouclier est actif (pas pendant la vulnérabilité)
            if (bossHealth.isShieldActive)
            {
                fireTimer -= Time.deltaTime;
                if (fireTimer <= 0f)
                {
                    ShootOrb();
                    fireTimer = 1f / currentOrbFireRate;
                }
            }
            yield return null;
        }
    }
    // Gère le cycle de l'attaque de rayon et de la vulnérabilité.
    IEnumerator HandleBeamAttackCycle()
    {
        beamTimer = currentTimeBetweenBeamAttacks;
        while (currentBossPhase != BossPhase.Dying)
        {
            // Le boss ne tire son rayon que si le bouclier est actif
            if (bossHealth.isShieldActive)
            {
                beamTimer -= Time.deltaTime;
                if (beamTimer <= 0f)
                {
                    yield return ExecuteBeamAttack(); // Exécute l'attaque de rayon
                    beamTimer = currentTimeBetweenBeamAttacks; // Réinitialise le timer après l'attaque
                }
            }
            yield return null;
        }
    }
    // Exécute l'attaque de rayon, rend le boss vulnérable, puis réactive le bouclier.
    IEnumerator ExecuteBeamAttack()
    {
        Debug.Log("Boss: Attaque de Rayon déclenchée !");
        // Désactive le tir d'orbes pendant l'attaque de rayon
        fireTimer = 9999f; // Empêche les tirs d'orbes pendant l'animation/attaque

        // 1. Joue l'animation de tir du rayon
        // bossAnimator.SetTrigger("Shoot");

        // 2. Attends un court instant pour que l'animation de préparation se joue
        yield return new WaitForSeconds(delayBeforeBeamAttack);

        // 3. Active le rayon visuel et inflige des dégâts
        GameObject currentBeam = null;
        if (beamPrefab != null && firePoint != null)
        {
            currentBeam = Instantiate(beamPrefab, firePoint.position, firePoint.rotation);
            // Ici, tu devrais implémenter la logique pour que le rayon inflige des dégâts
            // Par exemple, si le rayon a un script "BeamDamageHandler"
            // currentBeam.GetComponent<BeamDamageHandler>().StartDamage(playerTransform);
        }

        Debug.Log("Rayon tiré !");

        // 4. Attends la durée du rayon
        yield return new WaitForSeconds(beamDuration);

        // 5. Désactive et détruit le rayon visuel
        if (currentBeam != null)
        {
            Destroy(currentBeam);
        }
        Debug.Log("Rayon terminé.");

        // 6. Rend le boss vulnérable en désactivant son bouclier
        bossHealth.DeactivateShield();

        // 7. Le boss reste vulnérable pendant 'vulnerabilityDuration'
        float vulnerabilityTimer = vulnerabilityDuration;
        while (vulnerabilityTimer > 0)
        {
            // Optionnel : Change l'animation ou l'apparence du boss pour montrer sa vulnérabilité
            // bossAnimator.SetBool("IsVulnerable", true);
            vulnerabilityTimer -= Time.deltaTime;
            yield return null;
        }

        Debug.Log("Fin de la vulnérabilité.");
        bossHealth.ActivateShield(); // Réactive le bouclier après la période de vulnérabilité
        // bossAnimator.SetBool("IsVulnerable", false); // Retour à l'animation normale
        
        fireTimer = 0f; // Réactive le tir d'orbes
    }
    // Tire une orbe (attaque de base).
    void ShootOrb()
    {
        if (orbProjectilePrefab != null && firePoint != null)
        {
            // Assure que le boss regarde le joueur avant de tirer
            Vector2 directionToPlayer = (playerTransform.position - firePoint.position).normalized;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
            Quaternion rotationToPlayer = Quaternion.Euler(0, 0, angle);

            // Joue l'animation de tir de l'orbe
            // bossAnimator.SetTrigger("Shoot"); 
            
            // Instancie le projectile en le faisant suivre la rotation pour viser le joueur
            GameObject projectile = Instantiate(orbProjectilePrefab, firePoint.position, rotationToPlayer);
            
            // Tu peux ajouter une force au projectile si il a un Rigidbody
            // Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            // if (projRb != null) {
            //     projRb.AddForce(directionToPlayer * projectileSpeed, ForceMode2D.Impulse); // projectileSpeed doit être défini pour le projectile
            // }

            // Optionnel : Son de tir, etc.
            Debug.Log("Boss tire une orbe en direction du joueur !");
        }
    }
    // Gère la mort du boss.
    void HandleBossDeath()
    {
        Debug.Log("Boss est mort. Combat terminé.");
        currentBossPhase = BossPhase.Dying; // Passe en état de mort
        StopAllBossCoroutines(); // Arrête tout comportement

        // Ici, tu peux déclencher l'animation de mort du boss, des explosions, etc.
        // bossAnimator.SetTrigger("Die");

        // Par exemple, désactiver le collider pour qu'il ne bloque plus rien
        // if (GetComponent<Collider2D>() != null) GetComponent<Collider2D>().enabled = false;
        // if (rb != null) rb.linearVelocity = Vector2.zero; // Arrête tout mouvement

        // Ensuite, après l'animation de mort, faire apparaitre le teleporteur
        // StartCoroutine(LoadNextSceneTPAfterDelay(3f)); // Attend 3 secondes avant de charger la scène
    }
    IEnumerator LoadNextSceneTPAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // NextZoneTP.enabled = true // REMPLACE PAR LE NOM DE TA SCÈNE DE VICTOIRE
    }

    // Pour le debug : visualiser la portée de patrouille
    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) // Affiche pendant le jeu pour la phase en cours
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(new Vector3(initialPosition.x, transform.position.y, transform.position.z), new Vector3(currentPatrolRange * 2, 1, 1));
        }
        else // Affiche en édition pour la phase 1 (valeurs par défaut)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(phase1PatrolRange * 2, 1, 1));
        }
    }
}