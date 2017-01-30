using UnityEngine;
using System.Collections;

public class SpawnIconBehavior : MonoBehaviour
{
    //Declaring local members

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, -0.97903f, 0.20308f);
    }
}