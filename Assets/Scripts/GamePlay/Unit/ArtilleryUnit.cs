using UnityEngine;
using System.Collections;

public class ArtilleryUnit : PlayerUnit {

	#region member
	public ParticleSystem muzzle;
	public Transform Turret;
	public GameObject barrelEnd;
	public Transform healthBar;
	public float fireRateTimer = 0;
	public Animator unitAnimator;
	private bool isStation;
	private bool isAiming;
	private float baseAngel;
	public GameObject TempBullet;
	#endregion

	#region getter setter
	public bool Artillery_IsStation{
		get { return isStation;}
		set { isStation = value;}
	}

	#endregion

	#region built-in Method

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Danger") && other.CompareTag("EnemyLeg"))
		{
			//     testing 
			//    make unit take damage by force for 1 damage.
			//     add some code later will retrive damage base 
			//     on type of attack that unit colli with
			base.TakeDamage(1);
			healthBar.localScale = new Vector3(PercentHealth(),1.0f,1.0f);
		}
	}

	protected override void Start(){
		baseAngel = Turret.eulerAngles.z;
		isAiming = false;
		isStation = false;
		base.Start();
		Unit_AngleAimLimit = 359.0f-60.0f;
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
		if (isStation == false ) // unit ready to move
		{
			base.Move(direction);
		}
		else if(isStation == true && direction != 0)  // unit not ready to move
		{
			unitAnimator.SetBool("Prepare", false);
			unitAnimator.SetBool("Shooting", false);

			if(Unit_CurentAngle <= 359.0f)
			{
				StartCoroutine(ResetBarrel());
			}
		}
}

	public override void Shot(){
		if (isStation == false) {
			//unitAnimator.SetBool("Shooting", true);
			unitAnimator.SetBool("Prepare", true);
			StartCoroutine(SetBarrel());
		}
		else 
		{
			if (fireRateTimer >= base.Unit_FireRate) {
				base.Shot();
				GameObject bullet = Instantiate(TempBullet, barrelEnd.transform.position,barrelEnd.transform.rotation) as GameObject;
				bullet.GetComponent<Rigidbody>().AddForce(190.0f * barrelEnd.transform.forward);
				fireRateTimer = 0;
			}
		}

	}

	public override void Aim(int direction){
		Unit_CurentAngle = Turret.eulerAngles.z;
		if (isStation == true &&(
			(direction >=0 && Unit_CurentAngle - baseAngel <= 359.0f)||
			(direction <=0 && Unit_CurentAngle  - baseAngel >= Unit_AngleAimLimit)))
		{
			isAiming = true;
			Turret.Rotate(Vector3.forward * direction);
		}	
	}

	#endregion
	#region HelperMethod
	IEnumerator ResetBarrel()
	{
		//While the angle of the barrel is NOT zero or whatever
		//So change this
		while(Unit_CurentAngle <= 359.0f)
		{
			//Do some shit here that will lower the angle
			Aim(1);

			if(Unit_CurentAngle <= 359.0f)
			{
				isAiming = false;
			}

			//This will be the return address on every iteration of
			//the loop. 
			//Since these gameobjects will have a Physics component
			//to it, FixedUpdate is better
			yield return new WaitForFixedUpdate();
		}
	}

	IEnumerator SetBarrel()
	{
		//While the angle of the barrel is NOT zero or whatever
		//So change this
		while(Unit_CurentAngle >= 330.0f)
		{
			//Do some shit here that will lower the angle
			Aim(-1);
			
			if(Unit_CurentAngle >= 330.0f)
			{
				isAiming = true;
			}
			yield return new WaitForFixedUpdate();
		}
	}


	#endregion
}
