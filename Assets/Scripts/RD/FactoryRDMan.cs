using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
	function:
		-Start (call PopulateUnitInfo and PopulateResouse)
		-NextButtonPushed   (Call PopulateUnitInfo and ReplaceModel)
		-PrevButtonPushed 	(Call PopulateUnitInfo and ReplaceModel)
		
		-ConfirmBuild
		-CancelOrder
		-IncreaseOrder
		-DecreaseOrder
		
		-PopulateUnitInfo   
		-ReplaceModel
		-PopulateResouse

	Description:
		- Start : Create the icon model, populate "unit info", and "Player resourse"
		- NextButtonPushed: increase index by one or set to 0 if the last index. populate information and replace the icon model by the next model
		- PrevButtonPushed: decrease index by one or set to last index if reach 0. populate information and replace the icon model by the previous model
		- PopilateUnit Info: replace information of "name text", "Instock Text", "Stat txt" by infor of the next or prev index
		- ReplaceModel : delete the current model, instantiate the next or prev model
		- PopulateResouse : replace text of "Player budget" and require "ingredient" to buy the unit
		- ConfirmBuild : Check if enough resouce and unit is not default. 
 */
public class FactoryRDMan : MonoBehaviour
{

    #region Data Member

    public GameObject UnitModel;
    public List<int> listIndexUnlocked;
    public int tempUID;
    public int index = 0;
    private int orderNumber = 0;

    public Text NameAndDescTxt;
    public Text StatsTxt;
    public Text InstockTxt;
    public Text BudgetTxt;

    public Text Ingredient1Txt;
    public Text Ingredient2Txt;
    public Text Ingredient3Txt;
    public Text Ingredient4Txt;

    public Text UnitBdCost;
    public Text orderNumberText;
    public Text warningText;

    private struct ingredientOfUnit
    {
        public int ingre1;
        public int ingre2;
        public int ingre3;
        public int ingre4;
        public int buildCost;
    }

    private ingredientOfUnit tempIngredient = new ingredientOfUnit();

    #endregion

    #region built-in Method

    // Use this for initialization
    void Start()
    {
        //UnitModel = Instantiate(Resources.Load("Showing Model/" + UnitArsenal.instance.GetNameAt(index), 
        //                                       typeof(GameObject)),UnitModel.transform.localPosition,Quaternion.identity) as GameObject;
        listIndexUnlocked = UnitArsenal.instance.GetListIndexOfUnlockUnit();
        index = listIndexUnlocked[0];
        PopulateUnitInfo();
        PopulateResource();
        ReplaceModel();
    }

    #endregion

