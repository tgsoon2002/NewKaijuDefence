using UnityEngine;
using System.Collections;

public class KaijuVunerablePart : MonoBehaviour {

	// Use this for initialization
	public GameObject kaijuPart;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void gotHit(int damage)
	{
		kaijuPart.GetComponent<Kaiju_Health>().KaijuGetDmg();//damage
	}
}
