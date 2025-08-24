using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroCinematicManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Transform progressBar;        // Barre visuelle à scaler sur X
    public GameObject SkipAction;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI timerNewZone;

    [Header("Cinematic Settings")]
    public float textDisplaySpeed = 0.05f; // vitesse d'affichage du text
    public float timeBetweenLines = 3f; // durée du compte à rebours entre chaque ligne
    public float timeAfterLastLine = 3f; // durée du compte à rebours après la dernière ligne
    public string nextSceneName = "Zone de tutoriel";


    [TextArea(5, 10)]
    public string[] cinematicLines;

    private int currentLineIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private float progress = 0f;
    private float activationTime = 2f;    // Temps pour remplir la barre
    private float resetSpeed = 1f;        // Vitesse de descente de la barre (en secondes pour vider totalement)

    void Start()
    {
        if (storyText != null) storyText.text = "";
        if (timerNewZone != null) timerNewZone.text = "";

        // Texte cinématique
        if (cinematicLines == null || cinematicLines.Length == 0)
        {
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
    IEnumerator PlayCinematic() /// Lancement de l'intro
    {
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

        SceneManager.LoadScene(nextSceneName);
    }
    IEnumerator TypeLine(string line) // Ecriture du Texte Lettre par Lettre
    {
        isTyping = true;
        storyText.text = "";
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
