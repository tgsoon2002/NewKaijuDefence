using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;
public class UpgradeScript : MonoBehaviour {

	int money;
	int totalcost;
	string UnitType = "Tank";
	// current value 
	float tankMaxHP;
	float tankDam;
	float tankCD;
	float tankSp;
	// current value cost
	int tankMaxHPCst = 900;
	int tankDamCst = 1000;
	int tankCDCst = 800;
	int tankSpCst = 600;
	// initial value
	float initialtankMaxHP;
	float initialtankDam;
	float initialtankCD;
	float initialtankSp;
	// initial cost of upgrade
	int itankMaxHPC;
	int itankDamC ;
	int itankCDC;
	int itankSpC;

	string conn; 
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;

	public Text TxtFireRate;
	public Text TxtDamage;
	public Text TxtArmor;
	public Text TxtMovement;
	public Text TxtMoney;
	public Text TxtCost;
	// Use this for initialization //we read the data in here.
	void Start () {
		conn= "URI=file:" + Application.dataPath + "KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection (conn);
		dbconn.Open ();//open connection to db

		dbcmd = dbconn.CreateCommand ();
		string sqlQuery = "SELECT name,Damage,Armor,Firerate,Movement FROM KJData";
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader ();
	

		while (reader.Read ()) 
		{
			setValue(UnitType);
		}
		// Seleect Money from money table
		sqlQuery = "SELECT money FROM Money";
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader();
		while (reader.Read()) 
		{
			money=reader.GetInt32(0);
		}
		TxtMoney.text = "Money: " + money;
		dbconn.Close ();
	}
	/// =========================Public Functin Call by button or outside===============
	/// Upgrades the tank max HP Call by button.
	public void upgradeTankMaxHP() 
	{
		tankMaxHP += 2;
		totalcost += tankMaxHPCst;
		TxtArmor.text = tankMaxHP.ToString();
		TxtCost.text = "Cost: " + totalcost;

	}
	/// Upgrades the tank damage.
	public void upgradeTankDam()
	{
		tankDam += 1;
		totalcost += tankDamCst; //1000 as increased cost
		TxtDamage.text = tankDam.ToString();
		TxtCost.text = "Cost: " + totalcost;
	}

	/// Upgrades the tank firerate cooldown.
	public void upgradeTankCD()
	{
		tankCD -= 0.1f;
		totalcost += tankCDCst;// 500 as increased cost.
		TxtFireRate.text = tankCD.ToString();
		TxtCost.text = "Cost: " + totalcost;
	}
	/// Upgrades the tank sp.
	public void upgradeTankSp()
	{
		tankSp += 1;
		totalcost += tankSpCst; //500 as increase cost.
		TxtMovement.text = tankSp.ToString();
		TxtCost.text = "Cost: " + totalcost;
	}
	/// Confirm this instance. Call by confirm button, check if have enough money. If have enough, 
	/// save new data, else, set back to initiate 
	public void confirm()
	{
		if ((money - totalcost) >= 0)
		{
			string sqlQuery = "UPDATE KJData SET Damage = " + tankDam + 
				", Armor = " + tankMaxHP + 
				", Firerate = " + tankCD +
				", Movement = " + tankSp + 
				"WHERE NAME = \'" + UnitType + "\'";

			executeUpdateQuerry( sqlQuery);

			sqlQuery = "UPDATE MONEY SET MONEY = " + (money - totalcost);//subtrack total cost from money.
			executeUpdateQuerry( sqlQuery);
			dbcmd = null;
			dbconn.Close ();
			dbconn = null;
		} 
		else 
		{
			setInfoBack();
		}
	}
	/// Determines whether this instance cancel. was call by button  .
	/// <returns><c>true</c> if this instance cancel ; otherwise, <c>false</c>.</returns>
	public void Cancel()
	{
		setInfoBack();
	}
	//============================Helper Fucntion Call by other funcion in class===============================================
	/// Sets the info back. call by other function, when not enough money or cancel
	private void setInfoBack()
	{
		tankCD = initialtankCD;
		tankDam = initialtankDam;
		tankMaxHP = initialtankMaxHP;
		tankSp = initialtankSp;
		
		tankCDCst = itankCDC;
		tankDamCst = itankDamC;
		tankSpCst = itankSpC;
		tankMaxHPCst = itankMaxHPC;
	}
	//=================
	private void executeUpdateQuerry(String querry)
	{
		dbcmd.CommandText = querry; //tankCD
		reader = dbcmd.ExecuteReader ();
	}
	// Call to set the value when load in or change different tytpe of unit you want to upgrade.
	private void setValue(string UnitType)
	{
		string name=reader.GetString (0);
		
		//setting up values of tankMaxHP,tankDam,tankCD,tankSP and cost of each.
		if (name == UnitType)
		{
			tankDam = reader.GetInt32 (1);
			tankMaxHP = reader.GetInt32(2);
			tankCD = reader.GetFloat(3);
			tankSp = reader.GetInt32(4);
		}
		//memorize initial value before change
		initialtankMaxHP=tankMaxHP;
		initialtankDam=tankDam;
		initialtankCD=tankCD;
		initialtankSp=tankSp;
		//memorize inital cost 
		itankMaxHPC=tankMaxHPCst;
		itankDamC= tankDamCst;
		itankCDC= tankCDCst;
		itankSpC= tankSpCst;
		TxtArmor.text = tankMaxHP.ToString();
		TxtDamage.text = tankDam.ToString();
		TxtFireRate.text =  tankCD.ToString();
		TxtMovement.text = tankSp.ToString();
	}

}
