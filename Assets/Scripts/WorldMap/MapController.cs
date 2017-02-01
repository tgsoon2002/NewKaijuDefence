using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System.IO;
using System.Data;
using Mono.Data.SqliteClient;
using UnityEngine.SceneManagement;

public class MapController : MonoBehaviour
{
    #region Data Members

    private enum MissionState
    {
        HQ_SELECT,
        NORMAL,
        MENU_ACTION,
        COMBAT_SCENE
    }

    public Canvas canvas;
    public Text headerText;
    public GameObject hqIcon;
    public GameObject hqPanel;
   
    //Data members for linking with our database
    IDbConnection dbconn;
    IDbCommand dbcmd;
    IDataReader reader;
    string conn;

    //region is hard-coded to 1, generalize this later!
    private int regionID = 1;
    private int cityHQID;

    private MissionState state;

    private delegate void RegionInputs(MissionState s);

    private RegionInputs selectInputs;

    private int maxSquad;
    private GraphADT region;
    private static MapController _instance;
    private bool sceneChange;
    private bool selectHQInit;
    private bool selectHQChoice;
    private bool confirmHQButtonsPressed;

    private GameObject inGameHQIcon;
    public GameObject sampleCity;
    public CityNode headQuarters;
    private bool squadSelected;
    private Squad squadObject;
    private Ray ray;
    private RaycastHit hit;
    private List<int> possibleHQNodes;
    private List<GameObject> regionHQNodes;

    #endregion

    #region Setters & Getters

    public static MapController instance
    {
        get { return _instance; }
    }

    #endregion

    #region Built-in Unity Methods

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        EventManager.StartListening("CreateHQIcon", CreateHQIcon);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        EventManager.StopListening("CreateHQIcon", CreateHQIcon);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnLevelWasLoaded()
    {
        if (sceneChange)
        {
            if (Application.loadedLevelName == "WorldMap")
            {
                CreateCity();
            }
        }

        if (GameController.Get_Last_Scene_Loaded == "MissionBriefing")
        {
            selectHQInit = true;
        }
        else
        {
            selectHQInit = false;
        }
    }

