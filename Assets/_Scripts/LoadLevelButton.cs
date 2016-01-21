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
        Cursor.visible = true;
        SceneManager.LoadScene(levelNum);                
    }
}
