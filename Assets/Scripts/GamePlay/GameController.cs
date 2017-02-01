using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.SqliteClient;

public class GameController : MonoBehaviour
{
    #region Data Members

    private static string lastSceneLoaded;
    private static string currentScene;
    private static GameController _instance;

    #endregion

    #region Setters & Getters

    public static GameController instance
    {
        get
        {
            if(!_instance)
            {
                _instance = FindObjectOfType(typeof(GameController)) as GameController;

                if(!_instance)
                {
                    Debug.LogError("No GameController GameObject detected!");
                }
            }

            return _instance;
        }
    }

    public static string Get_Last_Scene_Loaded
    {
        get { return lastSceneLoaded; }
    }

    public static string Get_Current_Scene
    {
        get { return currentScene; }
    }

    #endregion

    #region Built-in Unity Methods

    /// <summary>
    /// 
    /// </summary>
	void Awake() 
    {
	    DontDestroyOnLoad(this.gameObject);
	}
	
    #endregion

    #region Main Methods

    public static void LoadScene(string sceneName)
    {
        lastSceneLoaded = currentScene;
        currentScene = sceneName;
        Application.LoadLevel(currentScene);
    }

    #endregion

    #region Helper Methods

    #endregion
}
