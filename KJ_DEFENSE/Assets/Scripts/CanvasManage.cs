using UnityEngine;
using System.Collections;

public class CanvasManage : MonoBehaviour {

	public float speed;
	public RectTransform healthtransform;
	private float cachedY;
	private float minXValue;
	private float maxXValue;
	public int currentHealth;
	public int maxHealth;

	public Kaiju_Health monster;
	// Use this for initialization
	void Start () {
		cachedY = healthtransform.position.y;
		maxXValue = healthtransform.position.x;
		minXValue = healthtransform.position.x - healthtransform.rect.width;
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		handleHealth ();


	}

	private float MapValue(float x, float inMin, float inMax, float outMin, float outMax)
	{
		return 	(x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
	public void handleHealth()
	{
		float currentXValue = MapValue (currentHealth, 0, maxHealth, minXValue, maxXValue);
		healthtransform.position = new Vector3 (currentXValue, cachedY);
	}
}
