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
	public UnitShootingScipt barrel;
	//public Unit unit;


	private float back = -1.0f;
	private float forward = 1.0f;
	private int unitFocused;
	private Vector3 lastMousePosition;
	private RaycastHit2D cameraRay;

	private GameObject tmp;

	private GameObject temp;

	//Private members for buy state
	private bool newUnitCoolDown;
	private bool buyingMode =false;
	private bool isBuyPressed;
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
		newUnitCoolDown = false;
		isBuyPressed = false;
		unitSelected = false;
		isNotHit = true;
		cameraManager = Camera.main.GetComponent<CameraManager>();
		//gameManager = gameObject.GetComponent<GameManager>();
        spawnManager = gameObject.GetComponent<SpawnManager>();
		unit = gameManager.GetFocusedUnit().GetComponent<Unit>();   
		//handleAllInputs += HandleAddUnit;
		barrel = gameManager.GetFocusedUnit ().GetComponent<Unit> ().GetComponent<UnitShootingScipt> ();

		handleAllInputs += HandleAddUnit;
		handleAllInputs += HandleCameraPanning;
		handleAllInputs += HandleCameraZooming;
		handleAllInputs += GoIntoBuyingState;
		
		handleUnitInputs += HandleUnitCameraFocus;
		handleUnitInputs += HandleUnitMovement;
		handleUnitInputs += HandleUnitShoot;
		handleUnitInputs += HandleCannonAngle;
        
		handleBuyInputs += HandleBuyingState;
		//handleBuyInputs += HandleSpawnState;

		cameraManager.UnitFocus(unit);
	}
	//=============================================================================================================
	// Update is called once per frame
	void Update() 
	{
		if (buyingMode) {
			Time.timeScale = 0.2F;
		} else {
			Time.timeScale = 1.0F;
		}
        handleAllInputs();
        handleBuyInputs();
		if(gameManager.focusedUnit != null && cameraManager.isBuying == false){
			handleUnitInputs();
            unit = gameManager.focusedUnit.GetComponent<Unit>();
        }
        else if(cameraManager.isBuying == true){
        	HandleSpawnState();
        }

		timerShot += Time.deltaTime;
		timerBuy += Time.deltaTime;
	}
	//===========================================================================================================================	
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
        if(Input.GetKey("a"))
        {
            unit.setAnimation(true);
            unit.MoveFocusedUnit(back);
        }
        else if(Input.GetKey("d")/*Down(KeyCode.D)*/)
        {
            unit.setAnimation(true);
            unit.MoveFocusedUnit(forward);
        }
        else
        {
            unit.setAnimation(false);
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
		else
		{
			//	unit.setAnimation(false); // i remove so there is no conflict of 2 functon affect. 
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

	// =================Adding new unit===========================
	void HandleAddUnit()
	{
		if (Input.GetButtonDown ("SpawnTank")) 
		{
			GameObject newUnit = Instantiate(Resources.Load("TankObj-A"),gameManager.SpawnLocation.position,gameManager.SpawnLocation.rotation) as GameObject;
			gameManager.AppendUnitToList (newUnit);
			gameManager.CycleUnits();
			gameManager.SetFocusedUnit();
			cameraManager.UnitFocus(newUnit.GetComponent<Unit>());
		}
	}

    // ===================The buying state======================
    void GoIntoBuyingState()
    {
        if(Input.GetKeyDown(KeyCode.B) && isBuyPressed == false)
        {
			isBuyPressed = true;
			tmp = Instantiate(Resources.Load("Temp_Spawn_Icon"), Camera.main.ScreenToWorldPoint(Input.mousePosition), gameManager.SpawnLocation.rotation) as GameObject;
			cameraManager.isBuying = true;
            cameraManager.SpawnIconFocus(tmp.GetComponent<SpawnIconBehavior>());

        }
		else {

		}
    }

    void HandleBuyingState()
    {
		if(isBuyPressed == true )
		{
			buyingMode = true;
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				type = Unit_Type.ARTILLERY;
				unitSelected = true;

			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				type = Unit_Type.INFANTRY;
				unitSelected = true;

			}
			else if(Input.GetKeyDown(KeyCode.Alpha3))
			{
				type = Unit_Type.TANK;
				unitSelected = true;

			}

		}
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			buyingMode = false;
			cameraManager.isBuying = false;
			isBuyPressed = false;
			unitSelected = false;
			Destroy(tmp);
		}

    }

	void HandleSpawnState()
	{
		//Transforms the mouse position from screen space to viewport space
		Vector3 mousePosNormalized;


		if(unitSelected == true && timerBuy >= timeBetweenBuy)
		{
			if(Input.GetMouseButtonDown(0))
			{
				mousePosNormalized = new Vector3(tmp.transform.position.x, -2.1238f, 0.0f);
				temp = spawnManager.SpawnUnit(type, mousePosNormalized.x);

				gameManager.AppendUnitToList (temp);
				spawnManager.Target = mousePosNormalized;
				spawnManager.CoroutineForMove(spawnManager.Target, temp);

				//newUnitCoolDown = true;

				cameraManager.isBuying = false;
				isBuyPressed = false;
				unitSelected = false;
				Destroy(tmp);
				timerBuy = 0;
			}

			if(Input.GetKeyDown(KeyCode.Escape))
			{

				cameraManager.isBuying = false;
				isBuyPressed = false;
				unitSelected = false;
				Destroy(tmp);
				buyingMode = false;
			}
		}
	
	}
	void HandleUnitShoot()
	{
		if (Input.GetButtonDown ("Shoot") && timerShot >= timeBetweenShot) {
			//this.transform.GetChild(0).transform.Rotate(Vector3.forward * angle);
			GameObject newUnit = Instantiate(Resources.Load("Bullet"),unit.transform.GetChild(0).GetChild(0).transform.position,unit.transform.GetChild(0).transform.rotation) as GameObject;
			newUnit.rigidbody2D.AddForce(900.0f * unit.transform.GetChild(0).transform.right);
			unit.GetComponent<Unit>().ShootSound();
//			barrel.flash();
			//unit.GetComponentInChildren<UnitShootingScipt>()
			timerShot = 0;
		}
	}
}
