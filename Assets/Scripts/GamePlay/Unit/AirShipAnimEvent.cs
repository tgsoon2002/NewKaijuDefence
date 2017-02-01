using UnityEngine;
using System.Collections;

public class AirShipAnimEvent : MonoBehaviour {
	public AirshipUnit parent ;
	
	public void ShootDone(){
		parent.unitAnimator.SetBool("Shooting",false);
	}


}
