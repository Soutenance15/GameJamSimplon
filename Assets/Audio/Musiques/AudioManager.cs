using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public AudioClip[] playlist;
	public AudioSource audioSource;
	private int musicIndex = 0;
	public bool nextMusic = false;
<<<<<<< Updated upstream
	public GameObject player ;
	public GameObject zoneTeleport ;
	public GameObject background1 ;
	public GameObject background2 ;
	
    void Start()
    {
	
		audioSource.clip = playlist[0];
		audioSource.Play();
        
    }
=======
	public GameObject player;
	public GameObject zoneTeleport;
	public GameObject background1;
	public GameObject background2;
	private static AudioManager instance;

	int ActualSound;
	public GameObject boss1;
	public GameObject boss2;
	// public GameObject boss3 ;
>>>>>>> Stashed changes

	public static AudioManager Instance
	{
		get { return instance; }
	}
<<<<<<< Updated upstream
	void OnCollisionEnter2D(Collision2D collision)
    {
		print(collision.gameObject.tag);
		//PlayNextSong();
        if (collision.gameObject.CompareTag("MusicTrigger"))
        {
            // Contact principal
			if (!nextMusic){
=======

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this.gameObject);
			return;
		}

		instance = this;
		//DontDestroyOnLoad(this.gameObject); 
	}
		void Start()
		{

			audioSource.clip= playlist[0];
			audioSource.Play();

		}

		void Update()
		{
			if (!audioSource.isPlaying)
			{
>>>>>>> Stashed changes
				PlayNextSong();
			}
<<<<<<< Updated upstream
        }
		if (collision.gameObject.CompareTag("TeleportTrigger"))
        {
            // Contact principal
			player.transform.position = zoneTeleport.transform.position;
			PlayNextSong();
			background1.SetActive(false);
			background2.SetActive(true);
        }
=======
		}

		public void PlayNextSong()
		{
		EndSound();
		audioSource.Stop();
		ActualSound++;
		audioSource.clip = playlist[ActualSound];

		StartSound();

			audioSource.Play();
		}
	public void EndSound()
	{
		audioSource.volume = Mathf.Lerp(1, 0, 1);
>>>>>>> Stashed changes
	}
	public void StartSound()
	{
		audioSource.volume = Mathf.Lerp(0, 1, 1);
}

		void OnCollisionEnter2D(Collision2D collision)
		{
			print(collision.gameObject.tag);
			//PlayNextSong();
			if (collision.gameObject.CompareTag("MusicTrigger"))
			{
				// Contact principal
				if (!nextMusic)
				{
					PlayNextSong();
					nextMusic = true;
				}
			}
			else if (collision.gameObject.CompareTag("TeleportTrigger"))
			{
				// Contact principal
				player.transform.position = zoneTeleport.transform.position;
				PlayNextSong();
				background1.SetActive(false);
				background2.SetActive(true);
			}
			else if (collision.gameObject.CompareTag("BossTrigger"))
			{
				// Contact principal
				PlayNextSong();
				boss1.SetActive(true);
				boss2.SetActive(true);
				// boss3.SetActive(true);
			}
			// else if (collision.gameObject.CompareTag("TutoEndTriggerTP"))
			// {
			// 	// Contact principal
			// 	SceneManager.LoadScene("Niveau de départ");
			// }

		}
}
