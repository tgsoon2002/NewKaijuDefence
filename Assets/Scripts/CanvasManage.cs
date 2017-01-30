using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManage : MonoBehaviour {
	
	public Slider Slider;
	public Kaiju_Health monster;
	// Use this for initialization
	void Start () {
		Slider.maxValue = monster.maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		Slider.value = monster.returnHealth ();
		//handleHealth ();
	}
}
