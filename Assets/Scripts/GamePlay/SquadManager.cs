using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine.SceneManagement;

public class SquadManager : MonoBehaviour
{

    #region Data Members

    private static SquadManager _instance;
    //Data members for linking with our database
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string conn;
    private bool sceneChange;

   

    public int newSquadID;
    private List<SquadStruct> listSquadInfo;
    private int curretnSquad;

    #endregion

    #region Getter and Setter

    public SquadStruct CurrentSquad
    {
        get{ return  listSquadInfo[curretnSquad]; }

    }

    #endregion

    #region built-in unity method

    void Awake()
    {
        //_instance = this;
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static SquadManager instance
    {
        get { return _instance; }
    }

    // Use this for initialization
    void Start()
    {
        // set connection string and open the conenction
        listSquadInfo = new List<SquadStruct>();

        conn = Application.dataPath + "/StreamingAssets/KJData.s3db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        LoadFromDataBase();
        sceneChange = false;
    }

    void OnLevelWasLoaded()
    {
        if (sceneChange)
        {
            
            if (SceneManager.GetActiveScene().name == "WorldMap")
            {
                PopulateSquad();
            }
        }
    }

    #endregion

    #region Main Method

    void PopulateSquad()
    {
        for (int i = 0; i < listSquadInfo.Count; i++)
        {
            GameObject newSquad = Instantiate(Resources.Load("WorldMap/PlayerSquad"), listSquadInfo[i].position, Quaternion.identity) as GameObject;	
            newSquad.GetComponent<Squad>().squadHealth = listSquadInfo[i].squadHealth;
            newSquad.GetComponent<Squad>().squadID = listSquadInfo[i].squadID;
        }
    }

    void LoadFromDataBase()
    {
        listSquadInfo.Clear();
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM CurrentMissionSquadSetup";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            SquadStruct temp = new SquadStruct();
            temp.squadID = reader.GetInt32(0);
            temp.unit1ID = reader.GetInt32(1);
            temp.unit2ID = reader.GetInt32(2);
            temp.unit3ID = reader.GetInt32(3);
            temp.unit4ID = reader.GetInt32(4);
            temp.position.x = reader.GetFloat(5);
            temp.position.y = reader.GetFloat(6);
            temp.position.z = reader.GetFloat(7);
            temp.squadHealth = reader.GetFloat(8);
            listSquadInfo.Add(temp);
        }
        dbconn.Close();
    }

    public void AddSquad(int SquU1, int SquU2, int SquU3, int SquU4)
    {
        string sqlQuery = "insert into CurrentMissionSquadSetup (deployID,Unit1,Unit2,Unit3,Unit4) Values ("
                          + newSquadID + "," + SquU1 + "," + SquU2 + "," + SquU3 + "," + SquU4 + ")";
        ExecuteUpdateQuerry(sqlQuery);
        LoadFromDataBase();
    }

    void RemoveSquad(int SqdID)
    {
        string sqlQuery = "delete from CurrentMissionSquadSetup where deployID = " + SqdID.ToString();
        ExecuteUpdateQuerry(sqlQuery);
    }

    public bool CheckSquadIDExist(int SqudID)
    {
        bool result = false;
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "select exists(select 1 from CurrentMissionSquadSetup where deployID = " + SqudID + " Limit 1)";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            result = reader.GetBoolean(0);
        }
        return result;
    }


    #endregion

    #region helper and get set method

    void ExecuteUpdateQuerry(string sqlQuery)
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        dbcmd.CommandText = sqlQuery; 
        dbcmd.ExecuteNonQuery();
        dbconn.Close();
    }

    //===========Setter and Getter
    public int GetUnit1IDOfSqd(int sqID)
    {
        return listSquadInfo[FindIndex(sqID)].unit1ID;
    }

    public int GetUnit2IDOfSqd(int sqID)
    {
        return listSquadInfo[FindIndex(sqID)].unit2ID;
    }

    public int GetUnit3IDOfSqd(int sqID)
    {
        return listSquadInfo[FindIndex(sqID)].unit3ID;
    }

    public int GetUnit4IDOfSqd(int sqID)
    {
        return listSquadInfo[FindIndex(sqID)].unit4ID;
    }

    public void SetUnit1IDofSqd(int SqID, int UnitID)
    {
        listSquadInfo[FindIndex(SqID)].unit1ID.Equals(UnitID);
    }

    public void SetUnit2IDofSqd(int SqID, int UnitID)
    {
        listSquadInfo[FindIndex(SqID)].unit2ID.Equals(UnitID);
    }

    public void SetUnit3IDofSqd(int SqID, int UnitID)
    {
        listSquadInfo[FindIndex(SqID)].unit3ID.Equals(UnitID);
    }

    public void SetUnit4IDofSqd(int SqID, int UnitID)
    {
        listSquadInfo[FindIndex(SqID)].unit4ID.Equals(UnitID);
    }

    //=============================================================================================
    int FindIndex(int SquadID)
    {
        return listSquadInfo.FindIndex(squad => squad.squadID == SquadID);
    }

    #endregion
}

public struct SquadStruct
{
    public Vector3 position;
    public float squadHealth;
    public int squadID;
    public int unit1ID;
    public int unit2ID;
    public int unit3ID;
    public int unit4ID;
}