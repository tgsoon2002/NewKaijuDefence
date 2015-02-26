using UnityEngine;
using System.Collections;

public class DamagePlayerBullet : MonoBehaviour {

	public float lifetime;
	// Use this for initialization
	void Start () 
	{
		Destroy (this.gameObject, lifetime);
	}
	public void destroy()
	{
		Destroy (this.gameObject, 0.0f);
		
	}
	//=====================================================================
	// even when bullet hit the head.
	void OnTriggerEnter2D(Collider2D bullet) 
	{
		if(bullet.CompareTag ( "Tank"))
		{	
			bullet.GetComponent<Unit>().takeDamage();
			Instantiate(Resources.Load("Explosion"),this.transform.position,this.transform.rotation)	;
			this.destroy();
		}
	}
}
