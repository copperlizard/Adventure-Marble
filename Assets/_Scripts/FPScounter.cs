using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPScounter : MonoBehaviour {

    public Text fpsText;
    private float deltaTime;

	// Use this for initialization
	void Start ()
    {
        deltaTime = 0.0f;	
	}
	
	// Update is called once per frame
	void Update ()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;

        float fps = 1.0f / deltaTime;

        fpsText.text = "FPS: " + fps.ToString();
    }
}
