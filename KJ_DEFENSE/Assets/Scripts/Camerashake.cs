using UnityEngine;
using System.Collections;

public class Camerashake : MonoBehaviour {

	// Transform of the camera to shake. Grabs the gameObject's transform
	// if null.
	public Transform camTransform;
	
	// How long the object should shake for.
	public float shake = 0;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakeAmount = 0.7f;
	public float decreaseFactor = 1.0f;
	public Vector3 originalPos;
	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}
	
	public void shakeCamera()
	{
		originalPos = camTransform.localPosition;
		shake = 1.5f;
	}
	
	void Update()
	{
		if (shake > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount; // put camera position differently to make it shake			
			shake -= Time.deltaTime * decreaseFactor; // reduce time it shake
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			shakeCamera();
			Debug.Log("shaking");
		}
	}


}