    #region Buton Method

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
            index = listIndexUnlocked[++i];
        }	
        //This method is called in order to populate menu with 
        //information about the current unit in the list.
        CancelOrder();
        CurrentIndexChange();
        PopulateUnitInfo();
        PopulateResource();
        ReplaceModel();
        warningText.text = "";
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
            index = listIndexUnlocked[--i];
        }
		
        //This method is called in order to populate menu with 
        //information about the current unit in the list.
        CancelOrder();
        CurrentIndexChange();
        PopulateUnitInfo();
        PopulateResource();
        ReplaceModel();
        warningText.text = "";

    }

    // Method: Confirm Build()
    public void ConfirmBuild()
    {
        if (UnitArsenal.instance.GetQuantityAt(index) >= 0)
        {
            int fID, sID, tID, foID;
            fID = BuildList.instance.IDOfFirIngreWithID(index);
            sID = BuildList.instance.IDOfSecIngreWithID(index);
            tID = BuildList.instance.IDOfThiIngreWithID(index);
            foID = BuildList.instance.IDOfFouIngreWithID(index);
            //Debug.Log(BuildList.instance.IDOfFirIngreWithID (index));
            if (PlayerResource.instance.GetBudget() > UnitArsenal.instance.GetBdCostAt(index) * orderNumber &&
            BuildList.instance.EnoughRes(fID, tempIngredient.ingre1 * orderNumber) &&
            BuildList.instance.EnoughRes(sID, tempIngredient.ingre2 * orderNumber) &&
            BuildList.instance.EnoughRes(tID, tempIngredient.ingre3 * orderNumber) &&
            BuildList.instance.EnoughRes(foID, tempIngredient.ingre4 * orderNumber))
            {
                //Decrease money
                int newbudget = PlayerResource.instance.GetBudget() - (UnitArsenal.instance.GetBdCostAt(index) * orderNumber);
                PlayerResource.instance.UpdateValue(newbudget);
                // Decrease Ingredient 
                BuildList.instance.ChangeQuantity(fID, tempIngredient.ingre1 * orderNumber);
                BuildList.instance.ChangeQuantity(sID, tempIngredient.ingre2 * orderNumber);
                BuildList.instance.ChangeQuantity(tID, tempIngredient.ingre3 * orderNumber);
                BuildList.instance.ChangeQuantity(foID, tempIngredient.ingre4 * orderNumber);
                //increase Quantity Unit
                UnitArsenal.instance.SetQuantity(index, orderNumber);
                CancelOrder();
                PopulateUnitInfo();
                PopulateResource();	
                warningText.text = "";
            }
            else
            {
                warningText.text = "Not Enough Resource Or Budget";	
            }
        }
        else
        {
            warningText.text = "Unit Is Unlimited";
        }
    }

    public void CancelOrder()
    {
        orderNumber = 0;
        orderNumberText.text = orderNumber.ToString();
    }

    public void IncreaseOrder()
    {
        orderNumber++;
        orderNumberText.text = orderNumber.ToString();
    }

    public void DecreaseOrder()
    {
        if (orderNumber > 0)
        {
            orderNumber--;
            orderNumberText.text = orderNumber.ToString();
        }
    }

    #endregion

    #region Helper Method

    void PopulateUnitInfo()
    {
        NameAndDescTxt.text = "Name: " + UnitArsenal.instance.GetNameAt(index) + '\n'
        + "Description : " + UnitArsenal.instance.GetDescriptionAt(index);	
        StatsTxt.text = "Health : " + UnitArsenal.instance.GetHPAt(index).ToString() + '\n'
        + "Fire Rate : " + UnitArsenal.instance.GetFireRateAt(index).ToString() + '\n'
        + "Movement : " + UnitArsenal.instance.GetMoveSpeedAt(index).ToString() + '\n'
        + "Damage : " + UnitArsenal.instance.GetDmgAt(index).ToString();
        if (UnitArsenal.instance.GetQuantityAt(index) >= 0)
        {
            InstockTxt.text = "INSTOCK:" + UnitArsenal.instance.GetQuantityAt(listIndexUnlocked[index]);
        }
        else
        {
            InstockTxt.text = "INSTOCK: Unlimited";
        }
    }

    void PopulateResource()
    {
        BudgetTxt.text = "Budget : " + PlayerResource.instance.GetBudget().ToString();
        Ingredient1Txt.text = BuildList.instance.NameOfFirIngreWithID(tempUID) + ": " + tempIngredient.ingre1 + "/" + BuildList.instance.QuanInstockOfFirIngreWithID(tempUID);
        Ingredient2Txt.text = BuildList.instance.NameOfSecIngreWithID(tempUID) + ": " + tempIngredient.ingre2 + "/" + BuildList.instance.QuanInstockOfSecIngreWithID(tempUID);
        Ingredient3Txt.text = BuildList.instance.NameOfThiIngreWithID(tempUID) + ": " + tempIngredient.ingre3 + "/" + BuildList.instance.QuanInstockOfThiIngreWithID(tempUID);
        Ingredient4Txt.text = BuildList.instance.NameOfFouIngreWithID(tempUID) + ": " + tempIngredient.ingre4 + "/" + BuildList.instance.QuanInstockOfFouIngreWithID(tempUID);
        UnitBdCost.text = BuildList.instance.MoneyCostID(tempUID).ToString();
    }

    void ReplaceModel()
    {
        Destroy(UnitModel);
        UnitModel = Instantiate(Resources.Load("Showing Model/" + UnitArsenal.instance.GetNameAt(index), 
                typeof(GameObject)), UnitModel.transform.localPosition, Quaternion.identity) as GameObject;
    }

    void CurrentIndexChange()
    {
        tempUID = UnitArsenal.instance.GetUnitIDAt(index);
        tempIngredient.ingre1 = BuildList.instance.QuanOfFirIngreWithID(tempUID);
        tempIngredient.ingre2 = BuildList.instance.QuanOfSecIngreWithID(tempUID);
        tempIngredient.ingre3 = BuildList.instance.QuanOfThiIngreWithID(tempUID);
        tempIngredient.ingre4 = BuildList.instance.QuanOfFouIngreWithID(tempUID);
    }

    #endregion

}
