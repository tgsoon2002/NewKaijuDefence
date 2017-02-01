using UnityEngine;
using System.Collections;

public class Done_BGScroller : MonoBehaviour
{
	public float scrollSpeed;
	public float tileSizeZ;
	float xPosition;
	float yPosition;
	public GameObject MainCamera;
	private Vector3 startPosition;


	void Start ()
	{
		startPosition = transform.position;
	}

	void Update ()
	{
		xPosition = MainCamera.transform.position.x /5 *4;
		yPosition = MainCamera.transform.position.y +7f;
		startPosition.x = xPosition;
		startPosition.y = yPosition;
		transform.position = startPosition;
		//float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeZ);
		//transform.position = startPosition + Vector3.right * newPosition;
	}
}