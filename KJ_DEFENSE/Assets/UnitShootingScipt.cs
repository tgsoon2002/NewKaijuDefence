using UnityEngine;
using System.Collections;

public class UnitShootingScipt : MonoBehaviour {

	ParticleSystem gunmuzzle;
	// Use this for initialization
	void Start () {
		gunmuzzle = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		//if (Input.GetButtonDown ("Shoot") && timerShot >= timeBetweenShot) {

		//}
	}
	public void flash(){
		gunmuzzle.Stop();
		gunmuzzle.Play ();
	}

}
