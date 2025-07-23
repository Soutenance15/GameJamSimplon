using UnityEngine;

public class PCEnemyInteraction : MonoBehaviour
{
    public Transform progressBar;        // Barre visuelle à scaler sur X
    public float activationTime = 2f;    // Temps pour remplir la barre
    public float resetSpeed = 1f;        // Vitesse de descente de la barre (en secondes pour vider totalement)
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 2f;

    private GameObject player;
    private float progress = 0f;
    private bool isDisabled = false;
    [SerializeField] private PCCounter pcCounter;



    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetProgressBar(0f);
    }

    void Update()
    {
        if (isDisabled) return;
        if (player == null) return;

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

        if (progress >= 1f)
        {
            ActivatePC();
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

    void ActivatePC()
    {
        isDisabled = true;
        Debug.Log("PC activé !");

        if (pcCounter != null)
        {
            pcCounter.PCDeactivated();
        }
        else
        {
            Debug.LogWarning("Référence à PC Counter non assignée !");
        }
    }
}
