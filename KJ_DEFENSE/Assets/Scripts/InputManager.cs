using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
	//Declaring local variables

	//--Private Members
	public delegate void HandleAllInputs();
	public delegate void HandleUnitInputs();
    public delegate void HandleBuyStateInputs();
		
    private SpawnManager spawnManager;
	private CameraManager cameraManager;
	private SpawnIconBehavior icon;

	private HandleAllInputs handleAllInputs;
	private HandleUnitInputs handleUnitInputs;
    private HandleBuyStateInputs handleBuyInputs;
	//public Unit unit;
	private bool paused;

	private float back = -1.0f;
	private float forward = 1.0f;
	private int unitFocused;
	private Vector3 lastMousePosition;
	private RaycastHit2D cameraRay;

	private GameObject spawnIndicator;

	private GameObject newUnitJustSpawn;

	//Private members for buy state
	private bool buyingMode = false;
	private bool unitSelected;
	private Unit_Type type;
	
	public float timeBetweenShot = 1.5f;
	public float timeBetweenBuy = 2.0f;
    public Unit unit;

	public GameManager gameManager;

    //public GameObject icon;

	float timerShot;
	public float timerBuy;

	private bool isNotHit;

	// Use this for initialization
	void Start () 
	{	
		unitSelected = false;
		isNotHit = true;
		cameraManager = Camera.main.GetComponent<CameraManager>();
        spawnManager = gameObject.GetComponent<SpawnManager>();
		unit = gameManager.GetFocusedUnit().GetComponent<Unit>();   

		// handle control unit, disable in pause, buying and unitchoosed mode.
		handleUnitInputs += HandleUnitCameraFocus;
		handleUnitInputs += HandleUnitMovement;
		handleUnitInputs += HandleUnitShoot;
		handleUnitInputs += HandleCannonAngle;
        // handle control the camera.
		handleAllInputs += HandleCameraPanning;
		handleAllInputs += HandleCameraZooming;

		// handle the controller of buying the unit
		handleBuyInputs += GoIntoBuyingState;
		handleBuyInputs += HandleBuyingState;

		cameraManager.UnitFocus(unit);
	}
	//=============================================================================================================
	// Update is called once per frame
	void Update() 
	{
		// change time scale if change to buying mode.
		if (paused) {
			Time.timeScale = 0.2F;

			if (unitSelected) {
				HandleSpawnState();
			}
			else {
				handleBuyInputs();
			}

		} else {
			Time.timeScale = 1.0F;
			handleUnitInputs();
			unit = gameManager.focusedUnit.GetComponent<Unit>();
		}
        handleAllInputs();
        
		// make timmer tick
		timerShot += Time.deltaTime;// Cooldown between shot
		timerBuy += Time.deltaTime; // cooldown between buy
	}
	//================================switch unit being focus with tab==================================================	
	//press tab to switch between unit.
	void HandleUnitCameraFocus()
	{
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			cameraManager.isPanning = false;
			isNotHit = true;
			unit.setAnimation(false);
			gameManager.CycleUnits();
			gameManager.SetFocusedUnit();
			unit = gameManager.GetFocusedUnit().GetComponent<Unit>();
			cameraManager.UnitFocus(unit);
		}

		if(isNotHit == false)
		{
			if (cameraRay.collider.gameObject.CompareTag("Tank"))
			{
				unit.setAnimation(false);
				unit = cameraRay.collider.gameObject.GetComponent<Unit>();
			}
			cameraManager.UnitFocus(unit);
		}
	}

	 //===========press a or d to call move functoin of  unit being controled=================
	void HandleUnitMovement()
	{
       //float move = Input.GetAxis ("Horizontal");
		//unit.MoveFocusedUnit(move);
		if(Input.GetKey("a")) // move unit back.
        {
            unit.setAnimation(true);
            unit.MoveFocusedUnit(back);
        }
        else if(Input.GetKey("d"))
        {
            unit.setAnimation(true);
            unit.MoveFocusedUnit(forward);
        }
        else // if moving unit buton not press, set animation to idle 
        {
            unit.setAnimation(false);
        }
	}
	//===========press spacebar to make unit shoot=================
	void HandleUnitShoot()
	{
		if (Input.GetButtonDown ("Shoot") && timerShot >= timeBetweenShot) {
			
			GameObject newUnit = Instantiate(Resources.Load("Bullet"),unit.transform.GetChild(0).GetChild(0).transform.position,unit.transform.GetChild(0).transform.rotation) as GameObject;
			newUnit.GetComponent<Rigidbody2D>().AddForce(900.0f * unit.transform.GetChild(0).transform.right);
			unit.GetComponent<Unit>().ShootSound();
			timerShot = 0;
		}
	}
	//===========press w or s to change functoin change angle of controlling unit=================
	void HandleCannonAngle()
	{
		if(Input.GetKey("w"))
		{	
			unit.ChangeCannonAngle(forward);
		}		
		else if(Input.GetKey("s")/*Down(KeyCode.D)*/)
		{
			unit.ChangeCannonAngle(back);
		}
	}


	// ========move mouse while holding left click to pan===============
	void HandleCameraPanning()
	{
		//Declaring local variables
		if(Input.GetMouseButtonDown(0))
		{
			cameraManager.isPanning = true;
			cameraRay = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(cameraRay.collider == null)
			{
				//Get the origin of the mouse's position
				lastMousePosition = Input.mousePosition;
				isNotHit = true;
			}
			else
			{
				isNotHit = false;
			}
		}
		if (Input.GetMouseButton(0) && isNotHit == true)
		{
			cameraManager.CameraPanning(lastMousePosition);
		}
	}

	// ===============zoom in and out with mouse wheel=====================
	void HandleCameraZooming()
	{
		float newValue = Input.GetAxis("Mouse ScrollWheel");
		cameraManager.CameraZooming(newValue);
	}

    // ===================change to buying state if not in that mode yet======================
	// change state to buying state, create an indicator 
    void GoIntoBuyingState()
    {
		if( paused == true && buyingMode == true && unitSelected)
		{
			CreateIndicator();
        }
    }
	private void CreateIndicator(){
			spawnIndicator = Instantiate(Resources.Load("Temp_Spawn_Icon"), Camera.main.ScreenToWorldPoint(Input.mousePosition), gameManager.SpawnLocation.rotation) as GameObject;
			cameraManager.isBuying = true;
			cameraManager.SpawnIconFocus(spawnIndicator.GetComponent<SpawnIconBehavior>());
		}
	//============ control while in buying state==================
    void HandleBuyingState()
    {
		if(buyingMode == true )
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				buyArtillery();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				buyInfantry();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				buyTank();
			}
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				ExitBuyingState();
			}
		}
    }
	public void buyTank(){
		type = Unit_Type.TANK;
		unitSelected = true;
		cameraManager.GetComponent<PauseMenuScript>().UnpauseGame();
		CreateIndicator();
	}
	public void buyArtillery(){
		type = Unit_Type.ARTILLERY;
		unitSelected = true;
		cameraManager.GetComponent<PauseMenuScript>().UnpauseGame();
		CreateIndicator();
	}
	public void buyInfantry(){
		type = Unit_Type.INFANTRY;
		unitSelected = true;
		cameraManager.GetComponent<PauseMenuScript>().UnpauseGame();
		CreateIndicator();
	}

	// =======handle input when in buying mode and unit type selected==============
	void HandleSpawnState()
	{
		//Transforms the mouse position from screen space to viewport space
		Vector3 destination;

        // unit type selected and cool down is enoug.
		if(unitSelected == true && timerBuy >= timeBetweenBuy)
		{
			if(Input.GetMouseButtonDown(0))
			{
				//set new destination for the unit at spawn indicator and set destination in spawn manager
				destination = new Vector3(spawnIndicator.transform.position.x, spawnIndicator.transform.position.y, 0.0f);
				spawnManager.Target = destination;
				newUnitJustSpawn = spawnManager.SpawnUnit(type);

				// add new unit to manager
				gameManager.AppendUnitToList (newUnitJustSpawn);
				spawnManager.CoroutineForMove(destination, newUnitJustSpawn);
				Debug.Log("destination: " + destination.x.ToString() +" "+ destination.y.ToString() +" "+ destination.z.ToString());
				ExitBuyingState();
				timerBuy = 0;
			}

			if(Input.GetKeyDown(KeyCode.Escape))
			{

				ExitBuyingState();
			}
		}
	
	}


	//=======Exit Buying state=============
	void ExitBuyingState()
	{	cameraManager.isBuying = false;
		buyingMode = false;
		unitSelected = false;
		Destroy(spawnIndicator);
		paused = false;

		}

	public void Pause()
	{
		paused = true;
	}
	public void unPause()
	{
		paused = false;
	}
	public void buyingPressed()
	{
		buyingMode = true;
	}
}
