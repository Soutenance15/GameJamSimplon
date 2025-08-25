using System.Collections;
using UnityEngine;

public class EnemyPc : MonoBehaviour
{
    public GameObject side1;
    public GameObject side2;
    public float scaleDuration = 2.5f; // Durée de l'animation

    public Transform progressBar; // Barre visuelle à scaler sur X
    public float activationTime = 2f; // Temps pour remplir la barre
    public float resetSpeed = 1f; // Vitesse de descente de la barre (en secondes pour vider totalement)
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 2f;
    private GameObject player;
    private Player playerInstance;
    private float progress = 0f;
    private bool isDisabled = false;
    public bool lastDisable = false;

    [SerializeField]
    private PCCounter pcCounter;

    [Header("Prefab de bombe (ennemi explosif)")]
    public GameObject explosiveEnemyPrefab;
    private bool everCollidedByPlayer = false;

    [Header("Bombe en ligne")]
    public int numberBombs; // Modifiable dans l’inspecteur !

    private IEnumerator Scale(GameObject obj, float duration, Vector3 fromScale, Vector3 toScale)
    {
        float t = 0f;

        obj.transform.localScale = fromScale;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            obj.transform.localScale = Vector3.Lerp(fromScale, toScale, t);
            yield return null;
        }
        obj.transform.localScale = toScale;
    }

    private void ScaleUp(GameObject obj, float duration)
    {
        Vector3 fromScale = new Vector3(0.5f, 0f, 1f);
        Vector3 toScale = new Vector3(0.5f, 1f, 1f);
        StartCoroutine(Scale(obj, duration, fromScale, toScale));
    }

    private void ScaleDown(GameObject obj, float duration)
    {
        Vector3 fromScale = new Vector3(0.5f, 1f, 1f);
        Vector3 toScale = new Vector3(0.5f, 0f, 1f);
        StartCoroutine(Scale(obj, duration, fromScale, toScale));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !everCollidedByPlayer)
        {
            playerInstance = (Player)collision.gameObject.GetComponent<Player>();
            playerInstance.RecallFriend();
            playerInstance.disableDeployFriend = true;
            everCollidedByPlayer = true;
            ScaleUp(side1, scaleDuration);
            ScaleUp(side2, scaleDuration);
            StartCoroutine(DropBombRain());
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetProgressBar(0f);
        side1.transform.localScale = Vector3.zero;
        side2.transform.localScale = Vector3.zero;
    }

    void Update()
    {
        if (isDisabled)
            return;
        if (player == null)
            return;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool isPlayerNear = distance <= interactionDistance;
        bool isHolding = isPlayerNear && Input.GetKey(interactionKey);
        if (isHolding)
        {
            progress += Time.deltaTime / activationTime;
        }
        else
        {
            progress -= Time.deltaTime / resetSpeed;
        }
        progress = Mathf.Clamp01(progress);
        SetProgressBar(progress);
        if (progress >= 1f && !isDisabled)
        {
            DisablePc();
        }
    }

    void SetProgressBar(float value)
    {
        if (progressBar != null)
        {
            Vector3 localScale = progressBar.localScale;
            progressBar.localScale = new Vector3(value, localScale.y, localScale.z);
        }
    }

    void DisablePc()
    {
        //TODO
        // CETTE GESTION N'est pas bonne a changé
        // il vaut mieux un atbleau pour le jeu une bonne fois pour toute
        GameObject[] allPCs = GameObject.FindGameObjectsWithTag("EnemyPC");
        foreach (GameObject pc in allPCs)
        {
            pc.GetComponent<EnemyPc>().lastDisable = false;
        }
        isDisabled = true;
        lastDisable = true;
        if (null != playerInstance)
        {
            playerInstance.disableDeployFriend = false;
        }
        ScaleDown(side1, scaleDuration);
        ScaleDown(side2, scaleDuration);
        // TODO
        // Donne le bon Pc Counter (hud) après
        if (pcCounter != null)
        {
            pcCounter.PCDeactivated();
        }
        // else
        // {
        //     Debug.LogWarning("Référence à PC Counter non assignée !");
        // }
    }

    public IEnumerator DropBombRain()
    {
        Vector3 playerPos = player.transform.position;

        for (int i = 0; i < 3; i++)
        {
            CreateLineBombToDrop();
            yield return new WaitForSeconds(1f); // Pause de 0.5s entre chaque rafale
        }
    }

    void CreateLineBombToDrop()
    {
        if (numberBombs <= 2f)
        {
            // Au minimum 3 bombes a placer
            numberBombs = 3;
        }
        // Choisit au hasard quel index ne sera PAS affiché
        int bombeVide = Random.Range(0, numberBombs); // inclut 0, exclut numberBombs
        // int bombeVide2 = Random.Range(0, numberBombs); // inclut 0, exclut numberBombs

        for (int i = 0; i <= numberBombs; i++)
        {
            int index = i; // la bombe de droite (indices croissants)
            // if (index == bombeVide || index == bombeVide2)
            if (index == bombeVide)
            {
                continue;
            }

            float coeff = 2f;
            Vector3 spawnPos = new Vector3(
                transform.position.x - (numberBombs * coeff / 2) + coeff * i,
                transform.position.y + 4.5f,
                transform.position.z
            );
            Instantiate(explosiveEnemyPrefab, spawnPos, Quaternion.identity);
        }
    }
}
