using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevelButton : MonoBehaviour {

    public int levelNum;   

	// Use this for initialization
	void Start ()
    {
               
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void loadlevel()
    {
        Time.timeScale = 1.0f;        
        Cursor.visible = true;
        AudioListener.pause = false;
        SceneManager.LoadScene(levelNum);                
    }
}
