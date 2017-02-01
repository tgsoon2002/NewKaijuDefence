using UnityEngine;
using System.Collections;

public class SpawnIconBehavior : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x + 2.0f, -0.97903f, 0.20308f);
    }
}