using UnityEngine;
using System.Collections;
using UnityEngine.UI; // Pour Image si tu utilises un background
using TMPro; // Pour TextMeshPro
using UnityEngine.SceneManagement; // Pour changer de scène

public class IntroCinematicManager : MonoBehaviour
{
    public TextMeshProUGUI storyText; // Drag & drop ton TextMeshPro ici dans l'Inspector
    public Image backgroundDim; // Optionnel : Drag & drop ton background ici
    public float textDisplaySpeed = 0.05f; // Vitesse de défilement du texte (caractère par caractère)
    public float timeBetweenLines = 3f; // Temps d'attente entre chaque ligne de dialogue APRÈS qu'elle soit entièrement affichée
    public float timeAfterLastLine = 2f; // Temps avant de charger la scène suivante après la dernière ligne
    public string nextSceneName = "Finale Level V2"; // Nom de ta scène de jeu

    [TextArea(5, 10)] // Permet d'avoir une zone de texte multi-lignes dans l'Inspector
    public string[] cinematicLines; // Tes lignes de dialogue pour la cinématique

    private int currentLineIndex = 0;
    private bool isTyping = false; // Indique si le texte est en train de s'écrire caractère par caractère

    void Start()
    {
        // Initialise le texte et le fond
        if (storyText != null)
        {
            storyText.text = "";
            storyText.gameObject.SetActive(true); // S'assurer que le texte est actif
        }
        if (backgroundDim != null)
        {
            backgroundDim.gameObject.SetActive(true); // S'assurer que le fond est actif
        }

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

        StartCoroutine(PlayCinematic());
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

    IEnumerator PlayCinematic()
    {
        // Boucle pour afficher chaque ligne de la cinématique
        for (currentLineIndex = 0; currentLineIndex < cinematicLines.Length; currentLineIndex++)
        {
            yield return TypeLine(cinematicLines[currentLineIndex]); // Affiche la ligne caractère par caractère

            // Attendre un délai avant de passer à la ligne suivante
            yield return new WaitForSeconds(timeBetweenLines);
        }

        // Toutes les lignes ont été affichées, attendre un peu avant de charger la scène de jeu
        yield return new WaitForSeconds(timeAfterLastLine);

        // Charger la scène de jeu
        SceneManager.LoadScene(nextSceneName);
    }
    IEnumerator TypeLine(string line)
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

    // void Update()
    // {
    //     // Si le joueur appuie sur une touche pendant que le texte défile,
    //     // on peut afficher la ligne entière instantanément.
    //     if (isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
    //     {
    //         StopAllCoroutines(); // Arrête le défilement actuel
    //         storyText.text = cinematicLines[currentLineIndex]; // Affiche la ligne complète
    //         // isTyping = false;
    //     }
    //     // else if (!isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
    //     // {
    //     //     PlayCinematic();
    //     // }
    // }
}
