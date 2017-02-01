using UnityEngine;
using System.Collections;

public class WMCameraManager : MonoBehaviour 
{

	private Vector3 temp;
	private float dragSpeed = -3.0f;
	private float zoomSpeed = .1f;
	private Vector3 lastMousePosition;
	private Vector3 lastMapPosition;
    private float lerpDuration = 1.0f;
    private bool isPanning = true;
	private bool mouseDown = false;
    
    private static WMCameraManager _instance;

    public static WMCameraManager instance
    {
        get { return _instance; }
    }

    public bool Camera_Pan_Mode
    {
        get { return isPanning; }
        set { isPanning = value; }
    }

    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        _instance = this;
    }

	void Start () 
    {
		//camHalfWidth = Camera.main.orthographicSize * ((float)Screen.width/Screen.height);
		lastMapPosition = Camera.main.transform.position;
		//lastMapPosition.z = -2.5f;
		temp = lastMapPosition;
		//temp.z = -2.5f;
	}
	
	// Update is called once per frame
	void Update () 
    {
        Debug.Log("Camera is in Panning Mode: " + isPanning);

		if (Input.GetMouseButton(0) && isPanning) 
        {
			if (!mouseDown ) 
            {
				mouseDown = true;
				lastMousePosition = Input.mousePosition;
			}

			CameraPanning(lastMousePosition);
		}

		if (Input.GetMouseButtonUp(0) && isPanning) 
        {
			mouseDown = false;
			lastMapPosition = Camera.main.transform.position;
		}

		// Button to zoom in and out.
		if (Input.GetKey("=") ) {
			zoomCamera(1);
		}

		if (Input.GetKey("-") ) {
			zoomCamera(2);
		}
	}

	public void CameraPanning(Vector3 mousePosOrigin)
	{
		//Transforms the mouse position from screen space to viewport space
		Vector3 mousePosNormalized = Camera.main.ScreenToViewportPoint(Input.mousePosition - mousePosOrigin);

		//This will be the new position of the mouse, thus will also be the camera's new position
		Vector3 mouseDestination = new Vector3(mousePosNormalized.x *dragSpeed, mousePosNormalized.y *dragSpeed , 0);

		temp = lastMapPosition + mouseDestination;
		//This will Translate, or move, the camera's position to the mouse's position
		Camera.main.transform.position = temp;
	}

    public void CameraFocus(Vector3 obj)
    {
        //Declaring local variables
        Vector3 newPosition;
        Vector3 currentPosition;

        currentPosition = gameObject.transform.position;

        //Initializing local variables
        newPosition = new Vector3(0.0f, 0.0f, 0.0f);
        newPosition.x = obj.x;
        newPosition.y = obj.y;
        newPosition.z = currentPosition.z;

        StartCoroutine(MoveCamToDestination(gameObject.transform.position, newPosition));    
    }

	public void zoomCamera(int level)
	{
	
		switch (level) 
        {

		case 1:
			temp.z += zoomSpeed ;
			lastMapPosition = temp;
			Camera.main.transform.localPosition = temp;
			break;
		case 2:
			temp.z -= zoomSpeed ;
			lastMapPosition = temp;
			Camera.main.transform.localPosition = temp;
			break;
		default:
		break;

		}
	}

    IEnumerator MoveCamToDestination(Vector3 curr, Vector3 pos)
    {
        float t = 0.0f;

        while(t < lerpDuration)
        {
            gameObject.transform.position = Vector3.Lerp(curr, pos, t / lerpDuration);
            t += Time.deltaTime;
            lastMapPosition = Camera.main.transform.position;
            if(t >= lerpDuration)
            {
                mouseDown = false;
                //lastMapPosition = Camera.main.transform.position;

                EventManager.TriggerEvent("CreateHQIcon");
            }

            yield return new WaitForFixedUpdate();
        }

    }
}
