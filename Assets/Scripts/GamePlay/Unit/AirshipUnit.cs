using UnityEngine;
using System.Collections;

public class AirshipUnit : PlayerUnit {

	#region member
	public ParticleSystem muzzle;
	public Transform Turret;
	public GameObject barrelEnd;
	public Transform healthBar;
	public float fireRateTimer = 0;
	public Animator unitAnimator;
	public GameObject TempBullet;
	public GameObject particleEffect;
	#endregion
	
	#region built-in Method
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.CompareTag("Danger"))
		{
			Debug.Log("Unit Take damage");
			base.TakeDamage(1);
			healthBar.localScale = new Vector3(PercentHealth(),1.0f,1.0f);
		}
		if (other.CompareTag("GroundLevel")) {
			Debug.Log("Hit the ground");
			GameObject bullet = Instantiate(particleEffect,transform.position,Quaternion.identity) as GameObject;
			Destroy (this.gameObject,1.0f);
		}
	}
	
	protected override void Start(){
		base.Start();
		Unit_AngleAimLimit = 0.0f;
		base.Unit_Muzzle = muzzle;
		base.Unit_Animator = unitAnimator;
		base.Unit_ObjectPhysics = gameObject.GetComponent<Rigidbody2D>();
		
	}
	
	// Update is called once per frame
	void Update () {
		fireRateTimer += Time.deltaTime;
	}
	#endregion
	
	#region Main Method
	
	public override void Move(float direction){
		base.Move(direction);
	}
	
	public override void Shot(){
		if (fireRateTimer >= base.Unit_FireRate) {
			base.Shot();
			GameObject bullet = Instantiate(TempBullet, barrelEnd.transform.position,barrelEnd.transform.rotation) as GameObject;
			bullet.GetComponent<Rigidbody>().AddForce(190.0f * barrelEnd.transform.forward);
			fireRateTimer = 0;
		}
		
		
	}
	
	public override void Aim(int direction){
		Unit_CurentAngle = Turret.eulerAngles.x;
		if (
			(direction >0 && Unit_CurentAngle <= 25.0f)||
			(direction <0 && Unit_CurentAngle >= 1.0f))
		{
			Turret.Rotate(Vector3.right * direction);
		}	
	}
	
	public override void KillThisUnit(){

		this.GetComponent<ParticleSystem>().Play();
		this.GetComponent<Rigidbody2D>().gravityScale = 0.05f;
		base.KillThisUnit();
	}


	#endregion
}
