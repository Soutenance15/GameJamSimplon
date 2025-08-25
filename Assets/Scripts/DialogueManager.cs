using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Pour TextMeshPro

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject playerControl;
    public GameObject backgroundDim;

    private Dictionary<string, string[]> dialogueDatabase = new Dictionary<string, string[]>();
    private string[] currentLines;
    private int index;

    void Start()
    {
        dialogueText.text = "";

        // 💬 Définir tous les dialogues ici
        dialogueDatabase.Add("activate_rebirth", new string[]
        {
            "???? : Salutations, petit être organique.",
            "Ash : Un r-r-ro-bot ... Qui parle.",
            "Rebirth : Je suis REBIRTH, entité autonome de type IA classe C-9.",
            "Ash : Arrière démon !",
            "Rebirth : Code d’identification : REB-CN009X. Création du professeur Tekno.",
            "Ash : Le professeur Tekno… ? M-Maais… C’est mon père !",
            "Rebirth : Confirmation : correspondance génétique 99,8 %. Tu es son descendant direct.",
            "Rebirth : Mission prioritaire mise à jour :\n—> Protéger Ash\n—> Restaurer l’équilibre biologique de la planète\n—> Neutraliser IA hostile : Décadence.",
            "Ash : …Papa savait qu’on allait devoir se battre.",
            "Rebirth : Il croyait en toi. Et… en moi. Nous ne sommes pas seuls, Ash. Nous formons une équipe."
        });

        dialogueDatabase.Add("ash_hit", new string[]
        {
            "Rebirth : Ash, reste près de moi. Je vais te protéger.",
            "Ash : Aïe... Ça va, je peux continuer.",
            "Rebirth : Sois prudent, je suis là pour toi.",
            "Ash : Je sais que mon père t’a créé pour ça. On va réussir, Rebirth."
        });

        dialogueDatabase.Add("pc_disabled", new string[]
        {
            "Rebirth : PC désactivé.",
            "Ash : Une étape de plus !",
            "Rebirth : Chaque pas que nous faisons, c’est un peu de vie que nous reprenons à Décadence.",
            "Rebirth : Regarde… la nature revient. Ce n’est que le début.",
            "Ash : C’est magnifique. On peut encore sauver ce monde."
        });

        dialogueDatabase.Add("pc_final_success", new string[]
        {
            "Rebirth : Tous les PC sont désactivés.",
            "Ash : Il est temps de lancer le protocole !"
        });

        dialogueDatabase.Add("pc_final_missing", new string[]
        {
            "Rebirth : Il manque encore des terminaux...",
            "Ash : Il faut tout désactiver avant d'agir ici."
        });
    }

    public void PlayDialogue(string key)
    {
        if (!dialogueDatabase.ContainsKey(key)) return;

        currentLines = dialogueDatabase[key];
        index = 0;

        if (playerControl) playerControl.SetActive(false);
        if (backgroundDim) backgroundDim.SetActive(true);

        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        dialogueText.text = "";
        foreach (char c in currentLines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentLines != null)
        {
            StopAllCoroutines();

            if (index < currentLines.Length - 1)
            {
                index++;
                StartCoroutine(TypeLine());
            }
            else
            {
                dialogueText.text = "";
                currentLines = null;

                if (playerControl) playerControl.SetActive(true);
                if (backgroundDim) backgroundDim.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
           FindObjectOfType<DialogueManager>().PlayDialogue("activate_rebirth");
        }
    }
}
