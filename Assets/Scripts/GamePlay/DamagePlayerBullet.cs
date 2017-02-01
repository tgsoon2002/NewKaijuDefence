using UnityEngine;
using System.Collections;

public class DamagePlayerBullet : MonoBehaviour {

//	private IUnitBehavior targetInterface;
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
	void OnTriggerEnter(Collider target) 
	{
		if(target.CompareTag ( "Tank"))
		{	
			//targetInterface = target.gameObject.GetComponent(typeof (IUnitBehavior)) as IUnitBehavior;
			//targetInterface.TakeDamage (2);
			Instantiate(Resources.Load("Explosion"),this.transform.position,this.transform.rotation)	;
			this.destroy();
		}
	}
}
