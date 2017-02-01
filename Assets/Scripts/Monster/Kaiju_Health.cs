using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Kaiju_Health : MonoBehaviour 
{
	public RectTransform healthTransform;
	private float cachedY;
	private float minXValue;
	private float maxXValue;
	public int maxHealth;
	public  int currentHealth;
	private int CurrentHealth 
	{
		get{return currentHealth;}
		set{currentHealth =value;
			HandleHealth();}
	}
	public List<AudioClip> listAudioClip ;


	public Text healthText;
	public Image visualHealth;
	// this group of value use for cool down
	public float timerBetweenMove;
	public float coolDown;
	private bool onCD;
	IEnumerator coolDownDmg(){
		onCD = true;
		yield return new WaitForSeconds(coolDown);
		onCD = false;
	}
	//=====================================================================
	void Start () 
	{

		cachedY = healthTransform.position.y;
		maxXValue = healthTransform.position.x;
		minXValue = healthTransform.position.x - healthTransform.rect.width;
		onCD  = false;
		currentHealth = maxHealth;
		HandleHealth();
	}
	void Update(){
		if (currentHealth == (maxHealth/2)+1)
			playSound(0);
		else if (currentHealth == 1){

			playSound(1);
		}
	}

	//=====================================================================
	// Calculate healthbar position
	private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return (x-inMin)*(outMax-outMin)/ (inMax -inMin) + outMin;
		
	}

	//=====================================================================
	// Handle Health bar color and text
	private void HandleHealth()
	{
		healthText.text = "Health: " + currentHealth;
		float currentXValue = MapValues(currentHealth,0,maxHealth,minXValue,maxXValue);
		healthTransform.position = new Vector3(currentXValue,cachedY);
		if (currentHealth > maxHealth/2) 
		{
			visualHealth.color = new Color32((byte)MapValues(currentHealth,maxHealth/2,maxHealth,255,0),255,0,255);
		}
		else {
			visualHealth.color = new Color32(255,(byte)MapValues(currentHealth,0,maxHealth/2,0,255),0,255);
		}
	}
	//=====================================================================
	public void KaijuGetDmg(){
	
		if (!onCD && currentHealth>0) {
			StartCoroutine(coolDownDmg());
			CurrentHealth--;

	
		}
	}
	//=====================================================================
	// Play sound
	private void playSound(int clip)
	{
		GetComponent<AudioSource>().clip = listAudioClip[clip];
		GetComponent<AudioSource>().Play();

	}

}
