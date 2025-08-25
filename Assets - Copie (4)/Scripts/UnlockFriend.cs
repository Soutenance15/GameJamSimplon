using UnityEngine;

public class UnlockFriend : MonoBehaviour
{
    public KeyCode interactionKey = KeyCode.E;
    public float interactionDistance = 1f;
    private GameObject player;
    public GameObject Friend;
    public GameObject FriendOnFloor;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool isPlayerNear = distance <= interactionDistance;

        if (isPlayerNear)
        {
            ActivateRebirth();
            Debug.Log("le joueur est proche");
        }
    }
    void ActivateRebirth()
    {
        if (Input.GetKey(interactionKey))
        {
            Friend.SetActive(true);
            FriendOnFloor.SetActive(false);  
        }

    }
}
