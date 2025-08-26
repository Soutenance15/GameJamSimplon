using UnityEngine;

public class EnemyTriggerSpawner : MonoBehaviour
{
    [Header("Liste des ennemis à activer")]
    public GameObject[] enemies;

    private bool hasSpawned = false;

    private void Start()
    {
        foreach (GameObject enemyGO in enemies)
        {
            if (!hasSpawned && enemyGO != null && enemyGO.activeSelf)
                enemyGO.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasSpawned && other.CompareTag("Player"))
        {
            Debug.Log("avant");
            foreach (GameObject enemyGO in enemies)
            {
                if (enemyGO != null && !enemyGO.activeSelf)
                {
                    enemyGO.SetActive(true);
                }
            }
            hasSpawned = true;
        }
    }
}
