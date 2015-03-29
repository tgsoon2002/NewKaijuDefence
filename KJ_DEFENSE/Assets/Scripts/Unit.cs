using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//added 3.20.15 for database
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public enum Unit_Type
{
	TANK,
	ARTILLERY,
	INFANTRY
}

public class Unit : MonoBehaviour
{
	//Declaring local variables:
	public Unit_Type type;
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;

	string name;

	int Damage;
	int Armor;
	float Firerate;
	int Movement;

	Animator anim;
	string conn;
	public int unitHealth = 10;
	public int unitMaxHealth = 10;
	private GameManager gameManager;
	private CameraManager cameraManager;
	public float objSpeed;
	private Vector3 position;
	private Quaternion rotation;

	public List<AudioClip> listAudioClip;

	//this group of value use for cool down
	int bulletDmg = 3;
	public float timeBetweenAttack = 0.5f;
	private float timer;
	public float coolDownBetweenTakeHit;
	private bool onCD;
	IEnumerator coolDownDmg()
	{
		onCD = true;
		yield return new WaitForSeconds(coolDownBetweenTakeHit);
		onCD = false;
	}

	//=====================================================================
	void Start()
	{
		// set connection string and open the conenction
		conn= "URI=file:" + Application.dataPath + "KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();//open connection to db
		// set command and exercute
		dbcmd = dbconn.CreateCommand ();
		string sqlQuery = "SELECT name,Damage,Armor,Firerate,Movement FROM KJData";
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader ();//open db first
		while (reader.Read ()) 
		{
			setValue(type);	
		}
		//isFocused = true;

		anim = GetComponent<Animator>();
		position = gameObject.transform.position;
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		cameraManager = Camera.main.GetComponent<CameraManager>();

		//		rotation = this.transform.GetChild (0).transform.rotation;
	}

	//=====================================================================
	// Update every frame, make sure destroy tank if health below 0
	void Update()
	{
		// if the key was lift, the animation set to non.
		if (Input.GetKeyUp(KeyCode.Space)) {
			anim.SetBool ("Shooting", false);
		}
		timer += Time.deltaTime; // record time since the last trigger

		if (this.unitHealth <= 0)
		{
			gameManager.RemoveUnitFromList(this.gameObject);
			gameManager.CycleUnits();
			if (gameManager.listOfUnits.Count != 0) {
				gameManager.SetFocusedUnit();
				cameraManager.UnitFocus(gameManager.GetFocusedUnit().GetComponent<Unit>());
			}
			Destroy(this.gameObject, 0.0f);
		}

		//if (this.unitHealth <= unitMaxHealth/3) {
			//this.gameObject.GetComponent<particleSystem>.enabled = true;
		//}
	}

	//=====================================================================;
	//change the x cordinate of the unit with float value
	//parameter moveDirection<float>
	public void MoveFocusedUnit(float moveDirection)
	{
		position.x += moveDirection * objSpeed * Time.deltaTime;

		this.transform.position = position;
	}

	//=====================================================================
	// change the angle of cannon with parameter, only applied to tank and artillery for now
	//parameter angel<float>
	public void ChangeCannonAngle(float angle)
	{
		if (type != Unit_Type.INFANTRY) {
			this.transform.GetChild(0).transform.Rotate(Vector3.forward * angle);
		}
	}

	//=====================================================================
	// set the animation to moving mode or idle mode
	//parameter move<bool>
	public void setAnimation(bool move)
	{
		if (type == Unit_Type.TANK) {
			anim.SetBool ("Moving", move);
		}
	}

	//=====================================================================
	// collider, check if it hit the damage 
	void OnTriggerStay2D(Collider2D other)
	{
		if(other.CompareTag("Danger"))
		{
			takeDamage();
		}
	}

	//=====================================================================
	// call this function to reduce tank health.
	public void takeDamage()
	{
		if (!onCD && this.unitHealth>0) {
			StartCoroutine(coolDownDmg());
			this.unitHealth--;
		}
	}

	//=====================================================================
	// Playsound and set animation for recoil
	public void ShootSound()
	{
		anim.SetBool ("Shooting", true);
		this.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<ParticleSystem> ().Play ();

		gameObject.GetComponent<AudioSource>().Play();

	}

	/// <summary>
	/// Gets the Bullet damage.
	/// </summary>
	/// <returns>The Bullet damage.</returns>
	public int getBDamage()
	{
		return bulletDmg;
	}
	private void setValue(Unit_Type a)
	{
		string name=reader.GetString(0);

		//Debug.Log ("unit_type  " + a);
		if (a == Unit_Type.TANK && name == "Tank")
		{
			Damage= reader.GetInt32 (1);
			Armor= reader.GetInt32(2);
			Firerate = reader.GetFloat(3);
			Movement = reader.GetInt32(4);

			//testing.
			bulletDmg=Damage;
			unitMaxHealth=Armor;
			coolDownBetweenTakeHit=Firerate;
			objSpeed=Movement;
		}
		if (a == Unit_Type.ARTILLERY && name =="Artillery")
		{
			Damage= reader.GetInt32 (1);
			Armor= reader.GetInt32(2);
			Firerate = reader.GetFloat(3);
			Movement = reader.GetInt32(4);

			bulletDmg=Damage;
			unitMaxHealth=Armor;
			unitHealth=unitMaxHealth;
			coolDownBetweenTakeHit=Firerate;
			objSpeed=Movement;
		}
		if (a == Unit_Type.INFANTRY && name =="Infantry")
		{
			Damage= reader.GetInt32 (1);
			Armor= reader.GetInt32(2);
			Firerate = reader.GetFloat(3);
			Movement = reader.GetInt32(4);

			bulletDmg=Damage;
			unitMaxHealth=Armor;
			coolDownBetweenTakeHit=Firerate;
			objSpeed=Movement;
		}
	}
}
