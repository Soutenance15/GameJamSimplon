using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using TMPro;
using UnityEngine;

public class Friend : MonoBehaviour
{
    [Header("Bonus")]
    public TextMeshProUGUI bonusTimerText;
    public TextMeshProUGUI bonusLabel;
    private bool keepEnergyMax = false;
    private Coroutine fireRateLabelCoroutine;
    private Coroutine scaleLabelCoroutine;
    private int energyBonusCount = 0;
    private int keepEnergyMaxCount = 0;

    private float fireRateTimer = 0f;
    private float scaleTimer = 0f;

    [Header("Upgrade tir")]
    public float fireRateMultiplier = 1f; // 1f (=normal), supérieur à 1 = cadence plus rapide
    public float projectileScaleMultiplier = 1f; // 1f par défaut, >1 gros projectiles

    [Header("Indicateur de lock")]
    public GameObject lockIndicatorPrefab;
    private GameObject currentLockIndicator = null;

    [Header("Cooldown Shoot")]
    public Transform cooldownBarObject; // Cooldown pour le shoot

    [Header("Gestion énergie de tir")]
    public float energyMax = 15f; // Energie totale
    public float energy = 0f; // Energie courante
    public float energyCostPerShot = 1f; // Coût par tir
    public float energyRegenRate = 3f; // Energie par seconde (lorsque relâché)
    public float emptyLockDuration = 3f; // Durée du verrou si énergie 0 (secondes)

    private bool energyLocked = false;
    private float energyLockTimer = 0f;

    [Header("Projectile")]
    public GameObject projectilePrefab; // Préfabriqué du projectile
    public Transform firePoint; // Position de tir
    public float fireRate = 5f; // Coups par seconde
    float lastShotTime = 0f;

    [Header("Suivi du joueur")]
    public Transform player; // Cible à suivre
    public float speed = 4f; // Vitesse de déplacement
    public Vector2 followOffset = new Vector2(1.5f, 2f); // Décalage autour du joueur

    [Header("Évitement d'obstacle")]
    public float obstacleDetectDistance = 1.5f; // Distance maximum pour détecter un obstacle
    public float obstacleAvoidHeight = 1.5f; // Hauteur pour contourner l'obstacle
    public LayerMask obstacleLayer; // Layer à assigner dans l'inspecteur (ex : Ground)

    [Header("Détection d'ennemis")]
    public string enemyTag = "Enemy"; // Tag utilisé pour les ennemis
    public float enemyDetectDistance = 20f; // Distance maximum pour cibler un ennemi
    private Transform lockedEnemy = null; // Cible actuellement verrouillée
    private int lockedEnemyIndex = -1; // Index dans la liste cyclique
    private List<Transform> validEnemies = new List<Transform>();

    [Header("Stand-by/Immobilisation")]
    public float blockTimeBeforeIdle = 3f; // Temps sans progrès avant immobilisation (secondes)
    public float minDistanceProgress = 0.5f; // Distance minimale à parcourir en blockTime
    public float reactivationDistance = 12f; // Distance du joueur requise pour réactivation

    float followSide = 1f; // -1 = gauche, 1 = droite
    Vector3 lastPlayerPosition;

    // --- Gestion immobilisation ---
    Vector3 lastCheckedPos;
    float blockTimer = 0f;
    bool immobile = false;
    public bool useCooldown = false; // à afficher dans l’inspecteur

