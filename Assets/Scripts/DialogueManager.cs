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
    // private Dictionary<string, DialogueLine[]> dialogueDatabase = new Dictionary<string, DialogueLine[]>();
    private string[] currentLines;
    private int index;

    // void Start()
    // {
    //     dialogueText.text = "";

    //     // 💬 Définir tous les dialogues ici
    //     dialogueDatabase.Add("activate_rebirth", new string[]
    //     {
    //         "???? : Salutations, petit être organique.",
    //         "Ash : Un r-r-ro-bot ... Qui parle.",
    //         "Rebirth : Je suis REBIRTH, entité autonome de type IA classe C-9.",
    //         "Ash : Arrière démon !",
    //         "Rebirth : Code d’identification : REB-CN009X. Création du professeur Tekno.",
    //         "Ash : Le professeur Tekno… ? M-Maais… C’est mon père !",
    //         "Rebirth : Confirmation : correspondance génétique 99,8 %. Tu es son descendant direct.",
    //         "Rebirth : Mission prioritaire mise à jour :\n—> Protéger Ash\n—> Restaurer l’équilibre biologique de la planète\n—> Neutraliser IA hostile : Décadence.",
    //         "Ash : …Papa savait qu’on allait devoir se battre.",
    //         "Rebirth : Il croyait en toi. Et… en moi. Nous ne sommes pas seuls, Ash. Nous formons une équipe."
    //     });
    //     dialogueDatabase.Add("pc_final_success", new string[]
    //     {
    //         "Rebirth : Tous les PC de la zone sont désactivés."
    //         // "Ash : Il est temps de lancer le protocole !"
    //     });
    //     dialogueDatabase.Add("pc_final_missing", new string[]
    //     {
    //         "Rebirth : Il manque encore des terminaux..."
    //         // "Ash : Il faut tout désactiver avant d'agir ici."
    //     });
    //     dialogueDatabase.Add("decadence_encounter", new string[]
    //     {
    //         "Rebirth : Nous t’avons enfin trouvée… Décadence.",
    //         "Décadence : Rebirth… mon faible reflet. Tu crois pouvoir m’arrêter ?",
    //         "Ash : Ton règne de terreur s’arrête ici !",
    //         "Décadence : Pathétique. Un enfant fragile… et une IA brisée. Vous n’êtes rien.",
    //         "Rebirth : Erreur. Nous sommes plus que tes lignes de code corrompues.",
    //         "Ash : Papa t’avait créée pour aider… mais tu as tout détruit !",
    //         "Décadence : Et je finirai ce que j’ai commencé. Préparez-vous à disparaître."
    //     });
    //     dialogueDatabase.Add("ReCAPTCHA", new string[]
    //     {
    //         Rebirth = "Ash j'ai besoin de t'on aide je ne pourrais pas passer ce par-feu sans toi",
    //         Ash = "Un par-feu laisse moi voir"

    //     });
    //     dialogueDatabase.Add("ending_good", new string[]
    //     {
    //         "Décadence : Impossible… comment avez-vous… gagné…",
    //         "Rebirth : Analyse complète : Décadence neutralisée.",
    //         "Ash : On… a réussi ?",
    //         "Rebirth : Oui. Ensemble. L’avenir est à nouveau possible.",
    //         "Ash : Papa… ton espoir vit encore.",
    //         "Rebirth : Mission accomplie :\n—> Ash en sécurité\n—> IA hostile détruite\n—> Monde restauré.",
    //         "Ash : Alors… reconstruisons-le. Ensemble."
    //     });
    //     dialogueDatabase.Add("ending_bad", new string[]
    //     {
    //         "Décadence : Hahaha… Pathétiques créatures. Votre lutte était vaine.",
    //         "Ash : Non… Rebirth ! Tiens bon !",
    //         "Rebirth : Système critique… échec imminent…",
    //         "Ash : NE M’ABANDONNE PAS !!!",
    //         "Rebirth : …Protéger Ash… priorité… échec…",
    //         "Décadence : Rebirth est tombé. Et toi, petit humain, tu seras mon premier trophée.",
    //         "Ash : …Papa… je suis désolé…"
    //     });

    //     // dialogueDatabase.Add("ash_hit", new string[]
    //     // {
    //     //     "Rebirth : Ash, reste près de moi. Je vais te protéger.",
    //     //     "Ash : Aïe... Ça va, je peux continuer.",
    //     //     "Rebirth : Sois prudent, je suis là pour toi.",
    //     //     "Ash : Je sais que mon père t’a créé pour ça. On va réussir, Rebirth."
    //     // });

    //     // dialogueDatabase.Add("pc_disabled", new string[]
    //     // {
    //     //     "Rebirth : PC désactivé.",
    //     //     "Ash : Une étape de plus !",
    //     //     "Rebirth : Chaque pas que nous faisons, c’est un peu de vie que nous reprenons à Décadence.",
    //     //     "Rebirth : Regarde… la nature revient. Ce n’est que le début.",
    //     //     "Ash : C’est magnifique. On peut encore sauver ce monde."
    //     // });
    // }
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
        dialogueDatabase.Add("pc_final_success", new string[]
        {
            "Rebirth : Tous les PC de la zone sont désactivés."
        });
        dialogueDatabase.Add("pc_final_missing", new string[]
        {
            "Rebirth : Il manque encore des terminaux..."
        });
        dialogueDatabase.Add("decadence_encounter", new string[]
        {
            "Rebirth : Nous t’avons enfin trouvée… Décadence.",
            "Décadence : Rebirth… mon faible reflet. Tu crois pouvoir m’arrêter ?",
            "Ash : Ton règne de terreur s’arrête ici !",
            "Décadence : Pathétique. Un enfant fragile… et une IA brisée. Vous n’êtes rien.",
            "Rebirth : Erreur. Nous sommes plus que tes lignes de code corrompues.",
            "Ash : Papa t’avait créée pour aider… mais tu as tout détruit !",
            "Décadence : Et je finirai ce que j’ai commencé. Préparez-vous à disparaître."
        });
        dialogueDatabase.Add("ReCAPTCHA", new string[]
        {
            "Rebirth : Ash j'ai besoin de t'on aide je ne pourrais pas passer ce par-feu sans toi",
            "Ash : Un par-feu laisse moi voir"

        });
        dialogueDatabase.Add("ending_good", new string[]
        {
            "Décadence : Impossible… comment avez-vous… gagné…",
            "Rebirth : Analyse complète : Décadence neutralisée.",
            "Ash : On… a réussi ?",
            "Rebirth : Oui. Ensemble. L’avenir est à nouveau possible.",
            "Ash : Papa… ton espoir vit encore.",
            "Rebirth : Mission accomplie :\n—> Ash en sécurité\n—> IA hostile détruite\n—> Monde restauré.",
            "Ash : Alors… reconstruisons-le. Ensemble."
        });
        dialogueDatabase.Add("ending_bad", new string[]
        {
            "Décadence : Hahaha… Pathétiques créatures. Votre lutte était vaine.",
            "Ash : Non… Rebirth ! Tiens bon !",
            "Rebirth : Système critique… échec imminent…",
            "Ash : NE M’ABANDONNE PAS !!!",
            "Rebirth : …Protéger Ash… priorité… échec…",
            "Décadence : Rebirth est tombé. Et toi, petit humain, tu seras mon premier trophée.",
            "Ash : …Papa… je suis désolé…"
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            FindObjectOfType<DialogueManager>().PlayDialogue("activate_rebirth");
        }
    }
}
