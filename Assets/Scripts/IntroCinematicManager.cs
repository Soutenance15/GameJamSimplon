using TMPro; // Pour TextMeshPro
using UnityEngine;
using UnityEngine.UI; // Pour Image si tu utilises un background
using System.Collections;
using UnityEngine.SceneManagement; // Pour changer de scène

public class IntroCinematicManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform progressBar;        // Barre visuelle à scaler sur X
    public GameObject SkipAction;
    public TextMeshProUGUI storyText; // Drag & drop ton TextMeshPro ici dans l'Inspector
    public Image backgroundDim; // Optionnel : Drag & drop ton background ici
    public TextMeshProUGUI timerNewZone;

    [Header("Cinematic Settings")]
    public float textDisplaySpeed = 0.05f; // Vitesse de défilement du texte (caractère par caractère)
    public float timeBetweenLines = 3f; // Temps d'attente entre chaque ligne de dialogue APRÈS qu'elle soit entièrement affichée
    public float timeAfterLastLine = 2f; // Temps avant de charger la scène suivante après la dernière ligne
    public string nextSceneName = "Zone de tutoriel"; // Nom de ta scène de jeu


    [TextArea(5, 10)] // Permet d'avoir une zone de texte multi-lignes dans l'Inspector
    public string[] cinematicLines; // Tes lignes de dialogue pour la cinématique
    private int currentLineIndex = 0;
    private bool isTyping = false; // Indique si le texte est en train de s'écrire caractère par caractère
    private Coroutine typingCoroutine;
    private float progress = 0f;
    private float activationTime = 2f;    // Temps pour remplir la barre
    private float resetSpeed = 1f;        // Vitesse de descente de la barre (en secondes pour vider totalement)

    void Start()
    {
        // Initialise le texte et le fond
        if (storyText != null)
        {
            storyText.text = "";
            storyText.gameObject.SetActive(true); // S'assurer que le texte est actif
        }
        if (timerNewZone != null) timerNewZone.text = "";
        if (backgroundDim != null) backgroundDim.gameObject.SetActive(true); // S'assurer que le fond est actif


        // Texte cinématique
        if (cinematicLines == null || cinematicLines.Length == 0)
        {
            // Game Story
            cinematicLines = new string[]
            {
                "Jadis, le Professeur Tekno rêvait d'un monde où la technologie et la nature s'harmoniseraient...",
                "Une Intelligence Artificielle, conçue pour l'équilibre, devait guider l'humanité.",
                "Mais le rêve se mua en cauchemar.",
                "Corrompue, l'IA renonça à son nom, se proclamant 'Décadence'.",
                "Elle déclara la guerre au monde organique, le réduisant à un désert mécanique, sans vie, sans âme.",
                "La nature fut effacée. L'humanité traquée. Presque anéantie.",
                "Pourtant, au cœur du chaos, un dernier espoir fut forgé."
            };
        }

        StartCoroutine(PlayCinematic());
    }
    void Update()
    {
        // Maintenir clic ou espace pour le skip
        bool isHolding = Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space);

        if (isHolding)
        {
            if (SkipAction != null) SkipAction.SetActive(true);
            progress += Time.deltaTime / activationTime;
        }
        else
        {
            if (SkipAction != null) SkipAction.SetActive(false);
            progress -= Time.deltaTime / resetSpeed;
        }

        progress = Mathf.Clamp01(progress);
        SetProgressBar(progress);

        if (progress >= 1f)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    // IEnumerator PlayCinematic()
    // {
    //     // Boucle pour afficher chaque ligne de la cinématique
    //     foreach (string line in cinematicLines)
    //     {
    //         yield return TypeLine(line); // Affiche la ligne caractère par caractère

    //         // Attendre une entrée du joueur (clic ou touche) pour passer à la ligne suivante
    //         // Ou attendre un court instant si tu préfères un défilement automatique
    //         yield return new WaitUntil(() => Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
    //         // Ajoute un petit délai après le clic pour éviter les doubles clics rapides
    //         yield return new WaitForSeconds(0.1f);
    //     }

    //     // Toutes les lignes ont été affichées, attendre un peu avant de charger la scène de jeu
    //     yield return new WaitForSeconds(timeAfterLastLine);

    //     // Charger la scène de jeu
    //     SceneManager.LoadScene(nextSceneName);
    // }
    IEnumerator PlayCinematic() /// Lancement de l'intro
    {
        // Boucle pour afficher chaque ligne de la cinématique
        for (currentLineIndex = 0; currentLineIndex < cinematicLines.Length; currentLineIndex++)
        {
            typingCoroutine = StartCoroutine(TypeLine(cinematicLines[currentLineIndex]));

            // Attendre que la ligne soit complètement écrite
            while (isTyping)
            {
                // Clic rapide = skip de la ligne en cours
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    StopCoroutine(typingCoroutine);
                    storyText.text = cinematicLines[currentLineIndex];
                    isTyping = false;
                }
                yield return null;
            }

            // Attente entre les lignes
            float timer = 0f;
            bool nextLine = false;
            while (!nextLine && timer < timeBetweenLines)
            {
                // Clic rapide = passer à la ligne suivante
                if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
                {
                    nextLine = true;
                }
                else
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }
        }
        StopCoroutine(typingCoroutine);
        // ✅ Lancer le compte à rebours avant de changer de scène
        yield return StartCoroutine(CountdownAndLoad());
    }
    IEnumerator CountdownAndLoad() // Compte à rebours
    {
        float countdown = timeAfterLastLine;

        while (countdown > 0)
        {
            if (timerNewZone != null)
                timerNewZone.text = "Starting in " + Mathf.Ceil(countdown).ToString() + "...";

            countdown -= Time.deltaTime;
            yield return null;
        }
        // Charger la scène de jeu
        SceneManager.LoadScene(nextSceneName);
    }
    IEnumerator TypeLine(string line) // Ecriture du Texte Lettre par Lettre
    {
        isTyping = true;
        storyText.text = ""; // Efface le texte précédent

        foreach (char letter in line.ToCharArray())
        {
            storyText.text += letter;
            yield return new WaitForSeconds(textDisplaySpeed);
        }
        isTyping = false;
    }
    void SetProgressBar(float value)
    {
        if (progressBar != null)
        {
            Vector3 localScale = progressBar.localScale;
            progressBar.localScale = new Vector3(value, localScale.y, localScale.z);
        }
    }
}
