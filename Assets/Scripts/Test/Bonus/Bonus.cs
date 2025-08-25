using System.Collections;
using TMPro;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public float multiplier = 2f;
    public float duration = 8f;
    public float respawnDelay = 15f;
    private float respawnTimer = 0f;
    public TextMeshPro respawnTimerText;
    // private Collider2D bonusCollider;
    // private SpriteRenderer childSpriteRenderer;

    // void Awake()
    // {
    //     bonusCollider = GetComponent<Collider2D>();
    //     childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null && player.friendInstance != null)
        {
            Friend friend = player.friendInstance.GetComponent<Friend>();
            if (friend != null)
            {
                ActivateBonus(player, friend, multiplier, duration);
                StartCoroutine(RespawnRoutine());
            }
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

    public virtual void ActivateBonus(
        Player player,
        Friend friend,
        float multiplier,
        float duration
    )
    {
        if (!player.isFriendDeployed)
        {
            player.DeployFriend();
        }
    }
}
