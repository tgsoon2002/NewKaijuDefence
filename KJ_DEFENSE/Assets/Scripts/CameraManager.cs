using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour 
{
	//Declaring data members

    //--Private members
    private SpawnIconBehavior spawner;
    private Vector3 newPosition;
    private float panningSpeed = 2.0f;
    private Vector3 temp;
    private float rightBound, leftBound, topBound, bottomBound;
    private Vector3 pos;
    private bool buyState;
    private bool panCam;
	private Transform focusTransform;
	//--Public members
	public float camSpeed;
	public GameObject centerBg;
    public Unit focusedUnit;
    
    //---Setter and Getter for buyState;
    public bool isBuying
    {
        get { return buyState; }
        set { buyState = value; }
    }
	
    //--Setter and Getter for isPanning;
    public bool isPanning
    {
        get { return panCam; }
        set { panCam = value; }
    }

	// Use this for super initialization
	void Awake()
	{
		//HACK
		if (centerBg == null || centerBg == null)
		{
			centerBg = GameObject.Find ("BG2");
		}
	}
			
	// Use this for initialization
	void Start () 
	{
		//Setting the camera into ortographic mode
		Camera.main.orthographic = true;
    
        //Initializing buyState to false;
        buyState = false;
        panCam = false;
            
		float widthBG = 38.4f;
		float heightBG = 10.24f;
		float vertExtent = GetComponent<Camera>().orthographicSize; 
		float horzExtent = GetComponent<Camera>().orthographicSize * Screen.width / Screen.height;
		leftBound = horzExtent - (widthBG / 2.0f);
		rightBound = (widthBG / 2.0f) - horzExtent;
		bottomBound = vertExtent - (heightBG / 2.0f);
		topBound = (heightBG / 2.0f) - vertExtent;
	}
	
	// =================Update is called once per frame=======================
	void Update () 
	{
		//Making the camera move to the focused GameObject
		if(panCam == false && buyState == false)
		{
			//Lerp the camera to new position.
			CameraFollowUnit();
		}
       	
		//If the player is in Buy mode, force the camera to follow the spawn icon instead
		if(buyState == true)
        {
            //Lerp the camera to the spawn icon
             CameraFollowSpawn();
        }

	}
	//==============================================================
	//Method -- CameraPanning
	//Parameters -- mousePosOrigin
	//Purpose -- This will make the camera pan around the world screen.
	public void CameraPanning(Vector3 mousePosOrigin)
	{

		//Transforms the mouse position from screen space to viewport space
		Vector3 mousePosNormalized = Camera.main.ScreenToViewportPoint(Input.mousePosition - mousePosOrigin);
		
		//This will be the new position of the mouse, thus will also be the camera's new position
		Vector3 mouseDestination = new Vector3(mousePosNormalized.x * panningSpeed, mousePosNormalized.y * panningSpeed, 0);

		temp = gameObject.transform.position + mouseDestination;
		temp.x = Mathf.Clamp(temp.x, leftBound, rightBound);
		temp.y = Mathf.Clamp(temp.y, bottomBound, topBound);

		//This will Translate, or move, the camera's position to the mouse's position
		gameObject.transform.position = temp;
	}
	//==================CameraZooming=================================
	//Method -- CameraZooming
	//Parameters -- value
	//Purpose -- add new value to orthogragive size .
	public void CameraZooming(float value)
	{
		//Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mousePosOrigin);
		//Vector3 mov = pos.y * zoomingSpeed * transform.forward;
		//transform.Translate(mov, Space.World);
		if (GetComponent<Camera>().orthographicSize < 5 && value > 0)
		{
			GetComponent<Camera>().orthographicSize += value;
		}
		if (GetComponent<Camera>().orthographicSize > 2.4 && value < 0)
		{
			GetComponent<Camera>().orthographicSize += value;
		}
	}
	//==================camera follow the unit being control=======================
	private void CameraFollowUnit()
	{
		if(panCam == false && focusedUnit != null)
		{
			newPosition = new Vector3 (focusedUnit.transform.position.x + 2.0f, focusedUnit.transform.position.y - 4.0f, -10.0f);
			newPosition.x = Mathf.Clamp (newPosition.x, leftBound, rightBound);
			newPosition.y = Mathf.Clamp (newPosition.y, bottomBound, rightBound);
			Camera.main.transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * camSpeed);
		}
	}
	//========================================================
	//set position co camera follow the span.
    private void CameraFollowSpawn()
    {
        newPosition = new Vector3(spawner.transform.position.x + 2.0f, spawner.transform.position.y + 2.0f, -10.0f);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        newPosition.y = Mathf.Clamp(newPosition.y, bottomBound, rightBound);
        Camera.main.transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * camSpeed);
    }
	//==================camera follow the unit being control=======================
	//Method -- UnitFocus
	//Parameters -- GameObject unitObj
	//Purpose -- UnitFocus will take in an argument of GameObject unitObj, this will tell the camera to then focus on that
	//GameObject. Essentially, the camera's transform will be changed to that of the transform of the GameObject.
	public void UnitFocus(Unit unitObj)
	{
		//Assigns the transform the unitObj to the camera
		newPosition = new Vector3 (unitObj.transform.position.x - 2.5f, unitObj.transform.position.y + 2.0f, -10.0f);
		newPosition.x = Mathf.Clamp (newPosition.x, leftBound, rightBound);
		newPosition.y = Mathf.Clamp (newPosition.y, bottomBound, rightBound);
		Camera.main.transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * camSpeed);
		focusedUnit = unitObj;
	}

	//Method -- SpawnIconFocus
	//Paramers: GameObject
    //Purpose:
	/* This method is called when the player goes into the buy state of the game. An icon will
     * be instantiated by the SpawnManager in which the CameraManager will then shift its focus
     * to the said icon, ignoring all other commands until it exits the buy state. 
     */
    public void SpawnIconFocus(SpawnIconBehavior spawnObj)
    {
        newPosition = new Vector3(spawnObj.transform.position.x - 2.5f, spawnObj.transform.position.y + 2.0f, -10.0f);
        newPosition.x = Mathf.Clamp(newPosition.x, leftBound, rightBound);
        newPosition.y = Mathf.Clamp(newPosition.y, bottomBound, rightBound);
        Camera.main.transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * camSpeed);
        spawner = spawnObj;
    }

}
