using System.Collections;
using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [Header("Réglages")]
    public float timeBeforeDisappear = 1f; // Temps avant la disparition
    public float timeBeforeRespawn = 3f; // Temps avant de réapparaître

    private Collider2D col;
    private SpriteRenderer sr;
    private bool playerOnPlatform = false;
    private bool disappearing = false;

    void Start()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        if (col == null)
            Debug.LogWarning("Aucun Collider2D sur la plateforme !");
        if (sr == null)
            Debug.LogWarning("Aucun SpriteRenderer sur la plateforme !");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!disappearing && collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            StartCoroutine(StartDisappearTimer());
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
        }
    }

    IEnumerator StartDisappearTimer()
    {
        float timer = 0f;
        while (timer < timeBeforeDisappear)
        {
            if (!playerOnPlatform)
                yield break; // Le joueur est descendu
            timer += Time.deltaTime;
            yield return null;
        }
        StartCoroutine(DisappearAndRespawn());
    }

    IEnumerator DisappearAndRespawn()
    {
        disappearing = true;
        col.enabled = false;
        if (sr != null)
            sr.enabled = false;
        yield return new WaitForSeconds(timeBeforeRespawn);
        col.enabled = true;
        if (sr != null)
            sr.enabled = true;
        disappearing = false;
    }
}
