using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine.SceneManagement;

public class WorkShopRDMan : MonoBehaviour
{
    #region Data Members

    private List<int> listIndexOfUnlocked;
    int index = 0;
    // temporary use as cost to upgrade till we got the right one.
    int cost = 1200;
    int playerBudget = 0;
    // temporary value if upgrade
    int tempUnitDmg = 0;
    int tempUnitHP = 0;
    float tempUnitMV = 0.0f;
    float tempUnitFR = 0.0f;
    //int tempCost = 0;
    bool confirmUpgrade = true;
    //Unit Stats
    public Slider L_Dmg;
    public Slider L_HP;
    public Slider L_MV;
    public Slider L_FR;
    public Text Description;
    public Text UnitName;
    public Text Ingre1;
    public Text Ingre2;
    public Text Ingre3;
    public Text Ingre4;

    public List<Transform> listUpgBtn;

    // Icon
    public GameObject UnitModel;

    #endregion

    #region Setters & Getters

    public int CurrentUnitIndex
    {
     
        set
        {
            index = value; 
            index %= index %= listIndexOfUnlocked.Count;
            //This method is called in order to populate menu with 
            //information about the current unit in the list.
            SetUnitInfo();
            ReplaceModel();
            PopulateUnitStats();
        
        }
    }

    #endregion

    #region Built-in Unity Methods

    // Use this for initialization //we read the data in here.
    void Start()
    {
        listUpgBtn = new List<Transform>();
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            listUpgBtn.Add(transform.GetChild(0).GetChild(i));
        }
        listIndexOfUnlocked = UnitArsenal.instance.GetListIndexOfUnlockUnit();
        index = listIndexOfUnlocked[0];
        ReplaceModel();
        SetUnitInfo();
    }

    #endregion

    #region Public Methods

    /// Upgrades the tank max HP Call by button.
    public void upgradeArmor()
    {
        if (confirmUpgrade && UnitArsenal.instance.GetLvHPAt(index) <= 10 &&
            PlayerResource.instance.GetBudget() > cost)
        {
            L_HP.value++;
            tempUnitHP += 2;
            Ingre2.text = "Cost : " + cost;
            //tempCost = cost;
            confirmUpgrade = false;
        }
    }

    /// Upgrades the tank damage.
    public void upgradePower()
    {
        if (confirmUpgrade && UnitArsenal.instance.GetLvDmgAt(index) <= 10 &&
            PlayerResource.instance.GetBudget() > cost)
        {
            L_Dmg.value++;
            tempUnitHP += 2;
            //tempCost = cost;
            Ingre2.text = "Cost : " + cost;
            confirmUpgrade = false;
        }
    }

    /// Upgrades the tank firerate cooldown.
    public void upgradeFireRate()
    {
        if (confirmUpgrade && UnitArsenal.instance.GetLvFireRateAt(index) <= 10 &&
            PlayerResource.instance.GetBudget() > cost)
        {
            L_FR.value++;
            tempUnitHP += 2;
            //tempCost = cost;
            Ingre2.text = "Cost : " + cost;
            confirmUpgrade = false;
        }
    }

    /// Upgrades the tank sp.
    public void upgradeMovement()
    {
        if (confirmUpgrade && UnitArsenal.instance.GetLvMoveSpeedAt(index) <= 10 &&
            PlayerResource.instance.GetBudget() > cost)
        {
            L_MV.value++;
            tempUnitHP += 2;
            //tempCost = cost;
            Ingre2.text = "Cost : " + cost;
            confirmUpgrade = false;
        }
    }

    #endregion

    #region Private Methods

    /// =========================Public Functin Call by button or outside===============

    void UpgBtn_Update(Transform upgButn, int maxValue, int currentValue, string upgType)
    {
        upgButn.GetChild(0).GetComponentInChildren<Text>().text = upgType + " : " + currentValue.ToString();
        upgButn.GetChild(0).GetComponentInChildren<Text>().color = Color.black;
        upgButn.GetChild(1).GetComponent<Image>().fillAmount = (float)currentValue / (float)maxValue;
    }

    #region changeUnitFrame button area

    /// Confirm this instance. Call by confirm button, check if have enough money. If have enough, 
    /// save new data, else, set back to initiate 
    /// //===================
    public void confirm()
    {
        if (!confirmUpgrade)
        {
            UnitArsenal.instance.UpdateUnitStats(tempUnitHP, tempUnitDmg, tempUnitMV, tempUnitFR,
                Convert.ToInt32(L_HP.value), Convert.ToInt32(L_Dmg.value), Convert.ToInt32(L_MV.value), Convert.ToInt32(L_FR.value), index);    
            PlayerResource.instance.UpdateValue(playerBudget - cost);
            Ingre1.text = "Budget : " + playerBudget.ToString();
            confirmUpgrade = true;
            Ingre2.text = "Cost : 0";
        }
    }

    /// Determines whether this instance cancel. was call by button  .
    /// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
    public void Cancel()
    {
        PopulateUnitStats();
    }

    #endregion

    //============================Helper Fucntion Call by other funcion in class===============================================
    /// Sets the info back. call by other function, when not enough money or cancel

    // call to populate info 
    private void SetUnitInfo()
    {
        UnitName.text = UnitArsenal.instance.GetNameAt(index);
        Description.text = UnitArsenal.instance.GetDescriptionAt(index);
        // Unit Stats and Stats Level

    }

    private void PopulateUnitStats()
    {
        confirmUpgrade = true;
        tempUnitDmg = UnitArsenal.instance.GetDmgAt(index);
        tempUnitHP = UnitArsenal.instance.GetHPAt(index);
        tempUnitMV = UnitArsenal.instance.GetMoveSpeedAt(index);
        tempUnitFR = UnitArsenal.instance.GetFireRateAt(index);

        L_FR.value = UnitArsenal.instance.GetLvFireRateAt(index);
        L_HP.value = UnitArsenal.instance.GetLvHPAt(index);
        L_MV.value = UnitArsenal.instance.GetLvMoveSpeedAt(index);
        L_Dmg.value = UnitArsenal.instance.GetLvDmgAt(index);

        Ingre2.text = "Cost : 0";
    }

    private void PopulateIngredient()
    {
        playerBudget = PlayerResource.instance.GetBudget();
        Ingre1.text = "Budget : " + playerBudget.ToString();
    }

    void ReplaceModel()
    {
        Destroy(UnitModel);
        UnitModel = Instantiate(Resources.Load("Showing Model/" + UnitArsenal.instance.GetNameAt(index), 
                typeof(GameObject)), UnitModel.transform.localPosition, Quaternion.identity) as GameObject;
    }

    #endregion


    #region Reference By Editor


    public void UpgBtn_Selected(int btnID)
    {
        Transform upgButn = listUpgBtn[btnID];
        upgButn.GetChild(0).GetComponentInChildren<Text>().color = Color.red;


    }


  
    #region ChangeScene

    public  void _ToWorldMap()
    {
        SceneManager.LoadScene("WorldMap");
    }

    public void _ToMenu()
    {
        SceneManager.LoadScene("Menu Scene");
    }


    #endregion

    #endregion
  
}

