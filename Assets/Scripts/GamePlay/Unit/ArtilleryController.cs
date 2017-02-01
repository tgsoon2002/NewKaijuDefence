using UnityEngine;
using System.Collections;

public class ArtilleryController : MonoBehaviour {
	public ArtilleryUnit parent ;

	public void PrepareDone(){

		parent.Artillery_IsStation = !parent.Artillery_IsStation;
		//EventManager.TriggerEvent("PrepareStatus");
	}


}
