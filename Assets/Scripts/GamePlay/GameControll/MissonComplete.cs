using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;
public class MissonComplete : MonoBehaviour {

	public GameObject missionComp;
	public Kaiju_Health enemyRef;
	public CombatSceneManager gman;

	public Text TextTimerTxt;
	public Text UnitLostTxt;
	public Text MoneyTxt;
	public Text partsTxt;
	public int unitDcount;//increases whenever unit got destroyed.
	private Animator anim;
	int partsrandom;
	int moneyEarned;
	public int Reward;
	float Timer;
	int KJHP;

	int prevMoney;
	string conn; 
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;

	// Use this for initialization
	void Start () {
		Timer=0.0f;
		unitDcount = 0;
		moneyEarned = 0;
		partsrandom = Random.Range (1, 10);//make an random value at first place.
		anim = missionComp.GetComponent<Animator> ();
		anim.enabled = false;
		Reward = 5000;
	}
	
	// Update is called once per frame
	void Update () {
//		unitDcount = gman.GameManager_UnitDestroyed;
//
//		// timer to count how long the combat.

//
//
//		if (enemyRef.currentHealth <= 0) //when kaiju dies.
//		{
			
//
//		}
	}



	public void addReward()
	{
		conn= "URI=file: Assets/Plugin/KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();//open connection to db
		
		dbcmd = dbconn.CreateCommand ();
		string sqlQuery= "SELECT money FROM Money";
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader();
		while (reader.Read())
		{
			prevMoney = reader.GetInt32 (0);
		}
		//Debug.Log ("prevMoney: " + prevMoney); 
		sqlQuery = "UPDATE MONEY SET MONEY = " + (prevMoney+moneyEarned);
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader ();
		dbcmd = null;
		dbconn.Close ();
		dbconn = null;
	}
}
