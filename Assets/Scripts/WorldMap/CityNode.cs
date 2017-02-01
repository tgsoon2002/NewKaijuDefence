using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CityNode : MonoBehaviour {

	public int cityID;
	public string cityName;
	public float healthPercent = 1;
	public TextMesh cityNameMesh;
	public Transform healthBar;
	public List<int> nearbyTownList;
	//public Transform 	 thisTransform;


	void OnEnable()
	{
		EventManager.StartListening("EndScene", PersistData);
	}

	void OnDisable()
	{
		EventManager.StopListening("EndScene", PersistData);
	}
	
	public void LostHealth(float damage){ 
		healthPercent -= damage;
		healthBar.localScale = new Vector3(healthPercent,1,1);
	}

	public bool NearbyTown(CityNode search)
	{
		//return nearbyTownList.Exists(city => city == search);
		return false;
	}

	public void UpdateInfo()
	{
		cityNameMesh.text = cityName;
		healthBar.localScale = new Vector3(healthPercent,1,1);
	}

	void PersistData()
	{
		MapController.instance.CityInfo(this.gameObject);
	}
	void SetNearbyTown(List<int> nearByTown){
		nearbyTownList = nearByTown;
	}
}
