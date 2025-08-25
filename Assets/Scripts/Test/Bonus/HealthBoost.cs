using System.Collections;
using UnityEngine;

public class HealthBoost : MonoBehaviour
{
    public float energyAmount = 25f;
    public float respawnDelay = 15f;

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
        bonusCollider.enabled = false;
        if (childSpriteRenderer != null)
            childSpriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        bonusCollider.enabled = true;
        if (childSpriteRenderer != null)
            childSpriteRenderer.enabled = true;
    }
}
