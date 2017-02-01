using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/***************************************************************************************
 * CLASS: DeployUnits                                                                  *
 * ------------------------------------------------------------------------------------*
 * The DeployUnits class is called when the player has selected a level. This class    *
 * will be in charge of which unit was selected to be used for the level. Upon         *
 * selection, the player can un-deploy the unit should they change their mind. Once    *
 * four units has been selected, a pop menu window will appear confirming their choice.*
 * If they decide to cancel, the list is emptied and the player has to once again      *
 * choose four units. If the Proceed button is pushed, the game will remember the units*
 * the player has chosen to deploy and proceed to the actual game level. The spawn     *
 * system will also remember these units, and only them to be the units be bought for  *
 * the current level.                                                                  *
 *-------------------------------------------------------------------------------------*
 * MEMBER METHODS:                                                                     *
 *                                                                                     *
 * Public / Non-Helper Methods --                                                      *
 *                                                                                     *
 * NextButtonPushed                                                                    *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * PrevButtonPushed                                                                    *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * AddButtonPushed                                                                     *
 * -> Parameers: None                                                                  *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * RemoveButtonPushed                                                                  *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * CancelButtonPressed                                                                 *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * ProceedButtonPressed                                                                *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 *                                                                                     *
 * Private / Helper Methods --                                                         *
 *                                                                                     *
 * PopulateStateInfo                                                                   *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * CheckIfReady                                                                        *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * RemoveGameObjectUnitIcon                                                            *
 * -> Parameters: int indexRemoveUnit                                                  *
 * -> Returns: None                                                                    *
 *                                                                                     *
 * PopulateIcon                                                                        *
 * -> Parameters: None                                                                 *
 * -> Returns: None                                                                    *
 **************************************************************************************/

public class DeployUnits : MonoBehaviour
{
    //Declaring local members

    //Private --
    private GameObject bigIcon;
    private int tempMaxUnitsToDeploy = 4;
    private List<int> listIndexUnlocked;
    private int index;
    private int deployedCount = 0;
    private List<DeployedUnitInfo> deployed;
    //  private int maxSquad = 3;

    //Public --
    public Text Stats;
    public Text UnitName;
    public GameObject temp;
    public List<GameObject> smallIconsOfUnitsDeployed;
    public GameObject popUpReadyWindow;
    public List<Transform> menuDeployedUnitTransform;

    //Method: DeployedUnitInfo
    //Purpose -- A struct that contains all the information
    //about the unit the player has picked to deploy.
    private struct DeployedUnitInfo
    {
        public string unitName;
        public int unitDmg;
        public int unitHP;
        public float unitAtkRate;
        public float unitMoveSpeed;
        public int unitCost;
        public int unitID;
    }


    // Use this for initialization
    void Start()
    {
//		if (SquadManager.instance.newSquadID == -1) {
//			sceneControl.ToWorldMap();
//		}
        //Instantiating the list of structs
        deployed = new List<DeployedUnitInfo>();
        listIndexUnlocked = UnitArsenal.instance.GetListIndexOfUnlockUnit();
        index = listIndexUnlocked[0];
        //This method is called in order to populate menu with 
        //information about the current unit in the list, which
        //in this case, should be the first element in the list.
        PopulateStatInfo();
        
        //Instantiates the icon for the current unit to be displayed. 
        //The name of the prefab should be the same as the one in the 
        //list inside the Player Profile class.
        temp = Instantiate(Resources.Load("TEST Unit Prefabs/" + UnitArsenal.instance.GetNameAt(index), 
                typeof(GameObject)), temp.transform.localPosition, Quaternion.identity) as GameObject;

        //Set this to false so the pop window won't appear until the 
        //list of selected units has been filled.
        // popUpReadyWindow.SetActive(false);
    }

    //Method: PopulateStatInfo
    //Purpose -- This is called to populate the menu about the current
    //unit object that is selected in the list.
    void PopulateStatInfo()
    {
        //Assigns the text field in the UI with the current unit name
        UnitName.text = "Unit Name: " + UnitArsenal.instance.GetNameAt(index) + '\n';

        //Assigns the text field with the rest of the unit's stats
        Stats.text = "Health : " + UnitArsenal.instance.GetHPAt(index).ToString() + "\n\n"
        + "Damage : " + UnitArsenal.instance.GetDmgAt(index).ToString() + "\n\n"
        + "Movement : " + UnitArsenal.instance.GetMoveSpeedAt(index).ToString() + "\n\n"
        + "Fire Rate : " + UnitArsenal.instance.GetFireRateAt(index).ToString();
    }

