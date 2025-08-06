using UnityEngine;

public class Parallax : MonoBehaviour {
	
	private float length, startpos;
	public GameObject cam;
	public float parallaxEffect;
	public float displacement;
	
	void Start () {
		startpos = transform.position.x;
		length = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	void FixedUpdate () {
		float temp = cam.transform.position.x * (1 - parallaxEffect);
		float dist = cam.transform.position.x * parallaxEffect;

		displacement = Mathf.Lerp(startpos, startpos + dist, Time.time);
		transform.position = new Vector3(displacement, transform.position.y, transform.position.z);
		
		if (temp > startpos + length) startpos += length;
		else if (temp < startpos - length) startpos -= length;
	}
}