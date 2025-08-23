using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string sceneToLoad;
    private bool playerInsidePortal = false;
    private Transform playerTransform = null;
    private bool isAbsorbing = false;

    public float attractDuration = 0.6f; // Durée de l’aspiration vers le centre
    public float shrinkDuration = 0.5f; // Durée du rapetissement

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAbsorbing && other.CompareTag("Player"))
        {
            playerInsidePortal = true;
            playerTransform = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!isAbsorbing && other.CompareTag("Player"))
        {
            playerInsidePortal = false;
            playerTransform = null;
        }
    }

    void Update()
    {
        // Quand le joueur est dans le portail + appuie sur E
        if (playerInsidePortal && !isAbsorbing && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(AbsorbPlayerAndLoad());
        }
    }

    // Coroutine qui anime le joueur : attiré puis rapetissé, puis change de scène
    private IEnumerator AbsorbPlayerAndLoad()
    {
        isAbsorbing = true;

        // Désactive la physique pour le joueur (plus de gravité, pas de mouvement physique)
        var player = playerTransform.gameObject.GetComponent<Player>();
        if (player.isFriendDeployed)
        {
            player.RecallFriend();
            player.controlDisable = true;
        }
        var rb = playerTransform.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; // Stop tout mouvement direct
            rb.bodyType = RigidbodyType2D.Kinematic; // Physique désactivée, déplacement manuel
        }

        // ---- Phase 1 : Aspiration au centre (effet "Lerp") ----
        Vector3 startPos = playerTransform.position; // D'où il part
        Vector3 targetPos = transform.position; // Centre du portail (vers où il va)
        float t = 0f;
        // Animation progressive sur attractDuration secondes
        while (t < attractDuration)
        {
            t += Time.deltaTime;
            // Lerp = interpolation linéaire, "avance de 0% à 100% du chemin"
            playerTransform.position = Vector3.Lerp(startPos, targetPos, t / attractDuration);
            yield return null; // Patiente 1 frame
        }
        playerTransform.position = targetPos; // Fin : exact centre portail

        // ---- Phase 2 : Rapetissement (shrink progressif) ----
        t = 0f;
        Vector3 startScale = playerTransform.localScale; // Taille d'origine
        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float s = Mathf.Lerp(1f, 0f, t / shrinkDuration); // 1->0 = grand->plus petit
            // TODO EFFACE en dessous après
            // EXPLICATION DU LERP CAR C'est COMPLIQUE
            // (fait par chat GPT car j'ai rien compris même ses explications )
            // Quand t = 0 :
            // t/shrinkDuration = 0
            // donc Mathf.Lerp(1, 0, 0) → 1
            // ⇒ taille normale

            // Quand t = shrinkDuration :
            // t/shrinkDuration = 1
            // donc Mathf.Lerp(1, 0, 1) → 0
            // ⇒ taille nulle (disparu)

            // Entre les deux (par exemple à mi-parcours, t = shrinkDuration/2 = 0.25s si shrinkDuration = 0.5s)
            // t/shrinkDuration = 0.5
            // donc Mathf.Lerp(1, 0, 0.5) → 0.5
            // ⇒ taille de moitié
            playerTransform.localScale = startScale * s;
            // Sécurité : On force à rester au centre !
            playerTransform.position = targetPos;
            yield return null;
        }
        // Fin : taille nulle, joueur "disparu"
        playerTransform.localScale = Vector3.zero;
        playerTransform.position = targetPos;

        // Petite pause pour fluidité
        yield return new WaitForSeconds(0.2f);

        // ---- Phase 3 : Changement de scène ----
        SceneManager.LoadScene(sceneToLoad);
    }
}
