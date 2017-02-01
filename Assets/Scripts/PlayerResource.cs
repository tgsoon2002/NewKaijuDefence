using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

/*
 * Function:
 * Pb- GetBudget    return int budget
 * Pb- UpdateValue  Create Querry to update the budget of player in db
 * Pr- ExecuteUpdateQuerry    Exercute the querry 
 * Pr- RetriveNewValue       Get data from DB to set the value of player budget
 * */
public class PlayerResource : MonoBehaviour
{

    private static PlayerResource _instance;
    //Data members for linking with our database
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string conn;


    public struct Resource
    {
        public int resID;
        public string resName;
        public int resQuantity;
    }

    private List<Resource> listOfResource;



    public static PlayerResource instance
    {
        get { return _instance; }
    }

    #region built-in method

    // ==========================================
    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        conn = "URI=file:Assets/Plugin/KJData.s3db"; //commented application datapath, added assets/plugin/ 4,13,15
        dbconn = (IDbConnection)new SqliteConnection(conn);
	
        listOfResource = new List<Resource>();
        RetriveNewValue();
    }

    #endregion

    #region getter and setter

    public int GetBudget()
    {
        return listOfResource[FindIndex(0)].resQuantity;
    }

    public string NameOfResWithID(int ResID)
    {
        return listOfResource[FindIndex(ResID)].resName;
    }

    public int QuanOfResWithID(int ResID)
    {
        return listOfResource[FindIndex(ResID)].resQuantity;
    }


    #endregion

    #region Main Method

    public bool EnoughRes(int resID, int req)
    {
        return (listOfResource[FindIndex(resID)].resQuantity >= req);
    }

    public void ChangeQuantity(int resID, int newQuan)
    {
        int temp = QuanOfResWithID(resID) - newQuan;
        string sqlQuery = "UPDATE Resource SET Quantity = " + temp.ToString() + " Where ResID = " + resID;
        ExecuteUpdateQuerry(sqlQuery);
        RetriveNewValue();

    }

    public void UpdateValue(int newValue)
    {
        string sqlQuery = "UPDATE Resource SET Quantity = " + (newValue).ToString() + " Where ResName = \"Money\"";
        ExecuteUpdateQuerry(sqlQuery);
        RetriveNewValue();
    }

    #endregion

    #region helper Method

    void ExecuteUpdateQuerry(string sqlQuery)
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = sqlQuery; 
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
    }

    void RetriveNewValue()
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT ResID,ResName,Quantity FROM Resource";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
		
        listOfResource.Clear();
        while (reader.Read())
        {
            Resource temp = new Resource();
            temp.resID = reader.GetInt32(0);
            temp.resName = reader.GetString(1);
            temp.resQuantity = reader.GetInt32(2);
            listOfResource.Add(temp);
        }
        dbconn.Close();
    }

    int FindIndex(int id)
    {
        return listOfResource.FindIndex(resourse => resourse.resID == id);
    }

    #endregion
}
