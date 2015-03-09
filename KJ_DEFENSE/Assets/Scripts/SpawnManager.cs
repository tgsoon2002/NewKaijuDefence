using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour 
{
    //Declaring data members
    
    //--Private
    private Vector3 target;
    private float smoothing;
    private float initialPosition;
    private GameObject spawnedUnit;
	private bool atPosition;

    //--Public

	public Transform spawnLocation;


    //Setter and Getter for target
    public Vector3 Target
    {
        get { return target; }

        set
        {
            target = value;
        }
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
    
    //Method - Start
    //Parameters: n/a
    //Purpose:
    /*  
        Initializes this class' data members.
    */
    void Start()
    {
        //Initializing data members
        smoothing = 7.0f;
		atPosition = false;
    }
    
    //Method -- SpawnUnit
    //Parameters: Unit_Type, Transform
    //Purpose:
    /*  
        This method will instantiate a unit to the screen.
        What kind of unit it will instantiate will be based
        on the value that was passed in this method's argument.
        Also takes on a second argument which is a Transform 
        object that is from the spawner icon. This method
        will tell where
    */
    public GameObject SpawnUnit(Unit_Type unit, float loc)
    {
        //Assigning our initial position
        //2000 is simply a placeholder value

        initialPosition = -50.0f;

		Debug.Log (initialPosition);

        switch(unit)
        {
            case Unit_Type.ARTILLERY:

			spawnedUnit = Instantiate(Resources.Load("Artillery"), 
			                          new Vector3(initialPosition, -2.1238f, 0.0f), gameObject.transform.rotation) as GameObject;  

                break;

            case Unit_Type.INFANTRY:

                //Do shit here
                break;

            /* Default case makes it spawn Tanks instead */
            default:
                
                //Instantiation happens here
                spawnedUnit = Instantiate(Resources.Load("TankObj-A"), 
			                          new Vector3(initialPosition, -2.1238f, 0.0f), gameObject.transform.rotation) as GameObject;  

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
		//target = target + new Vector3(spawnLocation.position.x, 0.0f, 0.0f);
		Vector3 start;
		bool isRunning = true;

		while(isRunning == true)
		{
			start = new Vector3(obj.transform.position.x, -2.1238f, 0.0f);
			obj.transform.position = Vector3.MoveTowards(start, target, Time.deltaTime * 10.0f);
			Debug.Log("New Tank's Position: " + obj.transform.position);

			if(obj.transform.position == target)
			{
				isRunning = false;
				atPosition = true;
			}

			yield return null;

		}
    }
}