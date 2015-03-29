using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{
    //Declaring data members
    
    //--Private
    private Vector3 target;
    private float initialPosition;
    private GameObject spawnedUnit;
	private bool atPosition = false;
	Vector3 start;
    //--Public
	bool isRunning = false;
	public Transform spawnLocation;


    //Setter and Getter for target
    public Vector3 Target
    {
        get { return target; }
        set { target = value; }
    }

    //Setter and Getter for spawedUnit
    public GameObject SpawnedUnit
    {
        get { return spawnedUnit; }
    }


	//Setter and Getter for atPosition
	public bool unitAtLocation
	{
		get { return atPosition; }
		set { atPosition = value; }
	}

    //--Implemented Methods
    
    
    //================Method -- SpawnUnit=============================
    public GameObject SpawnUnit(Unit_Type unit)
    {
    
        switch(unit)
        {
		case Unit_Type.TANK:
			spawnedUnit = Instantiate(Resources.Load("TankObj-A"), 
			new Vector3(spawnLocation.transform.position.x, spawnLocation.transform.position.y, 0.0f), 
			gameObject.transform.rotation) as GameObject;  
			break;
		case Unit_Type.ARTILLERY:
				spawnedUnit = Instantiate(Resources.Load("Artillery"), 
				new Vector3(spawnLocation.transform.position.x, spawnLocation.transform.position.y, 0.0f),
			    gameObject.transform.rotation) as GameObject;  
                break;

        case Unit_Type.INFANTRY:

                //Do shit here
                break;

            default: 
                break;
        }

		return spawnedUnit;
    }

	//Method -- CoroutineForMove
	//Parameters: Vector3, GameObject
	/*
	  	This method will start the couroutine: MoveToLocation
	*/
	public void CoroutineForMove(Vector3 tag, GameObject o)
	{
		isRunning = true;
		StartCoroutine(MoveToLocation(tag, o));
		StopCoroutine("MoveToLocation");



	}

    //Method -- MoveToLocation
    //Parameters: Vector3, GameObject 
    //Purpose
    /* 
        This method is a coroutine that will be called when the user has
        decided where to spawn a unit object on the screen. It will take
        a Vector3 and a GameObjects as its arguments to then have that object
        move towards that vector's coordinates.
    */
    IEnumerator MoveToLocation(Vector3 target, GameObject obj)
    {

		while(isRunning == true)
		{
			start = new Vector3(obj.transform.position.x, spawnLocation.transform.position.y, 0.0f);
			obj.transform.position = Vector3.MoveTowards(start, target, Time.deltaTime * 10.0f);
			Debug.Log("still couroutine");
			//Debug.Log("New Tank's Position: " + obj.transform.position);
			Debug.Log(obj.transform.position.x - target.x);
			if(Mathf.Abs(obj.transform.position.x - target.x) <= 0.005f )
			{
				isRunning = false;
				atPosition = true;
				Debug.Log("At destination");

			}

		yield return null;

		}
    }
}