    void Start()
    {
        energy = energyMax;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0;
        }
        followSide = Mathf.Sign(transform.position.x - player.position.x);
        if (followSide == 0)
            followSide = 1;
        lastPlayerPosition = player.position;
        lastCheckedPos = transform.position;
    }

    void Update()
    {
        if (player == null)
            return;

        // -- Gestion du mode cooldown par touche U --
        // (à remplacer par tes propres logiques de zones ou triggers plus tard)
        if (Input.GetKeyDown(KeyCode.U))
        {
            useCooldown = !useCooldown;
        }

        // -- Immobilisation si le Friend ne progresse plus --
        if (!immobile)
        {
            blockTimer += Time.deltaTime;
            if (blockTimer >= blockTimeBeforeIdle)
            {
                float prog = Vector3.Distance(transform.position, lastCheckedPos);
                if (prog < minDistanceProgress)
                    immobile = true;
                blockTimer = 0f;
                lastCheckedPos = transform.position;
            }
        }
        else
        {
            float distToPlayer = Vector2.Distance(transform.position, player.position);
            Vector2 dirToPlayer = (player.position - transform.position).normalized;
            float directDist = Vector2.Distance(transform.position, player.position);
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                dirToPlayer,
                directDist,
                obstacleLayer
            );
            if (distToPlayer <= reactivationDistance && hit.collider == null)
            {
                immobile = false;
                blockTimer = 0f;
                lastCheckedPos = transform.position;
            }
        }

        // -- Mouvement et évitement d'obstacle --
        if (!immobile)
        {
            float playerSpeedX =
                (player.position.x - lastPlayerPosition.x) / Mathf.Max(Time.deltaTime, 0.0001f);
            if (Mathf.Abs(playerSpeedX) > 0.1f)
                followSide = Mathf.Sign(playerSpeedX);
            lastPlayerPosition = player.position;

            Vector2 offset = new Vector2(Mathf.Abs(followOffset.x) * followSide, followOffset.y);
            Vector3 targetPos = player.position + (Vector3)offset;

            Vector2 moveDir = (targetPos - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                moveDir,
                obstacleDetectDistance,
                obstacleLayer
            );
            if (hit.collider != null)
            {
                targetPos.y = Mathf.Max(targetPos.y, hit.point.y + obstacleAvoidHeight);
                RaycastHit2D ceiling = Physics2D.Raycast(
                    new Vector2(transform.position.x, hit.point.y + obstacleAvoidHeight),
                    Vector2.up,
                    0.5f,
                    obstacleLayer
                );
                if (ceiling.collider != null)
                    targetPos = transform.position;
            }

            // if (isBonusActive)
            // {
            //     bonusTimer -= Time.deltaTime;
            //     if (bonusTimerText != null)
            //         bonusTimerText.text = Mathf.Ceil(bonusTimer).ToString() + "s";

            //     if (bonusTimer <= 0f)
            //     {
            //         isBonusActive = false;
            //         if (bonusTimerText != null)
            //             bonusTimerText.text = "";
            //     }
            // }

            float moveDelta = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveDelta);
        }

        // ----- GESTION ENNEMIS / CIBLAGE -----
        UpdateEnemyList();

        // ----- DELock avec 'R' -----
        if (lockedEnemy != null && Input.GetKeyDown(KeyCode.R))
        {
            lockedEnemy = null;
            lockedEnemyIndex = -1;
        }

        // --- LOCK/CYCLE AVEC TAB ---
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (lockedEnemy == null)
            {
                float minDist = float.MaxValue;
                Transform closest = null;
                for (int i = 0; i < validEnemies.Count; i++)
                {
                    float d = Vector2.Distance(transform.position, validEnemies[i].position);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = validEnemies[i];
                        lockedEnemyIndex = i;
                    }
                }
                if (closest != null)
                    lockedEnemy = closest;
            }
            else if (validEnemies.Count >= 2)
            {
                int idx = validEnemies.IndexOf(lockedEnemy);
                idx = (idx + 1) % validEnemies.Count;
                lockedEnemy = validEnemies[idx];
                lockedEnemyIndex = idx;
            }
        }

        // --- Sortie du lock si cible morte/hors champ ---
        if (lockedEnemy != null)
        {
            if (
                !lockedEnemy.gameObject.activeInHierarchy
                || Vector2.Distance(transform.position, lockedEnemy.position) > enemyDetectDistance
            )
            {
                lockedEnemy = null;
                lockedEnemyIndex = -1;
            }
        }

        // --- Sélection de la cible à viser/tirer ---
        Transform targetToAim = null;
        if (lockedEnemy == null)
        {
            float minDist = float.MaxValue;
            Transform closest = null;
            foreach (var enemy in validEnemies)
            {
                float d = Vector2.Distance(transform.position, enemy.position);
                if (d < minDist)
                {
                    minDist = d;
                    closest = enemy;
                }
            }
            targetToAim = closest;
        }
        else
        {
            targetToAim = lockedEnemy;
        }

        // --- ROTATION & SKIN ---
        float rotationSpeed = 500f;
        if (targetToAim != null)
        {
            Vector2 dir = (targetToAim.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
            skinTarget();
        }
        else
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.identity,
                rotationSpeed * Time.deltaTime
            );
            skinIdle();
        }

        // -------- SHOOT (avec ou sans cooldown) --------
        if (useCooldown)
        {
            // --- Mode complexe : énergie/cooldown ---
            if (energyLocked)
            {
                energyLockTimer += Time.deltaTime;
                if (energyLockTimer >= emptyLockDuration)
                {
                    energyLocked = false;
                    energyLockTimer = 0f;
                    energy = Mathf.Min(energyMax, energyRegenRate * Time.deltaTime);
                }
            }
            else
            {
                bool shootButton = Input.GetKey(KeyCode.LeftControl);
                if (
                    shootButton
                    && energy >= energyCostPerShot
                    && Time.time > lastShotTime + 1f / (fireRate * fireRateMultiplier)
                )
                {
                    if (targetToAim != null && projectilePrefab != null && firePoint != null)
                    {
                        Vector2 directionProjectile = (
                            targetToAim.position - firePoint.position
                        ).normalized;
                        GameObject proj = Instantiate(
                            projectilePrefab,
                            firePoint.position,
                            Quaternion.identity
                        );
                        proj.transform.localScale *= projectileScaleMultiplier;

                        var projScript = proj.GetComponent<ProjectileBasic>();
                        if (projScript != null)
                            projScript.Init(directionProjectile, ProjectileSource.Friend);
                        lastShotTime = Time.time;

                        // MODIFIE ICI
                        if (!keepEnergyMax)
                        {
                            energy -= energyCostPerShot;
                            energy = Mathf.Max(energy, 0f);
                            if (energy == 0)
                            {
                                energyLocked = true;
                                energyLockTimer = 0f;
                            }
                        }
                        else
                        {
                            energy = energyMax; // énergie toujours max pendant le bonus
                        }
                    }
                }

                // Énergie regen
                if (keepEnergyMax)
                {
                    energy = energyMax;
                }
                else if (!shootButton && energy < energyMax)
                {
                    energy += energyRegenRate * Time.deltaTime;
                    energy = Mathf.Min(energy, energyMax);
                }
            }

            // -- Barre d'énergie --
            if (cooldownBarObject != null)
            {
                cooldownBarObject.gameObject.SetActive(true);
                Vector3 localScale = cooldownBarObject.localScale;
                cooldownBarObject.localScale = new Vector3(
                    energy / energyMax,
                    localScale.y,
                    localScale.z
                );
            }
        }
        else
        {
            // --- Mode simple (pas de cooldown, pas d'énergie) ---
            bool shootButton = Input.GetKey(KeyCode.LeftControl);
            if (shootButton && Time.time > lastShotTime + 1f / (fireRate * fireRateMultiplier))
            {
                if (targetToAim != null && projectilePrefab != null && firePoint != null)
                {
                    Vector2 directionProjectile = (
                        targetToAim.position - firePoint.position
                    ).normalized;
                    GameObject proj = Instantiate(
                        projectilePrefab,
                        firePoint.position,
                        Quaternion.identity
                    );
                    proj.transform.localScale *= projectileScaleMultiplier;

                    var projScript = proj.GetComponent<ProjectileBasic>();
                    if (projScript != null)
                        projScript.Init(directionProjectile, ProjectileSource.Friend);
                    lastShotTime = Time.time;
                }
            }
            // Barre d’énergie cachée en mode simple
            if (cooldownBarObject != null)
                cooldownBarObject.gameObject.SetActive(false);
        }

        // -------- Indicateur de lock (triangle, losange, etc.) --------
        if (lockedEnemy != null)
        {
            if (currentLockIndicator == null)
                currentLockIndicator = Instantiate(lockIndicatorPrefab);

            // Position pile au-dessus (ajuste le 1.5f selon ton sprite/taille)
            currentLockIndicator.transform.position = lockedEnemy.position + Vector3.up * 1.5f;
            currentLockIndicator.SetActive(true);
        }
        else
        {
            if (currentLockIndicator != null)
                currentLockIndicator.SetActive(false);
        }
    }

    public void ActivateBoostShoot(float multiplier, float duration)
    {
        energyBonusCount++;
        keepEnergyMax = true;
        fireRateMultiplier = multiplier;
        fireRateTimer = duration;
        gameObject.SetActive(true);
        if (fireRateLabelCoroutine == null)
            fireRateLabelCoroutine = StartCoroutine(FireRateLabelRoutine());
        StartCoroutine(FireRateBoostRoutine(duration));
        StartCoroutine(SharedEnergyRoutine(duration));
    }

    private IEnumerator FireRateBoostRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        fireRateMultiplier = 1f;
        energyBonusCount--;
        if (energyBonusCount <= 0)
            keepEnergyMax = false;
    }

    private IEnumerator FireRateLabelRoutine()
    {
        while (fireRateTimer > 0f)
        {
            UpdateBonusLabel();
            yield return null;
            fireRateTimer -= Time.deltaTime;
        }
        UpdateBonusLabel();
        fireRateLabelCoroutine = null;
    }

    public void ActivateProjectileScaleBoost(float multiplier, float duration)
    {
        energyBonusCount++;
        keepEnergyMax = true;

        projectileScaleMultiplier = multiplier;
        scaleTimer = duration;
        if (scaleLabelCoroutine == null)
            scaleLabelCoroutine = StartCoroutine(ScaleLabelRoutine());
        StartCoroutine(ProjectileScaleBoostRoutine(duration));
        StartCoroutine(SharedEnergyRoutine(duration));
    }

    private IEnumerator ProjectileScaleBoostRoutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        projectileScaleMultiplier = 1f;
        energyBonusCount--;
        if (energyBonusCount <= 0)
            keepEnergyMax = false;
    }

    private IEnumerator SharedEnergyRoutine(float duration)
    {
        keepEnergyMaxCount++;
        keepEnergyMax = true;
        yield return new WaitForSeconds(duration);
        keepEnergyMaxCount--;
        if (keepEnergyMaxCount <= 0)
        {
            keepEnergyMax = false;
            keepEnergyMaxCount = 0; // Sécurité anti-valeurs négatives
        }
    }

    public bool IsBonusActive()
    {
        return keepEnergyMaxCount > 0;
    }

    private IEnumerator ScaleLabelRoutine()
    {
        while (scaleTimer > 0f)
        {
            UpdateBonusLabel();
            yield return null;
            scaleTimer -= Time.deltaTime;
        }
        UpdateBonusLabel();
        scaleLabelCoroutine = null;
    }

    private void UpdateBonusLabel()
    {
        string txt = "";
        if (fireRateTimer > 0f)
            txt += "Tir Boosté : " + Mathf.Ceil(fireRateTimer) + "s\n";
        if (scaleTimer > 0f)
            txt += "Projectiles Géants : " + Mathf.Ceil(scaleTimer) + "s\n";

        if (!string.IsNullOrEmpty(txt))
        {
            bonusLabel.gameObject.SetActive(true);
            bonusLabel.text = txt;
        }
        else
        {
            bonusLabel.gameObject.SetActive(false);
        }
    }

    private void skinTarget()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color32(176, 56, 26, 255); // Par exemple, rouge quand il cible
    }

    private void skinIdle()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = new Color32(95, 176, 26, 255); // Par exemple, rouge quand il cible
    }

    // Renvoie l'ennemi le plus proche, en ignorant ceux non ciblables
    Transform FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Transform closest = null;
        float minDistance = enemyDetectDistance;
        foreach (GameObject enemy in enemies)
        {
            bool isNotTargetable = enemy.GetComponent<EnemyNotTargetable>() != null;
            if (!isNotTargetable)
            {
                float dist = Vector2.Distance(transform.position, enemy.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = enemy.transform;
                }
            }
        }
        return closest;
    }

    void UpdateEnemyList()
    {
        validEnemies.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        foreach (GameObject enemy in enemies)
        {
            // Ici, adapte si tu as un système pour ignorer certains ennemis (non-ciblables)
            float dist = Vector2.Distance(transform.position, enemy.transform.position);
            if (dist <= enemyDetectDistance && enemy.activeInHierarchy)
            {
                // Optionnel : teste ici d'autres critères (vie/mort, etc.)
                validEnemies.Add(enemy.transform);
            }
        }
    }

    public void NextTarget()
    {
        UpdateEnemyList();
        if (validEnemies.Count == 0)
        {
            lockedEnemy = null;
            lockedEnemyIndex = -1;
            return;
        }

        // Trouve l'index actuel dans la nouvelle liste (si la cible existe encore)
        if (lockedEnemy != null)
            lockedEnemyIndex = validEnemies.IndexOf(lockedEnemy);

        // Passe à la suivante
        lockedEnemyIndex = (lockedEnemyIndex + 1) % validEnemies.Count;
        lockedEnemy = validEnemies[lockedEnemyIndex];
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            player.gameObject.GetComponent<Player>().RecallFriend();
        }
    }
}
