using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking.NetworkSystem;

public class GameManager : MonoBehaviour
{
    #region Data Members

    public GameObject focusedUnit;
    public Transform SpawnLocation;
    public List<GameObject> listOfUnits = new List<GameObject>();
    private int unitFocused = 0;

    #endregion

    #region Setters & Getters

    public List<GameObject> ListOfUnit
    {
        get{ return  listOfUnits; }
    }

    public int FocusUnitIndex
    {
        get{ return  unitFocused; }
        set
        {
            unitFocused = value;
            unitFocused %= listOfUnits.Count;
            focusedUnit = listOfUnits[unitFocused];
        }
    }

    public GameObject FocusedUnit
    {
        get{ return  focusedUnit; }
    }

    #endregion

    #region Built-in Unity Methods

    void Awake()
    {
        listOfUnits.Add(focusedUnit);
    }

    #endregion

    #region Public Methods

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

    #endregion

    #region Private Methods (Empty)

    #endregion

}