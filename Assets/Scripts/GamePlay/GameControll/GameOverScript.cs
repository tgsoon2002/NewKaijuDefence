using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class GameOverScript : MonoBehaviour {

	// for database
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;
	string conn;
	string sqlQuery;
	public CombatSceneManager gameManger;

	// Use this for initialization
	public Animator animLose;
	public Animator animWin;
	float Timer;
	bool gameOver = false;

	public Text TextTimerTxt;
	public Text UnitLostTxt;
	public Text MoneyTxt;
	public Text partsTxt;
	int tempQuantItem = 0;

	struct reward{
		public int ResID;
		public int ResQuantity;

	}
 	List<reward> endGameSalvage;
	int partsrandom;
	int moneyEarned;
	int Reward;

	int KJHP;

	#region built-in Method

	void Start () {
		conn = "URI=file:Assets/Plugin/KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection(conn);
		endGameSalvage = new List<reward>();
		Timer=0.0f;
		moneyEarned = 0;
		partsrandom = Random.Range(1,100);//make an random value at first place.
		Reward = 5000;
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameOver) 
		{
			Timer += Time.deltaTime;
		}



	}
	#endregion
	#region helper Method

//	public void WinOrLose(){
//		gameOver = true;
//
//		if(CombatSceneManager.instance.winOrLose){
//			Debug.Log("Winbattle");
//			MissionComplete();
//
//		}
//		else{
//			Debug.Log("lose battle");
//			MissionFail();
//		}
//
//
//	}

	#endregion

	public void CombatWin(int unitDestroyed){

		animWin.SetBool ("GameOver",true);

		// this for timer.
		TextTimerTxt.text=Mathf.RoundToInt(Timer).ToString ();

		//this for unit lost
		UnitLostTxt.text= unitDestroyed.ToString();

		// this for money
		moneyEarned = PauseMenuScript.instance.PauseMenu_WarFund;
		foreach (var item in CombatSceneManager.instance.listOfUnits) {
			moneyEarned += item.GetComponent<PlayerUnit>().UnitCost / 2;}
		MoneyTxt.text=moneyEarned.ToString ();

		// use this to get part number.
		sqlQuery = "select RareRate,DropQuantity,ResName,ResID,Quantity from Resource where ResID IN (select ResouceID from monsterDrop where MID =0 )";
		ExecuteWinQuerry(sqlQuery);
	
		// save the item to db.
		foreach (var item in endGameSalvage) {
			sqlQuery = "UPDATE Resource SET Quantity =" + item.ResQuantity + " where ResID = " + item.ResID;
			ExecuteUpdateQuerry(sqlQuery);
		}

	}

	public void CombatLose(){
		animLose.SetBool ("GameOver", true);
	}
	
	#region helper Function
	private void ExecuteWinQuerry(string querry){
		dbconn.Open();
		dbcmd = dbconn.CreateCommand();
		dbcmd.CommandText = querry;
		reader = dbcmd.ExecuteReader();
		while (reader.Read()){
			if (Random.Range(1,100) < reader.GetInt16(0)) {
				tempQuantItem = Random.Range(1,reader.GetInt16(1));
				if (tempQuantItem !=0) {
					partsTxt.text += reader.GetString(2) + " " + tempQuantItem + " ,";
					int newQuantItem = reader.GetInt16(4) + tempQuantItem ;
					reward tempReward = new reward();
					tempReward.ResID = reader.GetInt16(3);
					tempReward.ResQuantity = newQuantItem;
					endGameSalvage.Add(tempReward);
				}
			}
			tempQuantItem = 0;
			//tempQuantItem = reader.GetInt16(0);
		}
		dbconn.Close();
	}

	void ExecuteUpdateQuerry(string sqlQuery){
		dbconn.Open ();
		dbcmd = dbconn.CreateCommand ();
		dbcmd.CommandText = sqlQuery; 
		dbcmd.ExecuteNonQuery ();
		dbconn.Close ();
	}

	#endregion
}
