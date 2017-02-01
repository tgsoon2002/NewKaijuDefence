using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{

    #region Data Member

    //refrence for the pause menu panel in the hierarchy
    //public GameObject pauseMenuPanel;

    public List<Button> listOfPauseButton;
    Button currentSelectButton;

    //member use to move the pause board around.
    private Vector3 tempPosition;
    private Vector3 unPausePosition;
    private Vector3 pausePosition;
    private float objSpeed = 15.0f;
    private int directionMove = 0;
    public GameObject UICanvas;

    //Parameter for manager warfund and warfund rte
    private int warFund = 200;
    private float warFundRate;
    public Text warFundText;
    public Text warningText;

    public Text unitBuyText1;
    public Text unitBuyText2;
    public Text unitBuyText3;
    public Text unitBuyText4;

    //variable for checking if the game is paused
    private bool isPaused = false;

    //member use for buying unit
    public GameObject spawnIcon;
    private int tempSpawnUnitIndex = -1;

    private static PauseMenuScript _instance;

    public static PauseMenuScript instance
    {
        get { return _instance; }
    }

    #endregion

    #region getter and setter

    public int PauseMenu_WarFund
    {
        get { return warFund; }
    }

    #endregion

    #region built-in method

    void Awake()
    {   
        _instance = this;
    }

    void Start()
    {
        //set vector3 of some require for moving board
        tempPosition = gameObject.GetComponent<RectTransform>().position;
        pausePosition = tempPosition;   // pause position is currently,
        //unPausePosition = tempPosition;
			
        // aline pauseposition to the origin for x and z
        Debug.Log(UICanvas.GetComponent<RectTransform>().sizeDelta.y);
        unPausePosition.y = UICanvas.GetComponent<RectTransform>().sizeDelta.y;  // set y( the height to UI canvas.)
        // Set the text for button accodring to list of unit
        SetButtonText();
        //set value for warfund
        //warFund = 10000;
        warFundText.text = "Fund: " + warFund.ToString();
        // set the menu at start of game in unpuase mode.
        gameObject.GetComponent<RectTransform>().position = unPausePosition;

    }



    // Update is called once per frame
    public void Update()
    {
	
        // move the panel smoothly if pause or unpause
        if (directionMove != 0)
        {
            if ((gameObject.transform.position.y >= pausePosition.y && directionMove == 1)//when unpausing and current position is smaller than pause position
            || (gameObject.transform.position.y <= unPausePosition.y && directionMove == -1))
            { //when pausing and current position is larger than origional position
                directionMove = 0;
            }
            else
            {
                tempPosition.y += (objSpeed * directionMove);
                gameObject.transform.position = tempPosition;
            }
        }

        //Will increment the war funds every 2.5 seconds, set timer back, update text in warfund.
        if (warFundRate >= 2.5f)
        {
            ChangeWarFund(25);
            warFundRate = 0;

        }	 
        warFundRate += Time.deltaTime;
	
        // when button was press and tempUnit was choosed
        if (Input.GetMouseButtonUp(1) && tempSpawnUnitIndex != -1)
        {
            SpawmnUnit();
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && tempSpawnUnitIndex != -1)
        {
            CancelSpawn();
        }
        //Debug.Log("temp index: " + tempSpawnUnitIndex);
        if (!isPaused)
        {
            if (Input.GetButtonDown("StartButton"))
            {
                PauseGame();
            }
        }
        if (isPaused)
        {
//			if (Input.GetButtonDown("ControllerA")) {
//				BuyUnit(0);
//			}
//			if (Input.GetButtonDown("ControllerB")) {
//				BuyUnit(1);
//			}if (Input.GetButtonDown("ControllerX")) {
//				BuyUnit(2);
//			}
//			if (Input.GetButtonDown("ControllerY")) {
//				BuyUnit(3);
//			}
//			if (Input.GetAxis("DPadY")>.5) {
//				UnpauseGame();
//			}
//			if (Input.GetButtonDown("StartButton")) {
//				sceneManager.ToMainMenu();
//			}
        }
    }

    #endregion

    #region PauseMethod

    //function to pause the game
    public void PauseGame()
    {
        isPaused = true;
        directionMove = 1;
        Time.timeScale = 0.2F;
        //currentSelectButton = listOfPauseButton[0];
        //currentSelectButton.c = Color.gray;
    }
    //function to unpause the game
    public void UnpauseGame()
    {
        isPaused = false;
        directionMove = -1;
        Time.timeScale = 1.0F;
    }

    public bool PauseState()
    {
        return isPaused;
    }

    private void ChangeFocus()
    {

    }

    private void ButtonLoseFocus()
    {

    }

    #endregion

    #region BuyUnitMehod

    // pick unit to buy
    public void BuyUnit(int Index)
    {
        //Debug.Log(Index);
        //int Index = 1;
        if (SpawnManager.instance.GetCostOfUnit(Index) <= warFund)
        {
            tempSpawnUnitIndex = Index;
            CreateSpawningIndicator();

            directionMove = -1;	
        }
        else
        {
            warningText.text = "You dont have enough resource";
        }
			
    }

    private void ChangeWarFund(int newVaue)
    {
        warFund += newVaue;
        warFundText.text = "Fund: " + warFund.ToString();
    }

    private void CreateSpawningIndicator()
    {
        spawnIcon = Instantiate(spawnIcon, Camera.main.transform.position, gameObject.transform.rotation) as GameObject;
        //CameraManager.instance.CameraManager_FocusObject = spawnIcon;
    }

    private void SpawmnUnit()
    {
        Transform tempTrans = spawnIcon.transform;
        SpawnManager.instance.SpawnUnit(tempSpawnUnitIndex, tempTrans);
        ChangeWarFund(-SpawnManager.instance.GetCostOfUnit(tempSpawnUnitIndex));
        tempSpawnUnitIndex = -1;
        Destroy(spawnIcon);
        UnpauseGame();
    }

    private void CancelSpawn()
    {
        tempSpawnUnitIndex = -1;
        Destroy(spawnIcon);
        UnpauseGame();
    }

    // set the name of unit for for button.
    public void SetButtonText()
    {
        unitBuyText1.text = SpawnManager.instance.GetNameOfUnit(0) + " (" + SpawnManager.instance.GetCostOfUnit(0) + ")";
        unitBuyText2.text = SpawnManager.instance.GetNameOfUnit(1) + " (" + SpawnManager.instance.GetCostOfUnit(1) + ")";
        unitBuyText3.text = SpawnManager.instance.GetNameOfUnit(2) + " (" + SpawnManager.instance.GetCostOfUnit(2) + ")";
        unitBuyText4.text = SpawnManager.instance.GetNameOfUnit(3) + " (" + SpawnManager.instance.GetCostOfUnit(3) + ")";
    }

    #endregion

}