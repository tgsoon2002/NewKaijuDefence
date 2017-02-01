using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class MixLevels : MonoBehaviour {
	
	public AudioMixer masterMixer;
	public GameObject mainEnemy;
	public GameObject leftEndPoint;
	//public AudioSource hpguitar;
	//public AudioSource timpani;
	public GameObject location84percent;
	public float distanceTillDoom;
	private float maxDistanceTillDoom;
	
	public float currentDistance84;
	public float Distance84Percent;
	void Start()
	{
		//enemy = CombatSceneManager.instance.mainEnemy;
		maxDistanceTillDoom = Mathf.Abs(mainEnemy.transform.position.x - leftEndPoint.transform.position.x) + 0.00001f;
		
	}
	void Update(){
		//Gets distance from monster to gameover box
		if (!CombatSceneManager.instance.winOrLose) {
			distanceTillDoom = Mathf.Abs(mainEnemy.transform.position.x - leftEndPoint.transform.position.x) + 0.00001f ;	
		}

		//Sets volume of each parameter according to distance
		
		
		SetBassLvl (Mathf.Min(0,5*100-(6*100*Mathf.Pow(distanceTillDoom/maxDistanceTillDoom,2))));
		SetPercLvl (Mathf.Min(-8,4.5f*100-(6*100*Mathf.Pow(distanceTillDoom/maxDistanceTillDoom,2))));
		SetHPGuitarLvl (Mathf.Min(-3,4*100-(6*100*Mathf.Pow(distanceTillDoom/maxDistanceTillDoom,2))));
		SetStringsLvl (Mathf.Min(0,3.5f*100-(6*100*Mathf.Pow(distanceTillDoom/maxDistanceTillDoom,2))));
		SetChoirLvl (Mathf.Min(0,3*100-(6*100*Mathf.Pow(distanceTillDoom/maxDistanceTillDoom,2))));
		
		
		
	}
	// Sets volume levels of each parameter
	public void SetMasterLvl(float mastLvl) {
		masterMixer.SetFloat ("lvone_mastervol", (mastLvl));
		
	}
	
	public void SetTimpaniLvl(float timLvl) {
		masterMixer.SetFloat ("lvone_timpanivol", timLvl);
	}
	
	public void SetBassLvl(float bassLvl) {
		masterMixer.SetFloat ("lvone_bassvol", bassLvl);
	}
	
	public void SetPercLvl(float percLvl) {
		masterMixer.SetFloat ("lvone_percvol", percLvl);
	}
	
	public void SetHPGuitarLvl(float hpguitarLvl) {
		masterMixer.SetFloat ("lvone_hpguitarvol", hpguitarLvl);
	}
	
	public void SetStringsLvl(float strLvl) {
		masterMixer.SetFloat ("lvone_stringvol", strLvl);
	}
	
	public void SetChoirLvl(float choirLvl) {
		masterMixer.SetFloat ("lvone_choirvol", choirLvl);
	}
}
