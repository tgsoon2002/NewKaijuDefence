using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Kaiju_Health : MonoBehaviour 
{
	// Action
	public Vector3 KaijuPosition;
	public float kaijuSpeed;
	// health
	public int maxHealth;   
	public  int currentHealth;

	// audio
	public List<AudioClip> listAudioClip ;

	Animator anim;   

	// this group of value use for cool down
	public float coolDownBetweenHit;
	public float dieCD;
	public float attackCD;
	private bool onCD;
	public bool attacking = false;
	IEnumerator coolDownDmg(){
		onCD = true;
		yield return new WaitForSeconds(coolDownBetweenHit);
		onCD = false;
	}

	//=====================================================================
	void Start () 
	{
		onCD  = false;
		anim = GetComponent <Animator> ();
		currentHealth = maxHealth;
		KaijuPosition = this.transform.position;
	}
	//=================Update every frame===========
	// Check if kaiju is half health or 0 health to play sound. and play animation.
	void Update(){
		// make monster moving if still alive
		if (currentHealth > 0 && !attacking) {
			moving(-1.0f);
		}
		// play sound and set animation if monster low to certain healt
		if (currentHealth <= (maxHealth/2)+1)//i dont know why i must be +1 to give the right value.
			playSound(0);
		else if (currentHealth <= 0){
			playSound(1);
		}

		if (currentHealth<=0) {
			anim.SetTrigger ("Die");
			dieCD+= Time.deltaTime; // start count down still the body destroy
				}
		// loop stay dead animation and die afer 4 sec
		if (currentHealth <= 0 && dieCD >= 1f) {
			anim.SetTrigger ("StayDead");
			Destroy (this.gameObject, 4.0f);
				}
		// set animation attack 
		if (currentHealth >= 0 && attackCD < 5.0f) {
						attacking = false;
						attackCD += Time.deltaTime;
						anim.SetBool ("Golem Attack", false);
				} else if (currentHealth >= 0 && attackCD < 6.5f) {
						attacking = true;
						attackCD += Time.deltaTime;
						anim.SetBool ("Golem Attack", true);
				} else {
						attackCD = 0;
				}

	}

	//=======================Call when kaiju got hit=======================
	public void KaijuGetDmg(int damage){
	
		if (!onCD && currentHealth>0) {
			StartCoroutine(coolDownDmg());
			currentHealth-= damage;
		}
	}
	//===========================Public show the current health============
	public int returnHealth()
	{
		return currentHealth;
	}
	//=====================================================================
	// Call when to Play sound when low health or dead, 
	private void playSound(int clip)
	{
		GetComponent<AudioSource>().clip = listAudioClip[clip];
		GetComponent<AudioSource>().Play();
	}
	//=====================================================================
	// kaiju moving
	private void moving(float moveDirection)
	{
		KaijuPosition.x +=  moveDirection * kaijuSpeed * Time.deltaTime;;
		this.transform.position = KaijuPosition;
	
//		this.gameObject.GetComponent <Transform> ().position
	}
}
