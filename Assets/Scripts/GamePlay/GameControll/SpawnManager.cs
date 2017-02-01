using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class SpawnManager : MonoBehaviour
{
    //Declaring data members

    #region member

    //--Private


    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string conn;
    string sqlQuery;

    //--Public
    private struct UnitInfo
    {

        public string name;
        public string type;
        public int dmg;
        public int hp;
        public float fireRate;
        public float moveSpeed;
        public int cost;
    }

    private List<UnitInfo> squadListInfo;

    public Transform rightEndPosition;
    public Transform leftEndPosition;
    public Transform groundSpawnPosition;
    private Transform destination;

    private static SpawnManager _instance;

    public static SpawnManager instance
    {
        get { return _instance; }
    }

    public 

    #endregion


    #region built-in Method

    //--Implemented Methods

    void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        squadListInfo = new List<UnitInfo>();
        conn = "URI=file:Assets/Plugin/KJData.s3db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        RetrieveInfoFromDataBase();
	
    }

    #endregion

    #region getter and setter

    public int GetCostOfUnit(int index)
    {
        return(squadListInfo[index].cost);
    }

    public string GetNameOfUnit(int index)
    {
        return(squadListInfo[index].name);
    }

    #endregion

    #region Main Method Relate to Unit

    void RetrieveInfoFromDataBase()
    {
        //squadListInfo.Clear();
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        sqlQuery = "SELECT name,DMG,HP,FR,MV,UnitBuildCost,Type FROM Unit where UID = " + SquadManager.instance.GetUnit1IDOfSqd(0);
        ExecuteQuerry();
        sqlQuery = "SELECT name,DMG,HP,FR,MV,UnitBuildCost,Type FROM Unit where UID = " + SquadManager.instance.GetUnit2IDOfSqd(0);
        ExecuteQuerry();
        sqlQuery = "SELECT name,DMG,HP,FR,MV,UnitBuildCost,Type FROM Unit where UID = " + SquadManager.instance.GetUnit3IDOfSqd(0);
        ExecuteQuerry();
        sqlQuery = "SELECT name,DMG,HP,FR,MV,UnitBuildCost,Type FROM Unit where UID = " + SquadManager.instance.GetUnit4IDOfSqd(0);
        ExecuteQuerry();
        dbconn.Close();
    }

    private void ExecuteQuerry()
    {
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        reader.Read();
        UnitInfo temp = new UnitInfo();

        temp.name = reader.GetString(0);
        temp.type = reader.GetString(6);
        temp.dmg = reader.GetInt16(1);
        temp.hp = reader.GetInt16(2);
        temp.fireRate = reader.GetFloat(3);
        temp.moveSpeed = reader.GetFloat(4);
        temp.cost = reader.GetInt16(5);

        squadListInfo.Add(temp);

    }

    public GameObject SpawnUnit(int index, Transform newDestination)
    {
        destination = newDestination;
        Vector3 spawnPoint;
        GameObject spawnedUnit;
        if (squadListInfo[index].type == "AIR")
            spawnPoint = leftEndPosition.position;
        else
            spawnPoint = groundSpawnPosition.position;


        spawnedUnit = Instantiate(Resources.Load("GamePlayUnit/" + squadListInfo[index].name), spawnPoint, Quaternion.identity) as GameObject;
        spawnedUnit.transform.GetComponent<PlayerUnit>().Set_UnitInfo(squadListInfo[index].dmg, squadListInfo[index].hp
		                                                              , squadListInfo[index].moveSpeed, squadListInfo[index].fireRate, squadListInfo[index].cost);

        CombatSceneManager.instance.AddUnitToList(spawnedUnit);

        StartCoroutine(MoveToLocation(spawnedUnit));
        return spawnedUnit;
    }

    #endregion

    #region Relate to Golem

    public void SpawnGolem(int golemID)
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        sqlQuery = "SELECT * FROM Monster where monsID = " + golemID;
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        reader.Read();

        GameObject tempGolem = Instantiate(Resources.Load("Monster/" + reader.GetString(1)), rightEndPosition.position, Quaternion.identity) as GameObject;
        Golem golemInfo = tempGolem.GetComponent<Golem>();
        golemInfo.name = reader.GetString(1);
        golemInfo.Golem_MoveSpeed = reader.GetFloat(2);
        //golemInfo.sp = reader.GetFloat(2);
        golemInfo.Golem_MaxHealth = reader.GetInt16(5);
        golemInfo.Golem_CurrentHealth = reader.GetInt16(4);
        golemInfo.Golem_Hit_Threshold = reader.GetInt16(15);
        //tempGolem.GetComponent<Golem>().GetType = UnitType.GOLEM;
        golemInfo.Golem_Defend_Duration = reader.GetFloat(17);
        golemInfo.Golem_Sight_Range = reader.GetFloat(18);
        golemInfo.Golem_Stun_Duration = reader.GetFloat(19);
        dbconn.Close();
        CombatSceneManager.instance.AddGolemToList(tempGolem);
	

    }


    #endregion

    #region coroutine

    //Method -- CoroutineForMove
    //Parameters: Vector3, GameObject
    /*
	  	This method will start the couroutine: MoveToLocation
	*/


    IEnumerator MoveToLocation(GameObject newUnit)
    {
        Vector3 goal = destination.position;
        while (Mathf.Abs(newUnit.transform.position.x - goal.x) >= 0.5f)
        {  
            if (newUnit.GetComponent<PlayerUnit>().Unit_ObjectPhysics != null)
            {
                newUnit.GetComponent<PlayerUnit>().Move(1);		
            }		
            yield return null;
        }
    }

    #endregion
}