    //Method: NextButtonPushed()
    //Purpose -- Called when the Next button is pressed by the player. Cycles through the
    //list to get the next element and assign it to be the current unit.
    public void NextButtonPushed()
    {
        //Checks if the index value is equal to the list size inside the Player Profile
        int i = listIndexUnlocked.IndexOf(index);
        if (i == listIndexUnlocked.Count - 1)
        {
            index = listIndexUnlocked[0];
        }
        else
        {
            index = listIndexUnlocked[i++];
        }

        //This method is called in order to populate menu with 
        //information about the current unit in the list.
        PopulateStatInfo();

        //Since the previous icon is no longer being displayed,
        //destroy it.
        Destroy(temp);

        //Instantiates the icon for the current unit to be displayed. 
        //The name of the prefab should be the same as the one in the 
        //list inside the Player Profile class.
        temp = Instantiate(Resources.Load("TEST Unit Prefabs/" + UnitArsenal.instance.GetNameAt(index), 
                typeof(GameObject)), temp.transform.localPosition, Quaternion.identity) as GameObject;
    }

    //Method: PreviousButtonPushed()
    //Purpose -- Called when the Prev button is pressed by the player. Cycles through the
    //list to get the previous element and assign it to be the current unit.
    public void PrevButtonPushed()
    {
        //Checks if the list index is zero, meaning if the index is
        //pointing to the first element
        int i = listIndexUnlocked.IndexOf(index);
        if (i == 0)
        {
            index = listIndexUnlocked[listIndexUnlocked.Count - 1];
        }
        else
        {
            index = listIndexUnlocked[i--];
        }

        //This method is called in order to populate menu with 
        //information about the current unit in the list.
        PopulateStatInfo();

        //Since the previous icon is no longer being displayed,
        //destroy it.
        Destroy(temp);

        //Instantiates the icon for the current unit to be displayed. 
        //The name of the prefab should be the same as the one in the 
        //list inside the Player Profile class.
        temp = Instantiate(Resources.Load("TEST Unit Prefabs/" + UnitArsenal.instance.GetNameAt(index), 
                typeof(GameObject)), temp.transform.localPosition, Quaternion.identity) as GameObject;
    }

    //Method: AddButtonPushed()
    //Purpose -- The main meat of this class. Called when the player pressed the
    //Add button in the menu. Adds the current unit in the list into the deployed
    //unit list.
    public void AddButtonPushed()
    {
        //Declaring local variables
        DeployedUnitInfo tmp;

        //Checks if the deployed counter is less than max. alloted slots to deploy, and another check
        //if the name of the current unit about to be deployed is already in the deployed unit list.
        //If the name is there, don't add it.
        if (deployedCount < tempMaxUnitsToDeploy &&
            !deployed.Exists(name => name.unitName == UnitArsenal.instance.GetNameAt(index)))
        {
            //Instantiates the small icon into a variable called temp.
            GameObject tempicon = Instantiate(Resources.Load("TEST Unit Prefabs/IconGO/" + UnitArsenal.instance.GetNameAt(index), 
                                          typeof(GameObject))) as GameObject;

            //Adds the instantiated object into the list of deployed icons.
            smallIconsOfUnitsDeployed.Add(tempicon);
            
            //This block of code adds all the information associated with the deployed unit into the 
            //struct.
            tmp.unitName = UnitArsenal.instance.GetNameAt(index);
            tmp.unitDmg = UnitArsenal.instance.GetDmgAt(index);
            tmp.unitHP = UnitArsenal.instance.GetHPAt(index);
            tmp.unitAtkRate = UnitArsenal.instance.GetFireRateAt(index);
            tmp.unitMoveSpeed = UnitArsenal.instance.GetMoveSpeedAt(index);
            tmp.unitCost = UnitArsenal.instance.GetUnitCostAt(index);
            tmp.unitID = UnitArsenal.instance.GetUnitIDAt(index);

            //Add the instantiated struct into the deployed list of structs
            deployed.Add(tmp);

            //Increments the deployed counter
            deployedCount++;
        }

        //This method is called in order to populate menu with 
        //information about the current unit in the list.
        PopulateIcon();

        //This method is called to check if the user has already 
        //deployed units equal to the max. alloted of units to deploy.
        CheckIfReady();
    }

    //Method: RemoveButtonPushed()
    //Purpose -- Removes the unit in the deployed unit list that is currently
    //being shown in the big icon. Populates the deployed unit list again
    //if a unit has been removed.
    public void RemoveButtonPushed()
    {
        //Declaring local variables
        GameObject tmp;
        int indexToBeRemoved;

        //Checks if the deployed counter is not empty, and another check
        //if the name of the current unit about to be removed is already in the deployed unit list.
        //If the name is there, don't remove it.
        if (deployedCount > 0 &&
            deployed.Exists(name =>
		                name.unitName == UnitArsenal.instance.GetNameAt(index)))
        {
            //Set the index of unit to remove
            indexToBeRemoved = smallIconsOfUnitsDeployed.FindIndex(item => item.name.ToString() == temp.name.ToString());

            //Temporary keep track which game object to remove
            tmp = smallIconsOfUnitsDeployed[indexToBeRemoved];

            //Remove the the unit ID from the list
            //idOfUnitsDeployed.RemoveAt(indexToBeRemoved);
            deployed.RemoveAt(indexToBeRemoved);

            //Finall remove the small gameobject icon
            Destroy(tmp);
     
            //Remove icon from the list
            RemoveGameObjectUnitIcon(indexToBeRemoved);

            //With one unit removed, decrement our deployed count
            deployedCount--;

            //Populate the deployed list of icons again
            PopulateIcon();
        }
    }

