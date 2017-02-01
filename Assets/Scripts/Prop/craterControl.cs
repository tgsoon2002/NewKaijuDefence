using UnityEngine;
using System.Collections;

public class craterControl : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D bullet) 
	{
		if(bullet.CompareTag ("EnemyLeg") || bullet.CompareTag ("Bullet"))
		{	
			//bullet.GetComponent<KaijuVunerablePart>().gotHit(bulletDmg);
			//Instantiate(Resources.Load("Explosion"),this.transform.position,this.transform.rotation)	;
			Destroy (this.gameObject, 0.0f);
		}//Obstacle

	}
}
