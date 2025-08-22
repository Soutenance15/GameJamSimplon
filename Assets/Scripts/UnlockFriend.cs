using UnityEngine;

public class UnlockFriend : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 1f;
    public GameObject playerObj; // rename to avoid confusion
    public GameObject Friend;
    public GameObject FriendOnFloor;
    public DialogueManager dialogueManager; // ← drag & drop ton DialogueHUD ici

    private Player playerScript; // reference to your Player script

    void Start()
    {
        if (playerObj != null)
        {
            playerScript = playerObj.GetComponent<Player>();
            if (playerScript == null)
            {
                Debug.LogError("Player script not found on the assigned playerObj!");
            }
        }
    }
    void Update()
    {
        if (playerObj == null || playerScript == null) return;

        float distance = Vector2.Distance(transform.position, playerObj.transform.position);
        bool isPlayerNear = distance <= interactionDistance;

        if (isPlayerNear)
        {
            if (Input.GetKey(interactionKey))
            {
                ActivateRebirth();
                Debug.Log("Player unlocked Rebirth!");
            }
        }
    }
    void ActivateRebirth()
    {
        playerScript.disableDeployFriend = false; // The player can deploy and store friend
        if (dialogueManager != null)
        {
            dialogueManager.PlayDialogue("activate_rebirth");
        }
        else
        {
            Debug.LogWarning("DialogueManager reference missing in UnlockFriend!");
        }
        Friend.SetActive(true);
        FriendOnFloor.SetActive(false);
    }
}