    //Method: CancelButtonPushed()
    //Purpose -- When this button is pressed, the entire deployed list is
    //cleared. The player has to select up to four units again to deploy.
    public void CancelButtonPressed()
    {
        //Declaring local variables
        GameObject tmp;

        //Take the Confirmation Window out of view.
        popUpReadyWindow.SetActive(false);

        //For-loop that will loop through the whole
        //list of deployed units. Will remove the
        //deployed unit objects in ascending order.
        for (int i = 0; i < 4; i++)
        {
            //Temporarily store the first element in the list to
            //the tmp variable
            tmp = smallIconsOfUnitsDeployed[0];

            //Remove the object the at the first element.
            smallIconsOfUnitsDeployed.RemoveAt(0);
            
            //Destroy the tmp object just to be sure.
            Destroy(tmp);
        }

        //Since the list empty, set deployed count to zero.
        deployedCount = 0;

        //Empty the list of struct objects.
        deployed.Clear();
    }

    //Method: ProceedButtonPressed
    //Purpose -- Meant to be only pushed when the player is absolutely
    //confident thay they are ready to play the level. If pressed,
    //the scene will then transition to the level scene.
    public void ProceedButtonPressed()
    {
        //MapController.instance.SceneTransition(true);
        SquadManager.instance.AddSquad(deployed[0].unitID,
            deployed[1].unitID,
            deployed[2].unitID,
            deployed[3].unitID);
        UnitArsenal.instance.SetQuantity(deployed[0].unitID, -1);
        UnitArsenal.instance.SetQuantity(deployed[1].unitID, -1);
        UnitArsenal.instance.SetQuantity(deployed[2].unitID, -1);
        UnitArsenal.instance.SetQuantity(deployed[3].unitID, -1);

        //This loop will iterate through the list of structs. 
        //The iterator will then have the current struct object
        //in the list.
//        foreach(DeployedUnitInfo info in deployed)
//        {
//            //Calls the singleton SpawnManager GameObject and stores the values in the 
//            //info struct in its AddDeployInfo method. 
//            SpawnManager.instance.AddDeployInfo(info.unitName, info.unitDmg, info.unitHP, info.unitAtkRate,
//                                                info.unitMoveSpeed, info.unitCost);
//        }

        //Once done, finally transition to the next scene, which is the game level.
       
    }

    //Method: CheckIfReady
    //Purpose -- Helper function that checks if the deployed
    //list of units has aleady reached its max. capacity.
    //Shows the pop window that asks the player if they are
    //indeed ready to go to the game level.
    private void CheckIfReady()
    {
        //Checks if the deployed list of units has aleady reached
        //its max. capacity. In this case, the player can only
        //initially deploy four units max.
        if (smallIconsOfUnitsDeployed.Count == tempMaxUnitsToDeploy)
        {
            //Calls the SetActive method inherited in the Canvas object
            //assigns it to true. In layman's terms, this will allow
            //the pop-up window to be shown in the scene.
            popUpReadyWindow.SetActive(true);
        }
    }

    //Method: RemoveGameObjectUnitIcon
    //Purpose -- Helper method that takes in an integer parameter
    //that will serve as the index that will point which element
    //in list to be removed.
    private void RemoveGameObjectUnitIcon(int indexRemoveUnit)
    {
        //Calls the RemoveAt method in the small icons list
        //and passes in the index.
        smallIconsOfUnitsDeployed.RemoveAt(indexRemoveUnit);
    }

    //Method: PopulateIcon
    //Purpose -- Helper method that populates the menu with the GameObject
    //elements in the small icon list
    private void PopulateIcon()
    {
        //Loops through until the iterator has reached the current amount
        //of units the player has chosen to deploy thus far.
        for (int i = 0; i < deployed.Count; i++)
        {
            //This instruction will allow the deployed objects to be shown in the screen.
            //Each element in the small icons list is related to each Transform element in
            //the menu deployed unit transform list. Meaning, that the transform properties
            //in the latter list will be assigned to to the current small unit icon object
            //in the small icons list.
            smallIconsOfUnitsDeployed[i].transform.position = menuDeployedUnitTransform[i].transform.position;
        }
    }


}