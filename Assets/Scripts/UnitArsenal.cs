using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;


public class UnitArsenal : MonoBehaviour
{
    #region Data Member

    //Data members for linking with our database
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string conn;

    private struct UnitInfo
    {
        public string name;
        public int unitID;
        public int dmg;
        public int HP;
        public float FR;
        public float MV;
        public int ComdPointCost;
        public int LDmg;
        public int LHP;
        public int LFR;
        public int LMV;
        public string description;
        public string type;
        public int quantity;
        public int bdCost;
        public bool unlocked;

    }

    public static UnitArsenal instance
    {
        get { return _instance; }
    }

    #endregion

    #region Built-in Methods

    //Declaring local data members

    private List<UnitInfo> listUnitInfo;
    private static UnitArsenal _instance;

    //Member Methods:

    void Awake()
    {
        _instance = this;
    }

    // Use this for initialization
    void Start()
    {
        listUnitInfo = new List<UnitInfo>();
        conn = "URI=file:Assets/Plugin/KJData.s3db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbcmd = dbconn.CreateCommand();
        // set connection string and open the conenction
        RetriveNewValue();
    }

    #endregion

    #region helper Method

    //Method: SetValue
    //Purpose -- This method parses the specified table in our
    //database and sets it to the members inside our UnitInfo
    //struct. Upon parsing, it then appends the instantiated
    //struct objects into our list of UnitInfo objects. Only
    //units the player has unlocked will be the only data
    //read by this SetValue
    private void SetValue()
    {
        UnitInfo temp = new UnitInfo();
        temp.unitID = reader.GetInt32(0);
        temp.name = reader.GetString(1);
        temp.dmg = reader.GetInt32(2);
        temp.HP = reader.GetInt32(3);
        temp.FR = reader.GetFloat(4);
        temp.MV = reader.GetFloat(5);
        //temp.ComdPointCost = reader.GetInt32(6);
        temp.LDmg = reader.GetInt32(6);
        temp.LHP = reader.GetInt32(7);
        temp.LFR = reader.GetInt32(8);
        temp.LMV = reader.GetInt32(9);
        temp.description = reader.GetString(10);
        temp.type = reader.GetString(11);
        temp.quantity = reader.GetInt32(12);
        temp.bdCost = reader.GetInt32(13);
        temp.unlocked = reader.GetBoolean(14);

        listUnitInfo.Add(temp);
    }


    void ExecuteUpdateQuerry(string sqlQuery)
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
    }

    #endregion

    #region getter and setter

    //added 3.20.15 for database

    public int GetInfoListCount()
    {
        return listUnitInfo.Count;
    }

    public int GetUnitIDAt(int i)
    {
        return listUnitInfo[i].unitID;
    }

    public string GetNameAt(int i)
    {
        return listUnitInfo[i].name;
    }

    public string GetDescriptionAt(int i)
    {
        return listUnitInfo[i].description;
    }

    public string GetTypeAt(int i)
    {
        return listUnitInfo[i].type;
    }
    // ============Unit Upgrade Cost (This will change in future.)============

    public int GetUnitCostAt(int i)
    {
        return listUnitInfo[i].ComdPointCost;
    }
    // ============Unit Stats and Stats Level============
  
    public int GetDmgAt(int i)
    {
        return listUnitInfo[i].dmg;
    }

    public int GetHPAt(int i)
    {
        return listUnitInfo[i].HP;
    }

    public float GetMoveSpeedAt(int i)
    {
        return listUnitInfo[i].MV;
    }

    public float GetFireRateAt(int i)
    {
        return listUnitInfo[i].FR;
    }
    //--------------------------------
    public int GetLvDmgAt(int i)
    {
        return listUnitInfo[i].LDmg;
    }

    public int GetLvHPAt(int i)
    {
        return listUnitInfo[i].LHP;
    }

    public int GetLvMoveSpeedAt(int i)
    {
        return listUnitInfo[i].LMV;
    }

    public int GetLvFireRateAt(int i)
    {
        return listUnitInfo[i].LFR;
    }
    // ============build cost, and ingredient============
    public int GetQuantityAt(int i)
    {
        return listUnitInfo[i].quantity;
    }

    public int GetBdCostAt(int i)
    {
        return listUnitInfo[i].bdCost;
    }

    #endregion


    #region Data-base Relate Method

    // This method was call when changeing the quantity of unit instock
    public void SetQuantity(int index, int different)
    {
        // only increase if number of unit is or equal 0, Not add if unit is unlimit(-1 mean unlimit)
        if ((different < 0 && listUnitInfo[index].quantity > 0) || (different > 0 && listUnitInfo[index].quantity >= 0))
        {
            string sqlQuery = "UPDATE Unit SET Qty = " + (listUnitInfo[index].quantity + different).ToString() + " where UID = " + GetUnitIDAt(index);
            Debug.Log(sqlQuery);
            ExecuteUpdateQuerry(sqlQuery);
            RetriveNewValue();
        }
    }

    // This method use to update the static value of unit, Mostly in upgrade Scene
    public void UpdateUnitStats(int HP, int PW, float MV, float FR, int LHP, int LPW, int LMV, int LFR, int index)
    {
        string sqlQuery = "UPDATE Unit SET HP = " + HP.ToString() + " , MV = " + MV.ToString() + " , DMG = " + PW.ToString() + " , FR = " + FR.ToString() +
                          " , LHP = " + LHP.ToString() + " , LMV = " + LMV.ToString() + " , LDMG = " + LPW.ToString() + " , LFR = " + LFR.ToString() +
                          " where UID = " + GetUnitIDAt(index);
        ExecuteUpdateQuerry(sqlQuery);
        RetriveNewValue();
    }

    // Method Retrive new list of unit.
    void RetriveNewValue()
    {
        listUnitInfo.Clear();
        dbconn.Open();
        string sqlQuery = "SELECT * FROM Unit where Unlocked = 1";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();		
        while (reader.Read())
        {
            SetValue();
        }
    }

    #endregion

    #region search method

    int FindIndex(int id)
    {
        return listUnitInfo.FindIndex(unit => unit.unitID == id);
    }

    bool CheckUnlocked(int id)
    {
        return listUnitInfo[id].unlocked;
    }

    public List<int> GetListIndexOfUnlockUnit()
    {
        List<int> temp = new List<int>();
        for (int i = 0; i < listUnitInfo.Count; i++)
        {
            if (listUnitInfo[i].unlocked)
            {
                temp.Add(i);
            }
        }
        return temp;
    }

    #endregion
}
