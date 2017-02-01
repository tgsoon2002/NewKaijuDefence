using UnityEngine;
using System.Collections;

public class TankUnit : PlayerUnit {

	#region member
	public ParticleSystem muzzle;
	public Transform Turret;
	public GameObject barrelEnd;
	public Transform healthBar;
	public float fireRateTimer = 0;
	public Animator unitAnimator;
	public GameObject TempBullet;
	private float baseAngel;
	
	#endregion

	#region built-in Method

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Danger") && other.CompareTag("EnemyLeg"))
		{
			base.TakeDamage(1);
			healthBar.localScale = new Vector3(PercentHealth(),1.0f,1.0f);
		}

		if(other.CompareTag("OutOfBound"))
	    {
			this.TakeDamage (base.Unit_Max_HP);
		}
	}
	
	protected override void Start(){
		base.Start();
		baseAngel = Turret.eulerAngles.z;
		Unit_AngleAimLimit = 359-35.0f;
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
				bullet.GetComponent<Rigidbody2D>().AddForce(300.0f * barrelEnd.transform.forward);
				fireRateTimer = 0;
			}

		
	}
	
	public override void Aim(int direction){
		Unit_CurentAngle = Turret.eulerAngles.z;
		if (
			(direction >=0 && (Unit_CurentAngle - baseAngel) <= 30.0f)||
			(direction <=0 && (Unit_CurentAngle - baseAngel) >= 1.0f))
		{
			Turret.Rotate(Vector3.forward * direction);
		}	
	}


	#endregion
}
