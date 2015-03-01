using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
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

	Animator anim;

	//Public members:
	//public Texture playerHealthTexture; //Player Life
	//public float screenPosX; //Controls screen position of texture
	//public float screenPosY; //...
	//public int iconSizeX = 10; //Controls icon size on screen
	//public int iconSizeY = 10; //...
	public int unitHealth = 10; //Starting armor
	public int unitMaxHealth = 10;
	private GameManager gameManager;
	private CameraManager cameraManager;
	public float objSpeed;
	private Vector3 position;
	private Quaternion rotation;

	public List<AudioClip> listAudioClip ;

	//this group of value use for cool down
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
		if (Input.GetKeyUp(KeyCode.Space)) {
			anim.SetBool ("Shooting",false);
				}

		timer += Time.deltaTime; // record time since the last trigger
		position = gameObject.transform.position;

		if (this.unitHealth <= 0) 
		{

			gameManager.RemoveUnitFromList(this.gameObject);
			gameManager.CycleUnits();
			if (gameManager.listOfUnits.Count != 0){
				gameManager.SetFocusedUnit();
				cameraManager.UnitFocus(gameManager.GetFocusedUnit().GetComponent<Unit>());
			}
			Destroy(this.gameObject,0.0f);
			//.getAllUnit();

		}
		if (this.unitHealth <= unitMaxHealth/3) {
			//this.gameObject.GetComponent<particleSystem>.enabled = true;
				}

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
	// change the angle of cannon with parameter
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
			Debug.Log(move);
		} 
	
	}	
	//=====================================================================
	// collider, check if it hit the damage 
	void OnTriggerStay2D(Collider2D other )
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
		anim.SetBool ("Shooting",true);
		gameObject.audio.Play();
	}
}
