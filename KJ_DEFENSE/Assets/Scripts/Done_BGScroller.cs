using UnityEngine;
using System.Collections;

public class Done_BGScroller : MonoBehaviour
{
	public float scrollSpeed;
	public float tileSizeZ;
	float xposition;
	public GameObject squad;
	private Vector3 startPosition;


	void Start ()
	{
		startPosition = transform.position;
	}

	void Update ()
	{
		xposition =squad.transform.position.x /5 *4;
		startPosition.x = xposition;
		transform.position = startPosition;
		//float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
		//transform.position = startPosition + Vector3.right * newPosition;
	}
}