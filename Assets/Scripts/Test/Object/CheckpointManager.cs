using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    private Vector3 checkpointPosition;
    private float playerHealth; // Ajoute d’autres états à sauvegarder si besoin

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        // Stocke ici d’autres états : score, vie, inventaire…
        // Ex : playerHealth = FindObjectOfType<Player>().currentHealth;
    }

    public void RespawnPlayer(GameObject player)
    {
        player.transform.position = checkpointPosition;
        // Restaure la santé, le score, le nombre de bombes… selon besoins
        // player.currentHealth = playerHealth;
    }
}
