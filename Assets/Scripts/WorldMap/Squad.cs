using UnityEngine;
using System.Collections;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class Squad : MonoBehaviour {


	#region Data Members
	
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;
	string conn;
	
	public float squadHealth;
	private Vector3 distance;
	private float sqDistance;

	public bool moving;
	public float squadSpeed;
	public int squadID = 1;
	public CityNode origin;
	public CityNode destination;
	
	#endregion

	#region built-in method
	// Use this for initialization
	void Start () {
		conn = "URI=file:Assets/Plugin/KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection(conn);
		moving = false;
		sqDistance = 0.0005f;	
	}
	
	// Update is called once per frame
	void Update () {
		//Move speed is determined by this unit's Move Speed stat
		if (moving ) {
			if(squadSpeed!= 0.0f )	{
				gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position,
				                                                    destination.transform.localPosition,
				                                                    squadSpeed * Time.deltaTime);	
				distance = destination.transform.localPosition - gameObject.transform.position;
				if (Vector3.SqrMagnitude(distance) < sqDistance)	{
					origin = destination;
					Debug.Log(destination);
					moving = false;
				}
			}
			else{
				SetSquadSpeed();
			}
			
		}
		
	}
	#endregion

	#region main method
	public void MoveSquad(CityNode position)	{
		if (!moving || position == origin) {
			if (position == origin) {
				origin = destination;
			}
			destination = position;
			moving = true;	
		}
		if (squadSpeed == null) {
			SetSquadSpeed();
		}
	}
	// go to database and load the squad speed
	void AddSquadSpeed(int UnitID)
	{
		dbconn.Open();
		dbcmd = dbconn.CreateCommand();
		string sqlQuery = "SELECT MV,Type FROM Unit WHERE UID = " + UnitID;
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader();
		while(reader.Read())
		{
			//temp.SquadID = reader.GetInt32(0);
			if (reader.GetString(1) == "TANK") {
				squadSpeed += reader.GetInt32(0) * 0.003f ;	
			}
			else if (reader.GetString(1) == "ARTILLERY") {
				squadSpeed += reader.GetInt32(0) * 0.002f ;	
			}
			else if (reader.GetString(1) == "AIRSHIP") {
				squadSpeed += reader.GetInt32(0) * 0.0015f ;	
			}
			else if (reader.GetString(1) == "INFANTRY") {
				squadSpeed += reader.GetInt32(0) * 0.003f ;	
			}
		}
		dbconn.Close();
	}
	#endregion

	#region get and set
	public void SetSquadID(int id)
	{
		squadID = id;
	}
	public void SetOriginCity(CityNode originCity)
	{
		origin = originCity;
	}
	// add speed of squad base on each member of squad.
	void SetSquadSpeed()
	{
		squadSpeed= 0.0f;
		AddSquadSpeed(SquadManager.instance.GetUnit1IDOfSqd(squadID));
		AddSquadSpeed(SquadManager.instance.GetUnit2IDOfSqd(squadID));
		AddSquadSpeed(SquadManager.instance.GetUnit3IDOfSqd(squadID));
		AddSquadSpeed(SquadManager.instance.GetUnit4IDOfSqd(squadID));
	}
	#endregion
}
