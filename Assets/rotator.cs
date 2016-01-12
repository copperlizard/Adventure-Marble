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
        transform.Rotate(0.15f, 0.3f, 0.45f);
    }
}
