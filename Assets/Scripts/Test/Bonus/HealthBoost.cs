using System.Collections;
using TMPro;
using UnityEngine;

public class HealthBoost : MonoBehaviour
{
    public float energyAmount = 25f;
    public float respawnDelay = 15f;
    private float respawnTimer = 0f;
    public TextMeshPro respawnTimerText;
    private Collider2D bonusCollider;
    private SpriteRenderer childSpriteRenderer;

    void Awake()
    {
        bonusCollider = GetComponent<Collider2D>();
        childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.AddEnergy(energyAmount); // Méthode dans Player.cs
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        respawnTimer = respawnDelay;
        StartCoroutine(UpdateTimerDisplay());
        // cacher le bonus
        GetComponent<Collider2D>().enabled = false;
        GetComponentInChildren<SpriteRenderer>().enabled = false;
        while (respawnTimer > 0)
        {
            yield return new WaitForSeconds(1f);
            respawnTimer--;
        }
        // afficher le bonus
        GetComponent<Collider2D>().enabled = true;
        GetComponentInChildren<SpriteRenderer>().enabled = true;
        respawnTimerText.text = ""; // vide à la fin
    }

    private IEnumerator UpdateTimerDisplay()
    {
        while (respawnTimer > 0)
        {
            respawnTimerText.text = respawnTimer.ToString("0");
            yield return null;
        }
    }
}
