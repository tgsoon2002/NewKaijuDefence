using UnityEngine;
using System.Collections;

public class AttackComponent : MonoBehaviour {
	//InterEnvi
	// Use this for initialization
	//=======================================
	void OnTriggerEnter2D(Collider2D floor) 
	{
		if(floor.CompareTag ("InterEnvi"))
		{	
			//floor.GetComponent<KaijuVunerablePart>().gotHit(bulletDmg);
			Instantiate(Resources.Load("CraterObstacle"),this.transform.position,Quaternion.identity)	;
		}
	}
}
