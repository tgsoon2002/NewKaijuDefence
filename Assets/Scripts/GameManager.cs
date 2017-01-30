using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour 
{
	//Declaring local variables
	public GameObject focusedUnit;
	public Transform SpawnLocation;
	public List<GameObject> queueUnits = new List<GameObject>();
	public List<GameObject> listOfUnits = new List<GameObject>();
	private int unitFocused = 0;

	void Awake()
	{
		listOfUnits.Add(focusedUnit);
	}
	//=========================================================================
//	void Start()
//	{
//		//listOfUnits = GetUnitList();
//		unitFocused = 0;
//	}
	//=========================================================================
	// add new unit to the list
	public void AppendUnitToList(GameObject newUnit)
	{
		listOfUnits.Add(newUnit);
	}

	//=========================================================================
	// search and remove unit which was destroyed list
	public void RemoveUnitFromList(GameObject newUnit)
	{
		listOfUnits.Remove(newUnit);
	}

	//=========================================================================
	// return array have list of unit
	public GameObject[] GetUnitList()
	{
		return listOfUnits.ToArray();
	}
	//=========================================================================
	// cycle throught list to select which unit to troncol
	public void CycleUnits()
	{
		if(unitFocused >= listOfUnits.Count - 1)
		{
			unitFocused = 0;
		}
		else
		{
			unitFocused++;
		}
	}
	//=========================================================================
	// add all unit to the list.
	public void getAllUnit()
	{
		foreach (Unit unit in GameObject.FindObjectsOfType(typeof(Unit))) 
		{
			listOfUnits.Add(unit.gameObject);
		}
	}

	public void UpdateList()
	{
		List<GameObject> temp = new List<GameObject>();

		foreach (Unit unit in GameObject.FindObjectsOfType(typeof(Unit))) 
		{
			temp.Add(unit.gameObject);
		}

	}

	//return unit to control
	public GameObject GetFocusedUnit()
	{
		return focusedUnit;
	}
	// change unit being focus
	public void SetFocusedUnit()
	{
		focusedUnit = listOfUnits[unitFocused];
	}
	//Reference
	//Declaring local variables
	//List<GameObject> units = new List<GameObject>();	
	//foreach (Unit unit in GameObject.FindObjectsOfType(typeof(Unit))) 
	//{
	//units.Add(unit.gameObject);
	//}
	//Debug.Log("unitFocused value is: " + unitFocused);
	//Debug.Log (units.Count);
}