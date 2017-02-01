using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewWorldMapManager : MonoBehaviour
{
    public RaycastHit selectedSquad;

    //public List<CityOrTown> CityLocation;
    //public CityOrTown CurrentTown;
    public bool squadSelected = false;
    public CityNode headQuarter;
    private int maxSquad = 3;
    private Squad squadObject;
    public Vector3[] cityLocation;
    private RaycastHit hit;
    private Ray ray;
    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
		
            if (!squadSelected && headQuarter != null)
            {
                SelectSquad();
                Debug.Log("select squad");
            }
            else if (headQuarter != null)
            {
                MoveSelectedSquad();
                Debug.Log("move squad");
            }
            else
            {

                SelectHeadQuater();
            }
        }
        if (Input.GetMouseButtonUp(1) && squadSelected)
        {
            squadSelected = false;
            squadObject = null;
        }
        else if (Input.GetMouseButtonUp(1) && !squadSelected)
        {
            Damagecity();
        }

    }

    //test damage city
    void Damagecity()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City")
            {
                hit.collider.gameObject.GetComponent<CityNode>().LostHealth(0.1f);
            }
        }
    }

    void SelectHeadQuater()
    {
        //Debug.Log("selecting HQ");
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 50.0f, Color.green);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City")
            {
                SetHeadQuater(hit.collider.gameObject.GetComponent<CityNode>());
            }
        }
    }

    void SetHeadQuater(CityNode HQ)
    {
        headQuarter = HQ;
    }

    void SelectSquad()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 50.0f, Color.green);

        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "Unit")
            {
                squadObject = hit.collider.gameObject.GetComponent<Squad>();
                squadSelected = true;
                if (squadObject.GetComponent<Squad>().origin == null)
                {
                    squadObject.GetComponent<Squad>().origin = headQuarter;
                    //Debug.Log("Set HeadQuarter");
                }
            }
            if (hit.collider.gameObject.tag == "City")
            {
                squadSelected = false;
            }
        }
        else
        {
            squadSelected = false;
        }
    }

    void MoveSelectedSquad()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City" && squadObject.GetComponent<Squad>().origin != null)
            {
                if (squadObject.GetComponent<Squad>().origin.NearbyTown(hit.collider.GetComponent<CityNode>()))
                {
                    squadObject.MoveSquad(hit.collider.GetComponent<CityNode>());
                }	
            }
        }
    }

    public void CreateSquad()
    {
        for (int i = 0; i < maxSquad; i++)
        {
            if (!SquadManager.instance.CheckSquadIDExist(i) && headQuarter != null)
            {
                GameObject newSquad = Instantiate(Resources.Load("WorldMap/PlayerSquad"), headQuarter.transform.position, Quaternion.identity) as GameObject;	
                SquadManager.instance.newSquadID = i;
                newSquad.GetComponent<Squad>().SetSquadID(i);
                DontDestroyOnLoad(newSquad);
                i = maxSquad;
            }
        }
    }

    public void TriggerEevent()
    {
        EventManager.TriggerEvent("EndScene");
    }
}
