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
	private bool onCD;
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

		if (currentHealth>0) {
		//	KaijuPosition.x += -1.0f * kaijuSpeed * Time.deltaTime;;
		//	this.transform.position = KaijuPosition;
			moving(-1.0f);
		}
		if (currentHealth == (maxHealth/2)+1)//i dont know why i must be +1 to give the right value.
			playSound(0);
		else if (currentHealth == 0){
			anim.SetTrigger ("Die");
			playSound(1);
		}
		if (currentHealth == 0) {
			dieCD+= Time.deltaTime;
				}
		if (currentHealth == 0 && dieCD >= 1f) {
			anim.SetTrigger ("StayDead");
			Destroy (this.gameObject, 4.0f);

				}

	}

	//=======================Call when kaiju got hit=======================
	public void KaijuGetDmg(){
	
		if (!onCD && currentHealth>0) {
			StartCoroutine(coolDownDmg());
			currentHealth--;
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
		audio.clip = listAudioClip[clip];
		audio.Play();
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