    /// <summary>
    /// Initializes data members when the script is first loaded.
    /// If DontDestroyOnLoad is enabled, Start does not get called
    /// more than once.
    /// </summary>
    void Start()
    {
        state = MissionState.HQ_SELECT;
        selectInputs += SelectHQInput;

        //maxSquad's value is hard-coded for now. Needs to be 
        //generalized for later.
        maxSquad = 3;
        squadSelected = false;
        region = new GraphADT();
	
        conn = "URI=file:Assets/Plugin/KJData.s3db";
        dbconn = (IDbConnection)new SqliteConnection(conn);
        LoadCityData();
        LoadCityNeighborData();
        CreateCity();

        headerText.text = "SELECT HQ";
        LoadRegionMainHQSlotsData();
        LoadHQSelectUI();

        sceneChange = false;
        selectHQChoice = false;

        //Canvas shit
        //canvas.GetComponent<Animator>().SetBool("HQButtonPressed", true);
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        if (WMCameraManager.instance.Camera_Pan_Mode = true)
        {
            if (Input.GetButtonDown("ControllerA"))
            {
                HQSelectButtonPressed(0);
            }
            if (Input.GetButtonDown("ControllerB"))
            {
                HQSelectButtonPressed(1);
            }
            if (Input.GetButtonDown("ControllerX"))
            {
                HQSelectButtonPressed(2);
            }
            if (Input.GetButtonDown("ControllerY"))
            {
                HQSelectButtonPressed(3);
            }
        }
        if (!selectHQChoice)
        {

            if (EventSystem.current.IsPointerOverGameObject())
            {
                WMCameraManager.instance.Camera_Pan_Mode = false;
            }
            else
            {
                WMCameraManager.instance.Camera_Pan_Mode = true;
            }
        }

        //selectInputs(state);

        //if(Input.GetMouseButtonUp(0))
        //{
        //    if(!squadSelected && headQuarters != null)
        //    {
        //        SelectSquad();
        //    }
        //    else if(headQuarters != null)
        //    {
        //        MoveSelectedSquad();
        //    }
        //    else
        //    {
        //        SelectHeadQuarters();
        //    }
        //}

        //if(Input.GetMouseButtonUp(1) && squadSelected)
        //{
        //    squadSelected = false;
        //    squadObject = null;
        //}
        //else if(Input.GetMouseButtonUp(1) && !squadSelected)
        //{
        //    Debug.Log("Damage.");
        //    Damagecity();
        //}
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="HQ"></param>
    public void SetHeadQuarters(CityNode HQ)
    {
        headQuarters = HQ;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="city"></param>
    public void CityInfo(GameObject city)
    {
        region.SetVertexHealth(city.GetComponent<CityNode>().cityID, city.GetComponent<CityNode>().healthPercent);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    public void SceneTransition(bool val)
    {
        sceneChange = val;
    }

    /// <summary>
    /// 
    /// </summary>
    public void CreateSquad()
    {
        for (int i = 0; i < maxSquad; i++)
        {
            if (!SquadManager.instance.CheckSquadIDExist(i) && headQuarters != null)
            {
                GameObject newSquad = Instantiate(Resources.Load("WorldMap/PlayerSquad"), 
                                          headQuarters.transform.position, Quaternion.identity) as GameObject;

                SquadManager.instance.newSquadID = i;
                newSquad.GetComponent<Squad>().SetSquadID(i);
                DontDestroyOnLoad(newSquad);
                i = maxSquad;
                //SceneChangetest.ToDeploy();
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    public void ConfirmHQButtonPressed(bool val)
    {
        confirmHQButtonsPressed = val;

        if (!val)
        {
            canvas.GetComponent<Animator>().SetBool("HQButtonPressed", false);
            WMCameraManager.instance.Camera_Pan_Mode = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="val"></param>
    public void ConfirmPanelSlideOutEvent()
    {
        if (!confirmHQButtonsPressed)
        {
            hqPanel.SetActive(true);
            inGameHQIcon.SetActive(false);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void TriggerEevent()
    {
        EventManager.TriggerEvent("EndScene");
    }

    public void HQSelectButtonPressed(int id)
    {
        WMCameraManager.instance.CameraFocus(region.GetVertexPosition(possibleHQNodes[id]));
        WMCameraManager.instance.Camera_Pan_Mode = false;
        Debug.Log(WMCameraManager.instance.Camera_Pan_Mode);
        cityHQID = possibleHQNodes[id];
    }

    /// <summary>
    /// 
    /// </summary>
    void CreateHQIcon()
    {
        if (inGameHQIcon == null)
        {
            Debug.Log(region.GetVertexPosition(cityHQID));
            inGameHQIcon = Instantiate(hqIcon, new Vector3(region.GetVertexPosition(cityHQID).x,    
                    region.GetVertexPosition(cityHQID).y + 0.2f,
                    region.GetVertexPosition(cityHQID).z), Quaternion.identity) as GameObject;
        }
        else
        {
            inGameHQIcon.transform.position = new Vector3(region.GetVertexPosition(cityHQID).x,
                region.GetVertexPosition(cityHQID).y + 0.2f,
                region.GetVertexPosition(cityHQID).z);
        }

        hqPanel.SetActive(false);
        WMCameraManager.instance.Camera_Pan_Mode = false;
        selectHQChoice = true;
        inGameHQIcon.SetActive(true);
        canvas.GetComponent<Animator>().SetBool("HQButtonPressed", true);

    }

    #endregion


    #region Helper Methods

    void SelectHQInput(MissionState s)
    {
        if (s == MissionState.HQ_SELECT)
        {
            if (Input.GetMouseButtonUp(0))
            {
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 50.0f))
                {
                    if (hit.collider.gameObject.tag == "City" &&
                        (possibleHQNodes.Exists(o => o == (hit.collider.gameObject.GetComponent<CityNode>().cityID))))
                    {
                        Debug.Log("Possible HQ Selected!");
                        state = MissionState.NORMAL;
                        headerText.text = "SELECT THIS BASE?";
                        Debug.Log(state);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadCityData()
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM City";
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            //Adding this city into a graph data structure for later use.
            region.AddNode(reader.GetInt32(0), reader.GetString(1),
                new Vector3(reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6)),
                reader.GetFloat(2));
        }

        dbconn.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadRegionMainHQSlotsData()
    {
        //Instantiate this list here.
        possibleHQNodes = new List<int>();

        //Open the database reader
        dbconn.Open();

        //Tell the database to start taking in commands.
        dbcmd = dbconn.CreateCommand();

        //Actual database command.
        string sqlQuery = ("SELECT * FROM Region where RegionID = " + regionID);

        //Set the command and execute
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();
        reader.Read();  
        //Read the results from the query
        possibleHQNodes.Add(reader.GetInt32(3));
        possibleHQNodes.Add(reader.GetInt32(4));
        possibleHQNodes.Add(reader.GetInt32(5));
        possibleHQNodes.Add(reader.GetInt32(6));

        //Close the database reader
        dbconn.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadCityNeighborData()
    {
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        //Region ID is hard-coded, gonna have to generalize that later.
        string sqlQuery = ("SELECT * FROM CityNeighbor where CityID in (Select CityID from City where RegionID = " + 1 + ")");
        dbcmd.CommandText = sqlQuery;
        reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            region.AddDirectedEdge(reader.GetInt32(0), reader.GetInt32(1));
        }

        dbconn.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    void LoadHQSelectUI()
    {
        hqPanel.SetActive(true);

        for (int i = 0; i < possibleHQNodes.Count; i++)
        {
            if (possibleHQNodes[i] == -1)
            {
                hqPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void CreateCity()
    {
        //Declaring local variables
        var tmp = region.Get_Vertex_ID_List;

        for (int i = 0; i < tmp.Length; i++)
        {
            GameObject _city = Instantiate(sampleCity, region.GetVertexPosition(tmp[i]),
                                   Quaternion.identity) as GameObject;

            _city.GetComponent<CityNode>().cityID = tmp[i];
            _city.GetComponent<CityNode>().cityName = region.GetVertexName(tmp[i]);
            _city.GetComponent<CityNode>().healthPercent = region.GetVertexHealth(tmp[i]);
            _city.GetComponent<CityNode>().UpdateInfo();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void MoveSelectedSquad()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City" && squadObject.GetComponent<Squad>().origin != null)
            {
                if (squadObject.GetComponent<Squad>().origin.NearbyTown(hit.collider.GetComponent<CityNode>()))
                {
                    squadObject.MoveSquad(hit.collider.GetComponent<CityNode>());
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SelectHeadQuarters()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 50.0f, Color.green);

        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City")
            {
                SetHeadQuarters(hit.collider.gameObject.GetComponent<CityNode>());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void SelectSquad()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 50.0f, Color.green);

        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "Unit")
            {
                squadObject = hit.collider.gameObject.GetComponent<Squad>();
                squadSelected = true;

                if (squadObject.GetComponent<Squad>().origin == null)
                {
                    squadObject.GetComponent<Squad>().origin = headQuarters.GetComponent<CityNode>();
                }
            }

            if (hit.collider.gameObject.tag == "City")
            {
                squadSelected = false;
            }
        }
        else
        {
            squadSelected = false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void Damagecity()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 50.0f))
        {
            if (hit.collider.gameObject.tag == "City")
            {
                hit.collider.gameObject.GetComponent<CityNode>().LostHealth(0.1f);
                region.SetVertexHealth(hit.collider.gameObject.GetComponent<CityNode>().cityID,
                    hit.collider.gameObject.GetComponent<CityNode>().healthPercent);
            }
        }
    }

    #endregion
}
