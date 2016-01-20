using UnityEngine;
using System.Collections;

public class ResumeGameTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void resume()
    {
        Time.timeScale = 1.0f;
    }
}
