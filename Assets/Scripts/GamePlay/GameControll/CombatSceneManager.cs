using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//added 3.20.15 for database
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

/***************************************************************************************
 * CLASS: GameManager                                                                  *
 * ------------------------------------------------------------------------------------*
 * The GameManager class takes care of game state behaviors that occur in game levels. *
 * One of its many tasks is to keep track of the list of Unit objects, append said     * 
 * list, take out an element in the list, and tell the game which Unit object is       *
 * being used by the player at the current time.                                       *
 *                                                                                     *
 * As mentioned before, the GameManager keeps track of which state will be in, such as *
 * if the game level is still running, if the player has lost the mission objectives,  *
 * if the game is paused/been unpaused, and if the player won, etc.                    *
 *                                                                                     *
 * Another vital function this class performs is to talk to the database. This class   *
 * will be in charge of setting the amount of Research Points and which loot the       *
 * player will receive at the end of each level.                                       * 
 *                                                                                     *
 **************************************************************************************/
public class CombatSceneManager : MonoBehaviour
{
    #region member


    public GameObject mainEnemy;
    private GameObject focusedUnit;
    public Transform rightEnd;
    public Transform leftEnd;
    public MixLevels musicMixer;
    public CanvasManage canvasManage;
    [SerializeField]
    public List<GameObject> listOfUnits;
    public List<GameObject> listOfGolem;


    // End battle
    public bool winOrLose = false;
    int unitDestroyCount;
    public GameOverScript gameOverGO;

    public SquadStruct squadInfo;

    private static CombatSceneManager _instance;

    public static CombatSceneManager instance
    {
        get { return _instance; }
    }

    #endregion

    #region getter and setter

    public GameObject GameManager_FocusUnit
    {
        get { return focusedUnit; }
        set { focusedUnit = value; }
    }

    public int GameManager_UnitDestroyed
    {
        get { return unitDestroyCount; }
    }

    #endregion

    #region built-in method

    //Method: Awake
    //Purpose -- This called before Start, and allows the script
    //to order some of the members to be initialized (if needed);
    void Awake()
    {   
        //isAtStart = true;  
        //Initializing _instance to our current instance of 
        //the GameManager object
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

    }
	
    //Method: Start
    //Purpose -- This method is mainly used for initialization
    //of a class' private data fields. Use this method when
    //initializing stuff, do it here and only here.
    void Start()
    {
        squadInfo = SquadManager.instance.CurrentSquad;
        //=======Testing righ now. Need to call with parameter to create multiple Golem with golem ID========
        SpawnManager.instance.SpawnUnit(1, leftEnd);
        SetupGolem();
        //CameraManager.instance.CameraManager_MainEnemy = listOfGolem[0];
        GetAllUnitOnMap();
    }



    #endregion

    #region  Method related Unit

    //Method: AppendUnitToList
    //Parameters: GameObject newUnit
    //Purpose -- This method is called when a new Unit GameObject
    //is needed to appended to the list, usually when a new
    //Unit is instantiated from elsewhere.
    public void AddUnitToList(GameObject newUnit)
    {
        //Add the newly instantiated unit gameobject to the list

        //Check if the size of the list is exactly 1.
        if (listOfUnits.Count == 1)
        {
            //If true, then make this unit the new focused unit
            focusedUnit = newUnit;
            FocusedUnitChanged();
        }
    }

    public void RemoveUnitFromList(GameObject unitToDestroy)
    {
        if (focusedUnit == unitToDestroy && listOfUnits.Count > 1)
        {
            CycleFocus();
        }
        else if (listOfUnits.Count == 1)
        {
            focusedUnit = null;
        }
        listOfUnits.Remove(unitToDestroy);
        unitDestroyCount++;
    }

    //Method: SetFocusedUnit
    //Purpose -- This method will set the focused unit gameobject
    //to be equal to its parameter. To be used outside for camera
    //behavior purposes.
    public void SetFocusedUnit(GameObject newFocus)
    {
        focusedUnit.GetComponent<PlayerUnit>().Unit_Is_Controlled_Manually = false;
        focusedUnit = newFocus;
        FocusedUnitChanged();
    }


    // CycleFocus will search the index of current focus unit in the list.
    // Then increae to next index or rotate back the the first position.
    // After that, Set the focusUnit to the GO in list with the new index. and call focusUnitChange
    public void CycleFocus()
    {
        if (listOfUnits.Count > 1)
        {		
            focusedUnit.GetComponent<PlayerUnit>().Unit_Is_Controlled_Manually = false;
            int i = listOfUnits.IndexOf(focusedUnit);
            if (i == listOfUnits.Count - 1)
            {
                i = 0;
            }
            else
            {
                i++;
            }
            focusedUnit = listOfUnits[i];
            FocusedUnitChanged();
        }
    }

    // Called whenever focus unit have change.
    // change the focus unit of inpu mananger and camera manager.
    private void FocusedUnitChanged()
    {


        //CameraManager.instance.CameraManager_FocusObject = focusedUnit;
        //CameraManager.instance.CameraManager_IsPanning = false;
        //InputManager.instance.InputManager_FocusUnit = focusedUnit;
        focusedUnit.GetComponent<PlayerUnit>().Unit_Is_Controlled_Manually = true;
    }

    //	// add all game object which is unit to the list.
    public void GetAllUnitOnMap()
    {
        GameObject[] objTag = GameObject.FindGameObjectsWithTag("Unit");
        listOfUnits.Clear();

        if (objTag.Length > 0)
        {
            foreach (GameObject unit in objTag)
            {
                listOfUnits.Add(unit);
            }
            focusedUnit = listOfUnits[0];
        }
        FocusedUnitChanged();
    }

    #endregion

    //=================================

    #region Golem Related Methods

    //======Incomplete right now. need to have parameter of Golem ID=========
    //======and edit the golem info once we have the golem information=========
    public void AddGolemToList(GameObject tempGolem)
    {
        if (listOfGolem.Count == 0)
        {
            mainEnemy = tempGolem;
        }
        listOfGolem.Add(tempGolem);
    }

    // Call once golem is dead
    public void RemoveGolemFromList(GameObject deadGolem)
    {
        listOfGolem.Remove(deadGolem);
        Destroy(deadGolem);
        if (deadGolem == mainEnemy)
        {
            winOrLose = true;

            WinOrLose(true);
        }
    }

    public void GolemReachLeatPoint()
    {
        winOrLose = true;

        WinOrLose(false);
    }

    #endregion

    #region End Combat Scene related Method

    void WinOrLose(bool battleEnd)
    {

        if (battleEnd)
        {
            Debug.Log("Winbattle");
            gameOverGO.CombatWin(unitDestroyCount);
        }
        else
        {
            Debug.Log("lose battle");
            gameOverGO.CombatLose();
        }

    }

    void GenerateDropItem()
    {

    }

    void SetCityHealth()
    {

    }



    #endregion



    #region Begin Combat Scene related Method

 

    void SetupGolem()
    {
        for (int i = 0; i < 1; i++)
        {
            SpawnManager.instance.SpawnGolem(0);
        }
        musicMixer.mainEnemy = mainEnemy;
        Debug.Log(mainEnemy.GetComponent<Golem>());
        canvasManage.SetupGolemInfo(mainEnemy.GetComponent<Golem>());
    }

    void SetupBackground()
    {
		
    }

    #endregion


}