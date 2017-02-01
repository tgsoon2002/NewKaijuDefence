using UnityEngine;
using System.Collections;
public class Camerashake : MonoBehaviour
{
	private Vector3 originPosition;
	private Quaternion originRotation;
	public float shake_decay;
	public float shake_intensity;
	private Vector3 initialPosition;
	private Quaternion initialRotation;

	public CombatSceneManager gamMan;
	
	void Start(){
		initialPosition = this.transform.position;
		initialRotation= this.transform.rotation;
		//Debug.Log("initial x and y: "+initialPosition.x+" "+ initialPosition.y);
		//Debug.Log ("initial x and y: "+initialPosition.position.x +" , "+initialPosition.position.y);
	}
	

	
	void Update (){
		if (shake_intensity > 0 ){
			Shake();
			transform.position = originPosition + Random.insideUnitSphere * shake_intensity;
			//transform.rotation = new Quaternion(
				//originRotation.x + Random.Range (-shake_intensity,shake_intensity) * .2f,
				//originRotation.y + Random.Range (-shake_intensity,shake_intensity) * .2f, 0.0f,
				//originRotation.z + Random.Range (-shake_intensity,shake_intensity) * .2f,
				//originRotation.w + Random.Range (-shake_intensity,shake_intensity) * .2f);
			shake_intensity -= shake_decay;
		}
		
		if (shake_intensity <= 0.0f)
		{
//			Debug.Log("Penis");
			//this.transform.position=initialPosition;
			//this.transform.rotation=initialRotation;
			/*this.transform.position.x = initialPosition.x;//position.x;
			this.transform.position.y = initialPosition.y;//position.y;
			this.transform.position.z = initialPosition.z;//position.z;*/
			//this.transform.rotation = initialPosition.rotation;
//			Debug.Log ("after shake position: "+this.transform.position.x+" , "+this.transform.position.y);
			//Debug.Log(initialPosition.position == this.transform.position);
		}

		//if (gamMan.makeItShake()) {
			//Shake();
				//}
	}
	
	
	public void Shake()
	{
		initialPosition = this.transform.position;
		//initialRotation= this.transform.rotation;
		originPosition = transform.position;
		//originRotation = transform.rotation;
		shake_intensity = 0.1f;
		shake_decay = 0.002f;
	}
	
	
}