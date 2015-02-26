using UnityEngine;
using System.Collections;

public class SceneChangetest : MonoBehaviour {

	// Use this for initialization
	public void Start () {

	}
	
	// Update is called once per frame
	public void ToWorldMap () {
		Application.LoadLevel (3);
	}
	public void ToCredit () {
		Application.LoadLevel (4);
	}
	public void ToGame()
	{
		Application.LoadLevel (1);
	}
	public void ToMainMenu()
	{
		Application.LoadLevel (0);
	}
	public void ToUpgrade()
	{
		Application.LoadLevel (2);
	}
}
