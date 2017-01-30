using UnityEngine;
using System.Collections;

public class BulletLifetime : MonoBehaviour 
{
	public float lifetime;
	public int bulletDmg = 3;
	Vector3 prevPosition;
	Vector3 currentPosition;
	// Use this for initialization
	void Start () 
	{
		Destroy (this.gameObject, lifetime);
		prevPosition = this.transform.position;
		currentPosition = this.transform.position;
	}
	public void update()
	{
		rotateBullet ();
	}

	public void destroy()
	{
		Destroy (this.gameObject, 0.0f);
	}
	//=====================================================================
	// even when bullet hit the head.
	void OnTriggerEnter2D(Collider2D bullet) 
	{
		if(bullet.CompareTag ("KaijuVulnerable"))
		{	
			bullet.GetComponent<KaijuVunerablePart>().gotHit(bulletDmg);
			Instantiate(Resources.Load("Explosion"),this.transform.position,this.transform.rotation)	;
			this.destroy();
		}//Obstacle
		if(bullet.CompareTag ("Obstacle"))
		{	
		
			this.destroy();
		}
	}
	//===================================================
	private void rotateBullet()
	{
		currentPosition = this.transform.position;
		// calculate the derivative or angle of bullet,
		// apply angle of bullet.
		prevPosition = currentPosition;
		}
	//=================================
	public void setDamage(int newDmg)
	{
		bulletDmg = newDmg;
	}
}
