using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuClick : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    // Load Scene
    public void load( int ind )
    {
        SceneManager.LoadScene( ind );
    }

    // End application
    public void quitGame()
    {
        Application.Quit();
    }
}
