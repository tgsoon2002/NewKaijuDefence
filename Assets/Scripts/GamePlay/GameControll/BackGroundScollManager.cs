using UnityEngine;
using System.Collections;

public class BackGroundScollManager : MonoBehaviour {
	public float scrollSpeed = 0.5F;
	private Vector2 saveOffset;
	public Renderer rend;

	void Start () 
    {
		rend = this.GetComponent<Renderer> ();
		saveOffset = rend.sharedMaterial.GetTextureOffset ("_MainTex");
	}
	
	// Update is called once per frame
	void Update () 
    {
		if (rend.enabled)
        {
			float offset = Mathf.Repeat (Time.time * scrollSpeed,1);	
			Vector2 vOffSet = new Vector2 (saveOffset.x, offset);
			rend.sharedMaterial.SetTextureOffset ("_MainTex", vOffSet);
		}
		else 
        {
			Debug.Log("render disable");
				
        }
	}
}
