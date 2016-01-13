using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour {

	// Use this for initialization
	void Start () {        

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Fixed Update called once per physics update
    void FixedUpdate()
    {
        transform.Rotate( new Vector3(15.0f, 30.0f, 45.0f) * Time.deltaTime );
    }
}
