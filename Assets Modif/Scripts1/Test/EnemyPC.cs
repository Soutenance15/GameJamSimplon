using System.Collections.Generic;
using UnityEngine;

public class EnemyPc : MonoBehaviour
{
    public Transform progressBar; // Barre visuelle à scaler sur X
    public float activationTime = 2f; // Temps pour remplir la barre
    public float resetSpeed = 1f; // Vitesse de descente de la barre (en secondes pour vider totalement)
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 2f;
    private GameObject player;
    private float progress = 0f;
    private bool isDisabled = false;

    [SerializeField]
    private PCCounter pcCounter;

    [Header("Prefab de bombe (ennemi explosif)")]
    public GameObject explosiveEnemyPrefab;

    [Header("Bombe en ligne")]
    public int nombreDeBombesDansLaLigne = 12; // Modifiable dans l’inspecteur !
    public float largeurLigne = 8f; // Largeur totale de la ligne (modifie la distance entre bombes)
    public float hauteurLigne = 3f; // Hauteur au-dessus du joueur où la ligne tombe (en unités)

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetProgressBar(0f);
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
        isDisabled = true;
        Debug.Log("PC désactivé !");
        // TODO
        // Donne le bon Pc Counter (hud) après
        // if (pcCounter != null)
        // {
        //     pcCounter.PCDeactivated();
        DropBombRain();
        // }
        // else
        // {
        //     Debug.LogWarning("Référence à PC Counter non assignée !");
        // }
    }

    public void DropBombRain()
    {
        Vector3 playerPos = player.transform.position;

        // float offsetY = 4f;

        for (int i = 0; i < 3; i++)
        {
            CreateLineBombToDrop(playerPos, 2);
        }
    }

    void CreateLineBombToDrop(Vector3 playerPos, float offsetY)
    {
        // Vector3 playerPos = player.transform.position;
        // Choisit au hasard quel index ne sera PAS affiché
        int bombeVide = Random.Range(0, nombreDeBombesDansLaLigne); // inclut 0, exclut nombreDeBombesDansLaLigne
        int bombeVide2 = Random.Range(0, nombreDeBombesDansLaLigne); // inclut 0, exclut nombreDeBombesDansLaLigne

        for (int i = 0; i < nombreDeBombesDansLaLigne; i++)
        {
            int index = i; // la bombe de droite (indices croissants)
            if (index == bombeVide)
                continue;

            Vector3 spawnPos = new Vector3(
                playerPos.x - (nombreDeBombesDansLaLigne / 2) + i,
                playerPos.y + 1.5f * offsetY,
                playerPos.z
            );
            Instantiate(explosiveEnemyPrefab, spawnPos, Quaternion.identity);
        }
        // Partie négative (gauche)
        // for (int i = 1; i < nombreDeBombesDansLaLigne / 2; i++)
        // {
        //     int index = (nombreDeBombesDansLaLigne / 2) + (i - 1); // indices du côté gauche
        //     if (index == bombeVide || index == bombeVide2)
        //         continue;

        //     Vector3 spawnPos = new Vector3(
        //         playerPos.x - 1.25f * i,
        //         playerPos.y + 2 * offsetY,
        //         playerPos.z
        //     );
        //     Instantiate(explosiveEnemyPrefab, spawnPos, Quaternion.identity);
        // }
    }
}
