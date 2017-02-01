using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class BuildList : MonoBehaviour {

	#region Data Member
	//Data members for linking with our database
	IDbConnection dbconn;
	IDbCommand dbcmd;
	IDataReader reader;
	string conn;

	public struct Recipe
	{
		public int unitID;
		// Ingredient ID
		public int bdIngre1;
		public int bdIngre2;
		public int bdIngre3;
		public int bdIngre4;
		//Quantity need for each ingredient
		public int bdIngreQt1;
		public int bdIngreQt2;
		public int bdIngreQt3;
		public int bdIngreQt4;
		//Money need to build
		public int money;
		// Premimium currency
		public int ResearchPoint;
	}

	public struct Resource
	{
		public int resID;
		public string resName;
		public int resQuantity;
	}

	private List<Recipe> listOfRecipe;
	private List<Resource> listOfResource;

	private static BuildList _instance;
	public static BuildList instance
	{
		get { return _instance; }
	}

	#endregion

	#region getter and setter
	// This use for get infor from resouse
	public string NameOfResWithID(int ResID)
	{
		return listOfResource[FindIndex(ResID)].resName;
	}
	public int QuanOfResWithID(int ResID)
	{
		return listOfResource[FindIndex(ResID)].resQuantity;
	}
	public bool EnoughRes(int resID,int req){
		return (listOfResource[FindIndex(resID)].resQuantity >= req);
	}
	public void ChangeQuantity(int resID, int newQuan){
		int temp = QuanOfResWithID(resID) - newQuan;
		string sqlQuery = "UPDATE Resource SET Quantity = " + temp.ToString() + " Where ResID = " + resID;
		ExecuteUpdateQuerry(sqlQuery);
		RetriveNewValue("select ResID,ResName,Quantity from Resource");
		SetValueResource();
	}
	// This use for blueprint
	//---Name---
	public string NameOfFirIngreWithID(int UID)
	{
		return NameOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre1);
	}
	public string NameOfSecIngreWithID(int UID)
	{
		return NameOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre2);
	}
	public string NameOfThiIngreWithID(int UID)
	{
		return NameOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre3);
	}
	public string NameOfFouIngreWithID(int UID)
	{
		return NameOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre4);
	}
	//---Quantity---
	public int QuanOfFirIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngreQt1;
	}
	public int QuanOfSecIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngreQt2;
	}
	public int QuanOfThiIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngreQt3;
	}
	public int QuanOfFouIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngreQt4;
	}
	//---QuanInstock---
	public int QuanInstockOfFirIngreWithID(int UID)
	{
		return QuanOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre1);
	}
	public int QuanInstockOfSecIngreWithID(int UID)
	{
		return QuanOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre2);
	}
	public int QuanInstockOfThiIngreWithID(int UID)
	{
		return QuanOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre3);
	}
	public int QuanInstockOfFouIngreWithID(int UID)
	{
		return QuanOfResWithID(listOfRecipe[FindIndex(UID)].bdIngre4);
	}
	//---ResID---
	public int IDOfFirIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngre1;
	}
	public int IDOfSecIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngre2;
	}
	public int IDOfThiIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngre3;
	}
	public int IDOfFouIngreWithID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].bdIngre4;
	}
	//---Money---
	public int MoneyCostID(int UID)
	{
		return listOfRecipe[FindIndex(UID)].money;
	}
	#endregion

	#region built-in Method
	void Awake(){
		_instance = this;
	}

	// Use this for initialization
	void Start () {
		listOfRecipe = new List<Recipe>();
		listOfResource = new List<Resource>();
		conn = "URI=file:Assets/Plugin/KJData.s3db";
		dbconn = (IDbConnection)new SqliteConnection(conn);
		dbcmd = dbconn.CreateCommand();
		// set connection string and open the conenction
		RetriveNewValue("select ResID,ResName,Quantity from Resource");
		SetValueResource();
		RetriveNewValue("select * from RDFactory");
		SetValueRecipe();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion

	#region Helper Method
	int FindIndex(int id)
	{
		return listOfResource.FindIndex(resourse => resourse.resID == id);
	}

	void RetriveNewValue(string sqlQuery)
	{

		dbconn.Open();
		dbcmd.CommandText = sqlQuery;
		reader = dbcmd.ExecuteReader();		
	}

	private void SetValueResource()
	{
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

	private void SetValueRecipe()
	{
		listOfRecipe.Clear();
		while (reader.Read()) {
			Recipe temp = new Recipe();
			temp.unitID = reader.GetInt32(0);
			temp.bdIngre1 = reader.GetInt32(1);
			temp.bdIngre2 = reader.GetInt32(3);
			temp.bdIngre3 = reader.GetInt32(5);
			temp.bdIngre4 = reader.GetInt32(7);
			temp.bdIngreQt1 = reader.GetInt32(2);
			temp.bdIngreQt2 = reader.GetInt32(4);
			temp.bdIngreQt3 = reader.GetInt32(6);
			temp.bdIngreQt4 = reader.GetInt32(8);
			temp.money = reader.GetInt32(9);
			temp.ResearchPoint = reader.GetInt32(10);
			listOfRecipe.Add(temp);
		}
		dbconn.Close();
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
	#region Modify DB Method


	#endregion
}
